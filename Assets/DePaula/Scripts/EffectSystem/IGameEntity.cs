using UnityEngine;

public interface IGameEntity
{
    public string Id { get; }
    public bool IsPlayer1 { get;}

    public bool Buff(IGameEntity source, Stat stat, int amount);
    public bool TryUndoBuff(IGameEntity source);
    public bool MakeInert(int amount);

    // IDamageable
    public void TakeDamage(int amount);
    public void Heal(int amount);
    public int GetCurrentHealth();
    public bool TryRevive();

    // Attack Stuff
    public int GetCurrentAttack();
}
