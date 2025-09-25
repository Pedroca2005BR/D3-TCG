using UnityEngine;

public class HealthSystem
{
    public int MaxHealth {  get; private set; }
    public int CurrentHealth { get; private set; }
    public bool WasBuffed { get; private set; }

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
    }

    public void BuffMaxHealth(int amount, bool heal = false)
    {
        MaxHealth += amount;
        WasBuffed = true;

        if (heal)
        {
            Heal(amount);
        }
    }

    public void SetMaxHealth(int newMaxHealth, bool heal = false)
    {
        MaxHealth = newMaxHealth;
        //WasBuffed = true;

        if (heal)
        {
            Heal(newMaxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth); 
        //Debug.Log($"Jogador levou {damage} de dano! Vida atual: {CurrentHealth}");
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth); 
        //Debug.Log($"Jogador curado! Vida atual: {CurrentHealth}");
    }

    public bool IsDamaged()
    {
        return MaxHealth != CurrentHealth;
    }
}
