using UnityEngine;
using System.Collections.Generic;

// A funcao do TargetSelector eh transformar uma ideia vaga de alvo em um alvo real
public static class TargetSelector
{
    public static IGameEntity[] GetTargets(IGameEntity source, Targeting target)
    {
        // Se for special, o proprio EffectObject vai cuidar de conseguir o alvo
        if (target == Targeting.Special)
        {
            return null;
        }

        List<IGameEntity> processedTargets = new List<IGameEntity>();

        if ((target & Targeting.Self) != 0)
        {
            processedTargets.Add(source);
        }
        if ((target & Targeting.OwnHero) != 0)
        {
            processedTargets.Add(GameManager.Instance.GetHero(source.IsPlayer1));
        }
        if ((target & Targeting.EnemyHero) != 0)
        {
            processedTargets.Add(GameManager.Instance.GetHero(!source.IsPlayer1));
        }
        if ((target & Targeting.EnemyInFront) != 0)
        {
            source = source as CardInstance;
            CardSlot[] enemySlots = GameManager.Instance.GetSlots(!source.IsPlayer1);
            CardSlot[] allySlot = GameManager.Instance.GetSlots(source.IsPlayer1);

            for(int i = 0; i < allySlot.Length; i++)
            {
                // Se achar o lugar do source, testamos se tem alguem em frente
                if (allySlot[i].CardInstance.Id == source.Id)   // card achado
                {
                    if (enemySlots[i].CardInstance != null)     // tem card em frente
                    {
                        processedTargets.Add(enemySlots[i].CardInstance);
                    }
                    else                                        // nao tem card em frente
                    {
                        processedTargets.Add(GameManager.Instance.GetHero(!source.IsPlayer1));
                    }

                    break;
                }
            }
        }
        if ((target & Targeting.AdjacentAllies) != 0)
        {
            CardSlot[] allySlot = GameManager.Instance.GetSlots(source.IsPlayer1);
            
            for(int i = 0;i < allySlot.Length; i++)
            {
                if (allySlot[i].CardInstance.Id == source.Id)
                {
                    // tenta pegar a carta da esquerda
                    if (i-1 > 0 && allySlot[i - 1].CardInstance != null)
                    {
                        processedTargets.Add(allySlot[i-1].CardInstance);
                    }
                    // tenta pegar a carta da direita
                    if (i+1  < allySlot.Length && allySlot[i + 1].CardInstance != null)
                    {
                        processedTargets.Add(allySlot[i+1].CardInstance);

                    }
                }
            }
        }
        if ((target & Targeting.AllEnemyCards) != 0)
        {
            source = source as CardInstance;
            CardSlot[] enemySlots = GameManager.Instance.GetSlots(!source.IsPlayer1);
            CardSlot[] allySlot = GameManager.Instance.GetSlots(source.IsPlayer1);

            for (int i = 0; i < enemySlots.Length; i++)
            {
                processedTargets.Add(enemySlots[i].CardInstance);
            }
        }

        return processedTargets.ToArray();
    }
}
