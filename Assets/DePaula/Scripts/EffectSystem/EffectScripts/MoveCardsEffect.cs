using UnityEngine;

[CreateAssetMenu(fileName = "MoveCardsEffect", menuName = "Scriptable Objects/Effects/MoveCardsEffect")]
public class MoveCardsEffect : EffectObject
{
    public override int Resolve(CardInstance source, IGameEntity[] targets, int specialParam, int bonusParam = 0)
    {
        if (targets == null || targets.Length == 0) return -1;

        Debug.Log("Changing Positions beetween " + targets.Length);

        // -------------------------------------Change positions--------------------------------------
        CardInstance ci1 = targets[0] as CardInstance;
        CardInstance ci2 = targets[1] as CardInstance;

        if (ci2 != null && ci2.CurrentSlot != null && ci2.CurrentSlot.TryGetComponent<CardSlot>(out CardSlot cs2))
        {
            if (ci1 != null && ci1.CurrentSlot != null && ci1.CurrentSlot.TryGetComponent<CardSlot>(out CardSlot cs1))
            {
                cs2.PutCardInSlot(ci1);
                cs1.PutCardInSlot(ci2);
                return 0;
            }
            else
            {
                Debug.LogError("Card Instance or CardSlot not found! Can't move target!");
                return -1;
            }
        }
        else
        {
            Debug.LogError("Card Instance or CardSlot not found! Can't move target!");
            return -1;
        }
    }
}
