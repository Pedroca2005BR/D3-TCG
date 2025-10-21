using UnityEngine;

[CreateAssetMenu(fileName = "NextCardEffect", menuName = "Scriptable Objects/Effects/NextCardEffect")]
public class NextCardEffect : EffectObject
{
    public EffectObject secondaryEffect;

    public override int Resolve(CardInstance source, IGameEntity[] targets, int specialParam)
    {
        if (targets==null)
        {
            // TO DO: Ver com Leal
        }



        return 0;
    }
}
