using UnityEngine;

[CreateAssetMenu(fileName = "ReviveEffect", menuName = "Scriptable Objects/Effects/ReviveEffect")]
public class ReviveEffect : EffectObject
{
    public bool directToBattlefield;
    public override int Resolve(CardInstance source, IGameEntity[] targets, int specialParam)
    {
        if (targets != null)
        {
            foreach (var target in targets)
            {
                if(target.TryRevive())
                {
                    // TO DO: Reviver no battlefield ou na mao
                }
            }
        }

        return -1;
    }
}
