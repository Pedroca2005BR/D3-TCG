using System.Threading.Tasks;
using UnityEngine;

public class GameAction
{
    public EffectObject effect {  get; private set; }
    public CardInstance source;
    public Targeting target;
    bool isBlocked;
    public int priority;
    public int specialParam;
    public TimeToActivate toActivate;
    public bool isSlotEffect;
    public float chance;

    public GameAction(CardInstance source, EffectActivationData data)
    {
        this.source = source;
        this.target = data.targeting;
        this.effect = data.effect;
        isBlocked = false;
        priority = (int)effect.priority;
        this.specialParam = data.specialParameter;
        toActivate = data.timeToActivate;
        isSlotEffect = data.isSlotEffect;
        chance = data.chance;
    }
    public GameAction(GameAction other)
    {
        effect = other.effect;
        source = other.source;
        target = other.target;
        isBlocked = other.isBlocked;
        priority = other.priority;
        specialParam = other.specialParam;
        toActivate = other.toActivate;
        isSlotEffect = other.isSlotEffect;
        chance = other.chance;
    }

    public void BlockEffect(bool blockState = true)
    {
        isBlocked = blockState;
    }


    public virtual async Task<bool> Execute()
    {
        if (isBlocked) return false;

        // Transforma o objeto desconhecido target em uma(s) game entity conhecida
        IGameEntity[] tgs;
        //Debug.LogWarning($"Executing {effect.effectName} from {source.cardData.name} in {target.ToString()}!");

        if (isSlotEffect)
        {
            effect.Resolve(source, TargetSelector.GetTargetSlot(source, target), specialParam, chance);
            return true;
        }
        else if (toActivate == TimeToActivate.OnPlay)
        {
            tgs = await TargetSelector.Instance.SelectTargetsManually(source, target, specialParam);
        }
        else
        {
            tgs = TargetSelector.GetTargets(source, target);
        }

        

        effect.Resolve(source, tgs, specialParam);

        return true;
    }
}
