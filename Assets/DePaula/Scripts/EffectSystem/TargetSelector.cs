using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

// A funcao do TargetSelector eh transformar uma ideia vaga de alvo em um alvo real
[RequireComponent(typeof(LineRenderer))]
public class TargetSelector : MonoBehaviour
{
    #region Singleton
    public static TargetSelector Instance { get; private set; }
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

    [Header("Visual")]
    [SerializeField] GameObject targetSelectorPrefab;
    GameObject targeter;
    int targetsSelected;
    List<IGameEntity> processedTargets;
    GameObject source;
    LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (targeter != null)
        {
            targeter.transform.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Line renderer stuff
            lineRenderer.SetPosition(1, source.transform.position);
            lineRenderer.SetPosition(0, targeter.transform.position);
        }
    }

    public GameObject Selected(IGameEntity sel)
    {
        processedTargets.Add(sel);
        return targeter;
    }

    public async Task<IGameEntity[]> SelectTargetsManually(IGameEntity source, Targeting target, int amount)
    {
        processedTargets = new List<IGameEntity>();
        IGameEntity[] targets = GetTargets(source, target);

        if (targets == null || targets.Length < amount)
        {
            Debug.LogError("Not enough target candidates!");
            return null;
        }

        // liga visuais
        this.source = source.GameObject;
        targeter = Instantiate(targetSelectorPrefab, transform.position, Quaternion.identity, transform);
        lineRenderer.enabled = true;

        foreach (var t in targets)
        {
            t.PossibleTargetToClick();
        }

        while(processedTargets.Count < amount)
        {
            await Task.Yield();
        }

        // Desliga visuais
        Destroy(targetSelectorPrefab);
        lineRenderer.enabled=false;

        return processedTargets.ToArray();
    }

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
        if ((target & Targeting.DeadCards) != 0)
        {
            // --------------------------------------------------------------------- TO DO --------------------------------
        }

        return processedTargets.ToArray();
    }
}
