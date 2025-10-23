using UnityEngine;

[CreateAssetMenu(fileName = "AlterDamageEffect", menuName = "Scriptable Objects/Effects/AlterDamageEffect")]
public class AlterDamageEffect : EffectObject
{
    public CardType type;
    public override int Resolve(CardInstance source, IGameEntity[] targets, int specialParam, int bonusParam = 0)
    {
        // So aceita alvo unico, pois eh usado apenas nessas ocasioes
        if (targets == null || targets.Length != 1)
        {
            return 0;
        }

        // Caso: ataque alvo com certo tipo
        if (type != CardType.None)
        {
            CardInstance ci = targets[0] as CardInstance;
            if (ci != null && ci.cardData.type == type)
            {
                return specialParam;
            }
            else
            {
                return 0;
            }
        }
        else
        // Caso: diminui dano
        {
            return specialParam;
        }
    }
}
