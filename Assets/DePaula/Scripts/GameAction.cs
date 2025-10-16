using UnityEngine;

public class GameAction
{
    EffectObject effect;
    CardInstance source;
    Targeting target;
    bool isBlocked;
    public int priority;
    public int specialParam;

    public GameAction(CardInstance source, EffectActivationData data)
    {
        this.source = source;
        this.target = data.targeting;
        this.effect = data.effect;
        isBlocked = false;
        priority = (int)effect.priority;
        this.specialParam = data.specialParameter;
    }
    public GameAction(GameAction other)
    {
        effect = other.effect;
        source = other.source;
        target = other.target;
        isBlocked = other.isBlocked;
        priority = other.priority;
        specialParam = other.specialParam;
    }

    public void BlockEffect(bool blockState = true)
    {
        isBlocked = blockState;
    }


    public bool Execute()
    {
        if (isBlocked) return false;

        // Transforma o objeto desconhecido target em uma(s) game entity conhecida
        IGameEntity[] tgs = TargetSelector.GetTargets(source, target);

        effect.Resolve(source, tgs, specialParam);

        return true;
    }
}
