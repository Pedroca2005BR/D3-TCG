using UnityEngine;

public abstract class EffectObject : ScriptableObject
{

    public string id; // GUID persistente (preencher no editor)
    public string addressableKey; // opcional: key se usar Addressables

    public PriorityToResolve priority;

    public abstract int Resolve(CardInstance source, IGameEntity[] targets, int specialParam);

    // O que for de prioridade mais baixa eh resolvido primeiro
    public enum PriorityToResolve
    {
        Damage = 0,
        Heal = 1,
        Destroy = 2
    }
}
