using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IGameEntity : IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string Id { get; }
    public bool IsPlayer1 { get;}
    public GameObject GameObject {  get; }

    public bool Buff(IGameEntity source, Stat stat, int amount);
    public bool TryUndoBuff(IGameEntity source);
    public bool MakeInert(int amount);

    // IDamageable
    public void TakeDamage(IGameEntity source, int amount);
    public void Heal(int amount);
    public int GetCurrentHealth();
    public bool TryRevive();

    // Attack Stuff
    public int GetAttackDamage(IGameEntity tg);

    // Target Selector stuff

    public void PossibleTargetToClick();
    public void SelectionOver();
}
