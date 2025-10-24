using UnityEngine;

[CreateAssetMenu(fileName = "ReviveEffect", menuName = "Scriptable Objects/Effects/ReviveEffect")]
public class ReviveEffect : EffectObject
{
    public bool directToBattlefield;
    public override int Resolve(CardInstance source, IGameEntity[] targets, int specialParam, int bonusParam = 0)
    {
        if (targets != null)
        {
            foreach (var target in targets)
            {
                if(target.TryRevive())
                {
                    CardInstance ci = target as CardInstance;

                    // TO DO: Reviver no battlefield ou na mao
                    if (directToBattlefield)
                    {
                        if (ci != null)
                        {
                            if (!ci.Revive(GameManager.Instance.GetDeck(target.IsPlayer1).deadCards[ci.cardData], specialParam))
                            {
                                Debug.LogError("Can't complete revival process! CardSlot is not empty!");
                                return -1;
                            }
                        }
                    }
                    else
                    {
                        // TO DO: Revive na mao
                        if (ci != null)
                        {
                            GameManager.Instance.HandManager.AddZombieCard(ci.cardData, ci.IsPlayer1);
                        }
                    }
                }
            }
        }

        return 0;
    }
}
