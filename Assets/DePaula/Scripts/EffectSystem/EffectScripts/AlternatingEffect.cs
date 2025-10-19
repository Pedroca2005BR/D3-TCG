using UnityEngine;

[CreateAssetMenu(fileName = "AlternatingEffect", menuName = "Scriptable Objects/Effects/AlternatingEffect")]
public class AlternatingEffect : EffectObject
{
    public EffectActivationData evenTurnEffect;
    public EffectActivationData oddTurnEffect;

    public override void Resolve(CardInstance source, IGameEntity[] targets, int specialParam)
    {
        GameAction res;
        if (GameManager.Instance.turnController.turn % 2 == 0)
        {
            res = new GameAction(source, evenTurnEffect);
        }
        else
        {
            res = new GameAction(source, oddTurnEffect);
        }

        EffectHandler.Instance.EnqueueEffect(res);
    }
}
