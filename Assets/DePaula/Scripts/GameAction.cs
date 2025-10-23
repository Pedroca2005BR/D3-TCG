using System.Threading.Tasks;
using UnityEngine;

public class GameAction
{
    public EffectObject effect {  get; private set; }
    CardInstance source;
    Targeting target;
    bool isBlocked;
    public int priority;
    public int specialParam;
    public TimeToActivate toActivate;

    public GameAction(CardInstance source, EffectActivationData data)
    {
        this.source = source;
        this.target = data.targeting;
        this.effect = data.effect;
        isBlocked = false;
        priority = (int)effect.priority;
        this.specialParam = data.specialParameter;
        toActivate = data.timeToActivate;
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
    }

    public void BlockEffect(bool blockState = true)
    {
        isBlocked = blockState;
    }


    public async Task<bool> Execute()
    {
        if (isBlocked) return false;

        // Transforma o objeto desconhecido target em uma(s) game entity conhecida
        IGameEntity[] tgs;
        Debug.Log("Executing " + effect.effectName + "!");


        if (toActivate == TimeToActivate.OnPlay)
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
