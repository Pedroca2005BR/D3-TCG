using UnityEngine;

[CreateAssetMenu(fileName = "MakeInertEffect", menuName = "Scriptable Objects/Effects/MakeInertEffect")]
public class MakeInertEffect : EffectObject
{
    public override int Resolve(CardInstance source, IGameEntity[] targets, int specialParam)
    {
        if (targets != null)
        {
            foreach (var target in targets)
            {
                target.MakeInert(specialParam);
            }
        }

        return -1;
    }
}
