using UnityEngine;

[CreateAssetMenu(fileName = "BuffEffect", menuName = "Scriptable Objects/Effects/BuffEffect")]
public class BuffEffect : EffectObject
{
    public Stat statToBuff;
    public override void Resolve(CardInstance source, IGameEntity[] targets, int specialParam)
    {
        if (statToBuff == Stat.Nothing)
        {
            return;
        }

        foreach (var target in targets)
        {
            target.Buff(source, statToBuff, specialParam);
        }
    }
}
