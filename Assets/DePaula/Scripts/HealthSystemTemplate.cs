using UnityEngine;
using System.Collections.Generic;

public class HealthSystemTemplate
{
    public int MaxHealth {  get; private set; }
    public int CurrentHealth { get; private set; }

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

        m_Buffs = new Dictionary<IGameEntity, int>();
    }

    public void BuffMaxHealth(IGameEntity source, int amount, bool heal = false)
    {
        // buff = 10-6 = 4
        MaxHealth += amount;

        if (heal)
        {
            Heal(amount);
        }

        
        m_Buffs.Add(source, amount);
    }

    public bool TryUndoBuff(IGameEntity source, out int amount)
    {
        amount = 0;

        if (m_Buffs.ContainsKey(source))
        {
            // EX: buff = 10;  currentHealth = 5;
            MaxHealth -= m_Buffs[source];
            CurrentHealth -= m_Buffs[source];   // currentHealth = 5-10 = -5
            m_Buffs.Remove(source);


            
            amount = CurrentHealth - 1; // amount = -6

            // Do not die when debuffed to hell
            if (CurrentHealth <= 0) CurrentHealth = 1;

            return true;
        }

        

        return false;
    }

    public bool CheckBuff(out bool isGood)
    {
        if (m_Buffs.Count == 0)
        {
            isGood = false;
            return false;
        }

        int total = 0;
        foreach(IGameEntity source in m_Buffs.Keys)
        {
            total += m_Buffs[source];
        }

        if (total < 0)  isGood = false;
        else isGood = true;

        return true;
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
