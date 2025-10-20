using UnityEngine;

[CreateAssetMenu(fileName = "AlternatingEffect", menuName = "Scriptable Objects/Effects/AlternatingEffect")]
public class AlternatingEffect : EffectObject
{
    public EffectActivationData evenTurnEffect;
    public EffectActivationData oddTurnEffect;

    public override void Resolve(CardInstance source, IGameEntity[] targets, int specialParam)
    {
        GameAction res;
        TimeToActivate tm;
        if (GameManager.Instance.turnController.turn % 2 == 0)
        {
            res = new GameAction(source, evenTurnEffect);
            tm = evenTurnEffect.timeToActivate;
        }
        else
        {
            res = new GameAction(source, oddTurnEffect);
            tm = oddTurnEffect.timeToActivate;

        }

        EffectHandler.Instance.EnqueueEffect(tm, res);
    }
}
