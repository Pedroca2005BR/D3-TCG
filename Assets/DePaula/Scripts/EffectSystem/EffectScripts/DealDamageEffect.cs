using UnityEngine;

[CreateAssetMenu(fileName = "DealDamageEffect", menuName = "Scriptable Objects/Effects/DealDamageEffect")]
public class DealDamageEffect : EffectObject
{
    public int damage;
    public TargetLimiterData limiter;

    public override void Resolve(CardInstance source, object target = null)
    {
        IDamageable tg = target as IDamageable;

        tg.TakeDamage(damage);
        //throw new System.NotImplementedException();
    }
}
