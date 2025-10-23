using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hero : MonoBehaviour, IGameEntity
{
    [Header("Visuals")]
    [SerializeField] TextMeshProUGUI healthComponent;
    [SerializeField] Image artComponent;

    // ----------------------------------------------------------------IGameEntity stuff
    public string Id => id;
    public bool IsPlayer1 => isPlayer1;

    public GameObject GameObject => gameObject;

    string id;
    [SerializeField] bool isPlayer1;    // deixar pra settar no editor msm
    // ----------------------------------------------------------------IGameEntity stuff


    HealthSystemTemplate healthSystem;

    // Target Selector stuff
    GameObject targetPrefab;
    bool canBeSelected;

    public void Setup()
    {
        healthSystem = new HealthSystemTemplate(GameManager.Instance.rules.heroHealth);
        ChangeHealthComponent();

        id = Guid.NewGuid().ToString();
    }
    

    #region HealthMethods

    public void TakeDamage(IGameEntity source, int amount)
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


    public bool Buff(IGameEntity source, Stat stat, int amount)
    {
        if ((stat & Stat.Health) != 0)
        {
            healthSystem.BuffMaxHealth(source, amount, true);
            return true;
        }

        return false;
    }

    public bool TryUndoBuff(IGameEntity source, out int extra)
    {
        if (healthSystem.TryUndoBuff(source, out extra))
        {
            ChangeHealthComponent();
            return true;
        }
        return false;
    }

    public bool MakeInert(int amount)
    {
        Debug.LogWarning("Heroes can't become Inert!");
        return false;
    }

    public int GetAttackDamage(IGameEntity tg)
    {
        Debug.LogWarning("Heroes can't attack!");
        return 0;
    }

    public bool TryRevive()
    {
        Debug.LogWarning("Heroes can't be revived!");
        return false;
    }

    public void PossibleTargetToClick()
    {
        canBeSelected = true;
    }

    public void SelectionOver()
    {
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
            targetPrefab = Instantiate(TargetSelector.Instance.Selected(this), Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y)), Quaternion.identity, transform);
            canBeSelected = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}
