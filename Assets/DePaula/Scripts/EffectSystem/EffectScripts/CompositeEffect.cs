using UnityEngine;

[CreateAssetMenu(fileName = "NextCardEffect", menuName = "Scriptable Objects/Effects/CompositeEffect")]
public class CompositeEffect : EffectObject
{
    public EffectActivationData secondaryEffect;

    public override void Resolve(CardInstance source, IGameEntity[] targets, int specialParam)
    {
        if (targets == null)
        {
            // TO DO: Ver com Leal
        }
        else
        {
            GameAction res = new GameAction(source, secondaryEffect);
            EffectHandler.Instance.EnqueueEffect(res);
        }
    }
}
