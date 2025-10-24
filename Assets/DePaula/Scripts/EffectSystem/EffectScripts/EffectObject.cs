using UnityEngine;

public abstract class EffectObject : ScriptableObject
{
    public string effectName;
    public string id; // GUID persistente (preencher no editor)
    public string addressableKey; // opcional: key se usar Addressables

    public PriorityToResolve priority;

    public abstract int Resolve(CardInstance source, IGameEntity[] targets, int specialParam, int bonusParam = 0);
    public int Resolve(CardInstance source, CardSlot[] targets, int specialParam, float chance, int bonusParam = 0)
    {
        if (targets == null || targets.Length == 0) { return -1; }

        foreach (var target in targets)
        {
            SlotSavedAction ssa = new SlotSavedAction(source, target, this, specialParam, bonusParam, chance);
            
            target.Actions.Add(ssa);
            target.TryActivateEffect();
        }


        return 0;
    }

    // O que for de prioridade mais baixa eh resolvido primeiro
    public enum PriorityToResolve
    {
        Damage = 0,
        Heal = 1,
        Destroy = 2
    }
}
