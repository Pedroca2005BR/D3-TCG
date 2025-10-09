using UnityEngine;
using System.Collections.Generic;

public class HealthSystemTemplate
{
    public int MaxHealth {  get; private set; }
    public int CurrentHealth { get; private set; }
    public bool WasBuffed { get; private set; }

    private Dictionary<IGameEntity, int> m_Buffs;

    public HealthSystemTemplate(int maxHealth, int currentHealth = -1)
    {
        Setup(maxHealth, currentHealth);
    }

    public void Setup(int maxHealth, int currentHealth = -1)
    {
        MaxHealth = maxHealth;

        if (currentHealth < 0)
        {
            CurrentHealth = maxHealth;
        }
        else
        {
            CurrentHealth = currentHealth;
        }

        WasBuffed = false;
        m_Buffs = new Dictionary<IGameEntity, int>();
    }

    public void BuffMaxHealth(IGameEntity source, int amount, bool heal = false)
    {
        MaxHealth += amount;
        WasBuffed = true;

        if (heal)
        {
            Heal(amount);
        }

        m_Buffs.Add(source, amount);
    }

    public bool TryUndoBuff(IGameEntity source)
    {
        if (m_Buffs.ContainsKey(source))
        {
            MaxHealth -= m_Buffs[source];
            CurrentHealth -= m_Buffs[source];
            m_Buffs.Remove(source);

            if (m_Buffs.Count == 0)
            {
                WasBuffed = false;
            }


            // Do not die when debuffed to hell
            if (CurrentHealth < 0) CurrentHealth = 1;

            return true;
        }

        return false;
    }

    public void SetMaxHealth(int newMaxHealth, bool heal = false)
    {
        MaxHealth = newMaxHealth;

        if (heal)
        {
            Heal(newMaxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth); 
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth); 
    }

    public bool IsDamaged()
    {
        return MaxHealth != CurrentHealth;
    }
}
