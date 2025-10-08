using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour, IGameEntity, IDamageable
{
    [Header("Visuals")]
    [SerializeField] TextMeshProUGUI healthComponent;
    [SerializeField] Image artComponent;

    // ----------------------------------------------------------------IGameEntity stuff
    public string Id => id;
    public bool IsPlayer1 => isPlayer1;
    string id;
    [SerializeField] bool isPlayer1;    // deixar pra settar no editor msm
    // ----------------------------------------------------------------IGameEntity stuff


    HealthSystem healthSystem;

    public void Setup()
    {
        healthSystem = new HealthSystem(GameManager.Instance.rules.heroHealth);
        ChangeHealthComponent();

        id = Guid.NewGuid().ToString();
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
}
