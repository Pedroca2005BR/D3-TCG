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

    private Dictionary<GameStates, List<GameAction>> effectsToSolve = new Dictionary<GameStates, List<GameAction>>();

    // Finalmente executa os efeitos e depois os apaga
    public async void ResolveEffects(GameStates state)
    {
        for(int i = 0; i < effectsToSolve[state].Count; i++)
        {
            await effectsToSolve[state][i].Execute();
        }

        effectsToSolve[state].Clear();
    }

    // Adiciona na lista o comando, e entao da sort na lista
    public void EnqueueEffect(TimeToActivate time, GameAction effect)
    {
        GameStates state = ConvertTimeToActivateToGameState(time);

        effectsToSolve[state].Add(effect);

        if (state != GameManager.Instance.turnController.currentState)
        {
            SortListByPriority(effectsToSolve[state]);
        }
    }


    public void RemoveEffect(TimeToActivate time, GameAction effect)
    {
        GameStates state = ConvertTimeToActivateToGameState(time);
        effectsToSolve[state].Remove(effect);
    }

    private void SortListByPriority(List<GameAction> lt)
    {
        int n = effectsToSolve.Count;
        bool swapped;
        GameAction temp;

        for (int i = 0; i < n - 1; i++)
        {
            swapped = false;
            for (int j = 0; j < n - 1 - i; j++)
            {
                // Compare adjacent elements
                if (lt[j].priority > lt[j + 1].priority)
                {
                    // Swap them if they are in the wrong order
                    temp = lt[j];
                    lt[j] = lt[j + 1];
                    lt[j + 1] = temp;
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

    private GameStates ConvertTimeToActivateToGameState(TimeToActivate time)
    {
        if (time == TimeToActivate.OnReveal) return GameStates.revealing;
        else if (time == TimeToActivate.Passive) return GameManager.Instance.turnController.currentState;
        else if (time == TimeToActivate.OnStartOfTurn) return GameStates.startingTurn;
        else return GameStates.processing;
    }
}
