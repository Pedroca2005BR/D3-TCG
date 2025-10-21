using UnityEngine;

[CreateAssetMenu(fileName = "MoveCardsEffect", menuName = "Scriptable Objects/Effects/MoveCardsEffect")]
public class MoveCardsEffect : EffectObject
{
    public override int Resolve(CardInstance source, IGameEntity[] targets, int specialParam)
    {
        if (targets == null || targets.Length == 0) return -1;

        // -------------------------------------Change positions--------------------------------------

        return 0;
    }
}
