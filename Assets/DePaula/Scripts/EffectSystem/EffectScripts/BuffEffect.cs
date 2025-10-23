using UnityEngine;

[CreateAssetMenu(fileName = "BuffEffect", menuName = "Scriptable Objects/Effects/BuffEffect")]
public class BuffEffect : EffectObject
{
    public Stat statToBuff;
    public bool repeatable;

    public override int Resolve(CardInstance source, IGameEntity[] targets, int specialParam, int bonusParam = 0)
    {
        if (statToBuff == Stat.Nothing)
        {
            return -1;
        }

        foreach (var target in targets)
        {
            int e=0;
            if (!repeatable)
            {
                target.TryUndoBuff(source, out e);
            }

            if (e > 0) e = 0;

            target.Buff(source, statToBuff, specialParam+e);
        }

        return 0;
    }
}
