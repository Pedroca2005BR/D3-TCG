using UnityEngine;

[CreateAssetMenu(fileName = "BypassEffect", menuName = "Scriptable Objects/Effects/BypassEffect")]
public class BypassEffect : EffectObject
{
    public override int Resolve(CardInstance source, IGameEntity[] targets, int specialParam, int bonusParam = 0)
    {
        if (targets == null || targets.Length == 0)    return -1;

        foreach (var target in targets)
        {
            //CardInstance ci = target as CardInstance;

            if (source != null)
            {
                if (source.AttackTargeting == Targeting.EnemyInFront)
                {
                    source.AttackTargeting = Targeting.EnemyHero;
                }
                else if (source.AttackTargeting == Targeting.EnemyHero)
                {
                    source.AttackTargeting = Targeting.EnemyInFront;
                }
                else
                {
                    Debug.LogError("Target with strange attacking pattern! " +  source.AttackTargeting.ToString());
                    return -1;
                }
            }
            else
            {
                Debug.LogError("Impossible target!");
                return -1;
            }
        }

        return 0;
    }
}
