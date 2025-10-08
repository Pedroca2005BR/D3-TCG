using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int amount);
    public void Heal(int amount);

    public int GetCurrentHealth();
}
