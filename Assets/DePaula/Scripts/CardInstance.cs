using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;
using DG.Tweening;

public class CardInstance : MonoBehaviour, IGameEntity
{
    [Header("Card Info")]
    public CardData cardData;

    // Componentes adjacentes
    public CardVisuals CardVisuals { get; private set; }
    DraggableComponent draggable;


    HealthSystemTemplate healthSystem, attackSystem;
    public CardMode Mode { get; private set; }
    int turnsToSleep = 0;
    public Targeting AttackTargeting { get; set; } = Targeting.EnemyInFront;
    public IGameEntity Murderer { get; private set; } = null;
    List<GameAction> murdererActions;

    // Effect Control
    Dictionary<EffectActivationData, bool> effectsUsed;

    // ------------------------------------------------------------------------------------------GameEntity Stuff
    public bool IsPlayer1 => isPlayer1;
    public string Id => id;
    public GameObject GameObject => gameObject;

    string id;
    bool isPlayer1;
    // ------------------------------------------------------------------------------------------GameEntity Stuff

    // ----------------------------------- Draggable stuff
    public GameObject CurrentSlot {  get; set; }
    bool canBeSelected;
    GameObject targetPrefab;
    

    


    public void SetupCardInstance(CardData data, bool isPlayer1)
    {
        // Coloca a informacao da carta na instancia
        cardData = data;

        // Prepara componentes nao visuais
        healthSystem = new HealthSystemTemplate(cardData.health);
        attackSystem = new HealthSystemTemplate(cardData.attack);
        Mode = CardMode.InHand;
        this.isPlayer1 = isPlayer1;
        id = Guid.NewGuid().ToString();


        // Pega os componentes adjacentes
        CardVisuals = GetComponent<CardVisuals>();
        draggable = GetComponent<DraggableComponent>();

        // Setup componentes adjacentes
        CardVisuals.Setup(cardData, healthSystem, attackSystem);
        draggable.Setup(this);

        // Outros
        Murderer = null;
        effectsUsed = new Dictionary<EffectActivationData, bool>();

        foreach(var effect in cardData.effects)
        {
            effectsUsed[effect] = false;
        }
    }

    #region HealthMethods

    public void TakeDamage(IGameEntity source, int amount)
    {
        if (healthSystem.CurrentHealth == 0)
        {
            return;
        }

        List<EffectActivationData> effects = cardData.GetEffectsByTime(TimeToActivate.OnTakeDamage);

        foreach (EffectActivationData effect in effects)
        {
            if (!cardData.CheckIfCanUse(effect, effectsUsed[effect])) continue;
            IGameEntity[] ige = { source };
            amount += EffectHandler.Instance.ActivateEffectImmediatly(effect.effect, this, ige, effect.specialParameter, amount);
            effectsUsed[effect] = true;
        }

        transform.DOShakeRotation(0.5f, new Vector3(0, 0, 20), vibrato: 15);
        

        NumberPopup.Create(transform.position, amount, false);

        //--------------------------------------------------- DEBUG ---------------------------------
        //CardInstance ci = source as CardInstance;
        //Debug.LogWarning($"{effects.Count}x -> {cardData.cardName} recebeu {amount} de dano provindo de {ci.cardData.cardName}!");
        //--------------------------------------------------- DEBUG ---------------------------------

        healthSystem.TakeDamage(amount);
        CardVisuals.UpdateHealthUI(healthSystem);

        if (healthSystem.CurrentHealth == 0)
        {
            Murderer = source;
        }
    }

    public void Heal(int amount)
    {
        NumberPopup.Create(transform.position, amount, true);   
        healthSystem.Heal(amount);
        CardVisuals.UpdateHealthUI(healthSystem);
    }

    public IGameEntity Die()
    {
        if (Murderer == null) return null;

        Mode = CardMode.Dead;

        CardInstance ci = Murderer as CardInstance;

        

        GameManager.Instance.GetDeck(isPlayer1).DeadCards(cardData, CurrentSlot.GetComponent<CardSlot>());
        

        ReleaseSlot();

        AudioManager.instance.PlaySound("Explosion");
        //ParticleManager.instance.PlayParticle("Explosion", transform.position, Quaternion.identity);

        gameObject.SetActive(false);

        //////////////////////////
        List<EffectActivationData> effects = cardData.GetEffectsByTime(TimeToActivate.OnDeath);

        foreach (EffectActivationData effect in effects)
        {
            if (!cardData.CheckIfCanUse(effect, effectsUsed[effect])) continue;
            GameAction res = new GameAction(this, effect);
            EffectHandler.Instance.ActivateEffectImmediatly(res);
            effectsUsed[effect] = true;
        }
        //////////////////////////
        

        return Murderer;
    }

    public int GetCurrentHealth()
    {
        return healthSystem.CurrentHealth;
    }

    public bool TryRevive()
    {
        if (GameManager.Instance.GetDeck(isPlayer1).deadCards.ContainsKey(cardData))
        {
            return true;
        }

        return false;
    }

    public bool Revive(CardSlot slot, int life = -1)
    {
        if (slot == null || !slot.empty)
        {
            return false;
        }

        gameObject.SetActive(true);
        slot.PutCardInSlot(this);
        Mode = CardMode.InPlay;
        CurrentSlot = slot.gameObject;

        healthSystem = new HealthSystemTemplate(cardData.health, life);
        attackSystem = new HealthSystemTemplate(cardData.attack);
        CardVisuals.UpdateAttackUI(attackSystem);
        CardVisuals.UpdateHealthUI(healthSystem);

        //foreach (var effect in cardData.effects)
        //{
        //    effectsUsed[effect] = false;
        //}

        return true;
    }
    #endregion

    public void FinishedPlayCard()
    {
        EnqueueEffects(TimeToActivate.OnReveal);
    }

    #region Effect Activations
    public void BecomeAKiller()
    {
        List<EffectActivationData> effects = cardData.GetEffectsByTime(TimeToActivate.OnKill);

        foreach (EffectActivationData effect in effects)
        {
            if (!cardData.CheckIfCanUse(effect, effectsUsed[effect])) continue;
            GameAction res = new GameAction(this, effect);
            EffectHandler.Instance.ActivateEffectImmediatly(res);
            effectsUsed[effect] = true;
        }
    }

    private List<GameAction> EnqueueEffects(TimeToActivate state)
    {
        List<EffectActivationData> effects = cardData.GetEffectsByTime(state);
        List<GameAction> gamesActions = new List<GameAction>();

        foreach (EffectActivationData effect in effects)
        {
            if (!cardData.CheckIfCanUse(effect, effectsUsed[effect])) continue;
            GameAction res = new GameAction(this, effect);
            EffectHandler.Instance.EnqueueEffect(effect.timeToActivate, res);
            gamesActions.Add(res);
            effectsUsed[effect] = true;
        }        

        return gamesActions;
    }

    public List<GameAction> StartTurnEffects()
    {
        return EnqueueEffects(TimeToActivate.OnStartOfTurn);
    }

    #endregion

    #region Attack Methods
    public int GetAttackDamage(IGameEntity tg)
    {
        if (turnsToSleep > 0)
        {
            AudioManager.instance.PlaySound("Mimir");
            turnsToSleep--;

            return 0;
        }

        List<EffectActivationData> effects = cardData.GetEffectsByTime(TimeToActivate.OnAttack);
        int dmg = attackSystem.CurrentHealth;

        foreach (EffectActivationData effect in effects)
        {
            if (!cardData.CheckIfCanUse(effect, effectsUsed[effect])) continue;
            IGameEntity[] ige = { tg };
            dmg += EffectHandler.Instance.ActivateEffectImmediatly(effect.effect, this, ige, effect.specialParameter);
            effectsUsed[effect] = true;
        }

        return dmg;
    }


    #endregion

    #region Buffs and Inertness
    public bool Buff(IGameEntity source, Stat stat, int amount)
    {
        if ((stat & Stat.Health) != 0)
        {
            healthSystem.BuffMaxHealth(source, amount, true);
            CardVisuals.UpdateHealthUI(healthSystem);
            return true;
        }
        if ((stat & Stat.Attack) != 0)
        {
            attackSystem.BuffMaxHealth(source, amount, true);
            CardVisuals.UpdateAttackUI(attackSystem);
            return true;
        }

        return false;
    }

    public bool TryUndoBuff(IGameEntity source, out int extra)
    {
        if (healthSystem.TryUndoBuff(source, out extra) || attackSystem.TryUndoBuff(source, out extra))
        {
            CardVisuals.UpdateHealthUI(healthSystem);
            CardVisuals.UpdateAttackUI(attackSystem);
            return true;
        }

        return false;
    }

    public bool MakeInert(int amount)
    {
        turnsToSleep += amount;
        UpdateUIInertEffect();
        return true;
    }

    public void UpdateUIInertEffect()
    {
        CardVisuals.UpdateUIInertEffect(turnsToSleep > 0);
    }

    #endregion


    public void FlipCard(bool faceUp)
    {
        CardVisuals.FlipCard(faceUp);
    }


    public void ReleaseSlot()
    {
        CardSlot currentCardSlot = CurrentSlot.GetComponent<CardSlot>();
        if (CurrentSlot != null && !currentCardSlot.empty)
        {
            currentCardSlot.empty = true;
            currentCardSlot.CardInstance = null;
            CurrentSlot = null;
        }
    }

    public async Task ConfirmPlay(CardSlot slot)
    {
        Debug.Log("Play Confirmed!");
        Mode = CardMode.InPlay;
        CurrentSlot = slot.gameObject;

        List<EffectActivationData> effects = cardData.GetEffectsByTime(TimeToActivate.OnPlay);

        foreach (EffectActivationData effect in effects)
        {
            if (!cardData.CheckIfCanUse(effect, effectsUsed[effect])) continue;
            GameAction res = new GameAction(this, effect);
            await EffectHandler.Instance.CardPlayed(res);
            effectsUsed[effect] = true;
        }

        FinishedPlayCard();

        //return Task.CompletedTask;
    }

    public void PossibleTargetToClick()
    {
        //Debug.LogError("SelectionStart!");
        canBeSelected = true;
    }

    public void SelectionOver()
    {
        //Debug.LogError("SelectionOver!");
        canBeSelected = false;
        if (targetPrefab != null)
        {
            Destroy(targetPrefab);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canBeSelected)
        {            
            //OnPointerExit(eventData);

            var canvasRect = GetComponentInParent<Canvas>().transform as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, eventData.position, eventData.pressEventCamera, out var localPoint);

            targetPrefab = Instantiate(TargetSelector.Instance.Selected(this), canvasRect);
            targetPrefab.GetComponent<RectTransform>().anchoredPosition = localPoint;

            canBeSelected = false;
        }
    }
}

public enum CardMode
{
    InHand = 0,
    InPlay = 1,
    Dormant = 2,
    Dead = 3
}
