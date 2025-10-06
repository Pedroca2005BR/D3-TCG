using UnityEngine;

[CreateAssetMenu(fileName = "HealDamageEffect", menuName = "Scriptable Objects/Effects/HealDamageEffect")]
public class HealDamageEffect : EffectObject
{
    public int regeneratingPower;
    public TargetLimiterData limiter;

    public override void Resolve(CardInstance source, object target = null)
    {
        IDamageable tg = target as IDamageable;

        tg.Heal(regeneratingPower);
        //throw new System.NotImplementedException();
    }
}
