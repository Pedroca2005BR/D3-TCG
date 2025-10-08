using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    #region Singleton
    public static EffectHandler Instance { get; private set; }
    // Singleton pattern
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private List<ResolveEffectCommand> effectsToSolve = new List<ResolveEffectCommand>();

    // Finalmente executa os efeitos e depois os apaga
    public void ResolveEffects()
    {
        for(int i = 0; i < effectsToSolve.Count; i++)
        {
            effectsToSolve[i].Execute();
        }

        effectsToSolve.Clear();
    }

    // Adiciona na lista o comando, e entao da sort na lista
    public void EnqueueEffect(ResolveEffectCommand effect)
    {
        effectsToSolve.Add(effect);
        SortListByPriority();
    }


    public void RemoveEffect(ResolveEffectCommand effect)
    {
        effectsToSolve.Remove(effect);
    }

    private void SortListByPriority()
    {
        int n = effectsToSolve.Count;
        bool swapped;
        ResolveEffectCommand temp;

        for (int i = 0; i < n - 1; i++)
        {
            swapped = false;
            for (int j = 0; j < n - 1 - i; j++)
            {
                // Compare adjacent elements
                if (effectsToSolve[j].priority > effectsToSolve[j + 1].priority)
                {
                    // Swap them if they are in the wrong order
                    temp = effectsToSolve[j];
                    effectsToSolve[j] = effectsToSolve[j + 1];
                    effectsToSolve[j + 1] = temp;
                    swapped = true;
                }
            }

            // If no two elements were swapped in the inner loop,
            // then the list is sorted, and we can break early.
            if (!swapped)
            {
                break;
            }
        }
    }
}

// Struct de comando usada para controlar os parametros para a resolucao de efeitos no momento que quisermos
public struct ResolveEffectCommand
{
    EffectObject effect;
    CardInstance source;
    Targeting target;
    bool isBlocked;
    public int priority;
    public int specialParam;

    public ResolveEffectCommand(CardInstance source, EffectActivationData data)
    {
        this.source = source;
        this.target = data.targeting;
        this.effect = data.effect;
        isBlocked = false;
        priority = (int)effect.priority;
        this.specialParam = data.specialParameter;
    }
    public ResolveEffectCommand(ResolveEffectCommand other)
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