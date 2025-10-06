using UnityEngine;

public abstract class EffectObject : ScriptableObject
{
    public PriorityToResolve priority;

    public abstract void Resolve(CardInstance source, object target = null);

    // O que for de prioridade mais baixa eh resolvido primeiro
    public enum PriorityToResolve
    {
        Damage = 0,
        Heal = 1,
        Destroy = 2
    }
}
