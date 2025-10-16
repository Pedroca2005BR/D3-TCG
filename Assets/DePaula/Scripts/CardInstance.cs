using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class CardInstance : MonoBehaviour, IGameEntity
{
    public enum Mode
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
    [SerializeField] TextMeshProUGUI descriptionComponent;
    [SerializeField] TextMeshProUGUI healthComponent;
    [SerializeField] TextMeshProUGUI attackComponent;
    [SerializeField] Image cardArtComponent;
    [SerializeField] Image backgroundComponent;


    HealthSystemTemplate healthSystem, attackSystem;
    Mode mode;
    int turnsToSleep = 0;

    // ------------------------------------------------------------------------------------------GameEntity Stuff
    public bool IsPlayer1 => isPlayer1;
    public string Id => id;
    string id;
    bool isPlayer1;
    // ------------------------------------------------------------------------------------------GameEntity Stuff


    public void SetupCardInstance(CardData data, bool isPlayer1)
    {
        // Coloca a informacao da carta na instancia
        cardData = data;

        // Prepara componentes nao visuais
        healthSystem = new HealthSystemTemplate(cardData.health);
        attackSystem = new HealthSystemTemplate(cardData.attack);
        mode = Mode.InHand;
        this.isPlayer1 = isPlayer1;
        id = Guid.NewGuid().ToString();

        // Prepara textos
        nameComponent.text = cardData.cardName;
        descriptionComponent.text = cardData.cardDescription;
        ChangeHealthComponent();
        ChangeAttackComponent();

        // Prepara artes
        cardArtComponent.sprite = cardData.cardArt;
        backgroundComponent.sprite = cardData.backgroundArt;
    }

    #region HealthMethods

    public void TakeDamage(int amount)
    {
        healthSystem.TakeDamage(amount);
        ChangeHealthComponent();
    }

    public void Heal(int amount)
    {
        healthSystem.Heal(amount);
        ChangeHealthComponent();
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

    public void PlayCard()
    {
        EnqueueEffects(TimeToActivate.OnReveal);
        // TO DO: Put card down
    }
    public void Attack()
    {
        EnqueueEffects(TimeToActivate.OnAttack);
        // TO DO: Attack stuff
    }

    private void EnqueueEffects(TimeToActivate state)
    {
        List<EffectActivationData> effects = cardData.GetEffectsByTime(state);

        foreach (EffectActivationData effect in effects)
        {
            GameAction res = new GameAction(this, effect);
            EffectHandler.Instance.EnqueueEffect(res);
        }

        
    }

    public int GetCurrentAttack()
    {
        return attackSystem.CurrentHealth;
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

    public bool TryUndoBuff(IGameEntity source)
    {
        if (healthSystem.TryUndoBuff(source) || attackSystem.TryUndoBuff(source))
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
}
