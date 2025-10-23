using UnityEngine;

[CreateAssetMenu(fileName = "BypassEffect", menuName = "Scriptable Objects/Effects/BypassEffect")]
public class BypassEffect : EffectObject
{
    public override int Resolve(CardInstance source, IGameEntity[] targets, int specialParam, int bonusParam = 0)
    {
        if (targets == null || targets.Length == 0)    return -1;

        foreach (var target in targets)
        {
            CardInstance ci = target as CardInstance;

            if (ci != null)
            {
                if (ci.AttackTargeting == Targeting.EnemyInFront)
                {
                    ci.AttackTargeting = Targeting.EnemyHero;
                }
                else if (ci.AttackTargeting == Targeting.EnemyHero)
                {
                    ci.AttackTargeting = Targeting.EnemyInFront;
                }
                else
                {
                    Debug.LogError("Target with strange attacking pattern! " +  ci.AttackTargeting.ToString());
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
