using UnityEngine;

[CreateAssetMenu(fileName = "DealDamageEffect", menuName = "Scriptable Objects/Effects/DealDamageEffect")]
public class DealDamageEffect : EffectObject
{
    //public int damage;

    public override void Resolve(CardInstance source, IGameEntity[] targets, int specialParam)
    {
        if (targets == null || targets.Length == 0)
        {
            Debug.LogError("Nothing to damage. Targets is empty or null.");
        }

        for (int i = 0; i < targets.Length; i++)
        {
            IDamageable tg = targets[i] as IDamageable;
            tg.TakeDamage(specialParam);
        }
    }
}
