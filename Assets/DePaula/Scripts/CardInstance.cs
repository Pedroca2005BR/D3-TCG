using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

public class CardInstance : MonoBehaviour, IGameEntity, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public enum CardMode
    {
        InHand = 0,
        InPlay = 1,
        Dormant = 2,
        Dead = 3
    }

    [Header("Card Info")]
    public CardData cardData;
    

    [Header("Display Info")]
    [SerializeField] TextMeshProUGUI nameComponent;
    [SerializeField] GameObject descriptionImage;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI healthComponent;
    [SerializeField] TextMeshProUGUI attackComponent;
    [SerializeField] Image cardArtComponent;
    [SerializeField] Image backgroundComponent;


    HealthSystemTemplate healthSystem, attackSystem;
    public CardMode Mode { get; private set; }
    int turnsToSleep = 0;
    public Targeting AttackTargeting { get; set; } = Targeting.EnemyInFront;
    public IGameEntity Murderer { get; private set; } = null;
    List<GameAction> murdererActions;

    // ------------------------------------------------------------------------------------------GameEntity Stuff
    public bool IsPlayer1 => isPlayer1;
    public string Id => id;

    public GameObject frontSide;
    public GameObject backSide;

    public GameObject GameObject => gameObject;

    string id;
    bool isPlayer1;
    // ------------------------------------------------------------------------------------------GameEntity Stuff

    // ----------------------------------- Draggable stuff
    public List<GameObject> slotObjects;
    public GameObject CurrentSlot {  get; private set; }
    public bool dropped = false;
    public float snapRange = 1f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 dragTargetPosition;
    bool isBeingDragged;
    Vector3 dragOffset;
    bool canBeSelected;
    GameObject targetPrefab;
    Vector3 initialPosition;

    // --------------------- Special visuals
    IEnumerator descriptionCoroutine;


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

        // Prepara textos
        nameComponent.text = cardData.cardName;
        descriptionText.text = cardData.cardDescription;
        ChangeHealthComponent();
        ChangeAttackComponent();

        // Prepara artes
        cardArtComponent.sprite = cardData.cardArt;
        backgroundComponent.sprite = cardData.backgroundArt;
        descriptionImage.gameObject.SetActive(false);

        // Prepara corotinas
        descriptionCoroutine = DescriptionAppearTimer();

        // Outros
        Murderer = null;
        murdererActions = new List<GameAction>();
        
    }

    #region HealthMethods

    public void TakeDamage(IGameEntity source, int amount)
    {
        List<EffectActivationData> effects = cardData.GetEffectsByTime(TimeToActivate.OnTakeDamage);

        foreach (EffectActivationData effect in effects)
        {
            IGameEntity[] ige = { source };
            amount += EffectHandler.Instance.ActivateEffectImmediatly(effect.effect, this, ige, amount);
        }

        healthSystem.TakeDamage(amount);
        ChangeHealthComponent();

        if (healthSystem.CurrentHealth == 0)
        {
            EnqueueEffects(TimeToActivate.OnDeath);
            
            CardInstance ci = source as CardInstance;

            if (ci != null)
            {
                murdererActions = ci.TryEnqueueKillEffect();
            }

            Murderer = source;
        }
    }

    public void Heal(int amount)
    {
        healthSystem.Heal(amount);
        ChangeHealthComponent();

        if (healthSystem.CurrentHealth > 0)
        {
            Murderer = null;

            foreach (var ga in murdererActions)
            {
                EffectHandler.Instance.BlockEffect(TimeToActivate.OnKill, ga);
            }
        }
    }

    public int GetCurrentHealth()
    {
        return healthSystem.CurrentHealth;
    }

    private void ChangeHealthComponent()
    {
        // Altera o valor do componente
        healthComponent.text = healthSystem.CurrentHealth.ToString();

        if (healthSystem.IsDamaged())
        {
            healthComponent.color = Color.red;
        }
        else if (healthSystem.WasBuffed)
        {
            healthComponent.color = Color.green;
        }
        else
        {
            healthComponent.color = Color.white;
        }
    }
    #endregion

    public void FinishedPlayCard()
    {
        EnqueueEffects(TimeToActivate.OnReveal);
        // TO DO: Put card down
    }


    private List<GameAction> EnqueueEffects(TimeToActivate state)
    {
        List<EffectActivationData> effects = cardData.GetEffectsByTime(state);
        List<GameAction> gamesActions = new List<GameAction>();

        foreach (EffectActivationData effect in effects)
        {
            GameAction res = new GameAction(this, effect);
            EffectHandler.Instance.EnqueueEffect(effect.timeToActivate, res);
            gamesActions.Add(res);
        }        

        return gamesActions;
    }

    public List<GameAction> TryEnqueueKillEffect()
    {
        return EnqueueEffects(TimeToActivate.OnKill);
    }

    public int GetAttackDamage(IGameEntity tg)
    {
        if (turnsToSleep > 0)
        {
            turnsToSleep--;
            return 0;
        }

        List<EffectActivationData> effects = cardData.GetEffectsByTime(TimeToActivate.OnTakeDamage);
        int dmg = attackSystem.CurrentHealth;

        foreach (EffectActivationData effect in effects)
        {
            IGameEntity[] ige = { tg };
            dmg += EffectHandler.Instance.ActivateEffectImmediatly(effect.effect, this, ige, effect.specialParameter);
        }

        return dmg;
    }

    private void ChangeAttackComponent()
    {
        // Altera o valor do componente
        attackComponent.text = attackSystem.CurrentHealth.ToString();

        if (attackSystem.IsDamaged())
        {
            attackComponent.color = Color.red;
        }
        else if (attackSystem.WasBuffed)
        {
            attackComponent.color = Color.green;
        }
        else
        {
            attackComponent.color = Color.white;
        }
    }

    public bool Buff(IGameEntity source, Stat stat, int amount)
    {
        if ((stat & Stat.Health) != 0)
        {
            healthSystem.BuffMaxHealth(source, amount, true);
            return true;
        }
        if ((stat & Stat.Attack) != 0)
        {
            attackSystem.BuffMaxHealth(source, amount, true);
            return true;
        }

        return false;
    }

    public bool TryUndoBuff(IGameEntity source, out int extra)
    {
        if (healthSystem.TryUndoBuff(source, out extra) || attackSystem.TryUndoBuff(source, out extra))
        {
            ChangeHealthComponent();
            ChangeAttackComponent();
            return true;
        }

        return false;
    }

    public bool MakeInert(int amount)
    {
        turnsToSleep += amount;
        return true;
    }

    public bool TryRevive()
    {
        // TO DO: Mecanica de reviver
        throw new NotImplementedException();
    }

    void Update()
    {
        if (isBeingDragged)
        {
            transform.position = Vector3.SmoothDamp(
            transform.position,
            dragTargetPosition,
            ref velocity,
            0.02f 
        );
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        
        if (!isBeingDragged) return;
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)transform.parent,
            eventData.position,
            Camera.main,
            out worldPoint
        );

        dragTargetPosition = new Vector3(worldPoint.x, worldPoint.y, transform.position.z) + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isBeingDragged = false;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        InitializeSlots();

        GameObject closestSlot = null;
        float closestDistance = float.MaxValue;

        if(!dropped)
        {
            StartCoroutine(ReturnToHand());
        }
    }
    
    private void InitializeSlots()
    {
        if (slotObjects != null && slotObjects.Count > 0) return;

        slotObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Slot"));

    }

    public IEnumerator ReturnToHand()
    {
        float t = 0f;
        Vector3 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            transform.position = Vector3.Lerp(startPos, initialPosition, t);
            yield return null;
        }

        transform.position = initialPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Mode == CardMode.InHand)
        {
            initialPosition = transform.position;
            isBeingDragged = true;
            dropped = false;

            Vector3 worldPoint;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                (RectTransform)transform.parent,
                eventData.position,
                Camera.main,
                out worldPoint
            );
            dragOffset = transform.position - worldPoint;

            dragTargetPosition = transform.position;

            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
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
            GameAction res = new GameAction(this, effect);
            await RewindableActionsController.Instance.CardPlayed(res);
        }

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
            OnPointerExit(eventData);

            // Pega uma copia do alvo pra ficar la na carta
            targetPrefab = Instantiate(TargetSelector.Instance.Selected(this), 
                eventData.position, 
                Quaternion.identity, 
                transform);
            canBeSelected = false;
            //Debug.LogError("Selected!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // TODO : Animation for hovering
        descriptionCoroutine = DescriptionAppearTimer();
        StartCoroutine(descriptionCoroutine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // TODO: Stop animation for hovering
        StopCoroutine(descriptionCoroutine);
        descriptionImage.SetActive(false);
    }

    IEnumerator DescriptionAppearTimer()
    {
        yield return new WaitForSeconds(1f);
        descriptionImage.SetActive(true);    // TO DO: Adicionar easing
        Debug.Log("Description!");
    }
}
