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

    [Header("References")]
    [SerializeField] RectTransform canvas;

    [Header("Dead Card needs")]
    [SerializeField] GameObject deadCardsPanel;
    [SerializeField] GameObject deadCardsContent;
    [SerializeField] GameObject cardPrefab;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (targeter != null)
        {
            Vector3 pos;
            //targeter.transform.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas,
            Mouse.current.position.ReadValue(),
            Camera.main,
            out pos
            );
            targeter.transform.position = pos;

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
        //Debug.LogWarning("Selecting targets manually!");

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
        Destroy(targeter);
        lineRenderer.enabled=false;
        
        for (int i = 0; i<deadCardsContent.transform.childCount; i++)
        {
            deadCardsContent.transform.GetChild(i).gameObject.SetActive(false);
        }
        deadCardsPanel.SetActive(false);

        foreach (var t in targets)
        {
            t.SelectionOver();
        }

        return processedTargets.ToArray();
    }

    public List<CardInstance> TurnOnDeadCardsPanel(bool isPlayer1)
    {
        List<CardInstance> deadCards = new List<CardInstance>();
        List<CardData> cartas = new List<CardData>(GameManager.Instance.GetDeck(isPlayer1).deadCards.Keys);
        if (cartas.Count == 0) return null;

        deadCardsPanel.SetActive(true);

        foreach(CardData data in cartas)
        {
            GameObject go = Instantiate(cardPrefab, deadCardsContent.transform);
            CardInstance card = go.GetComponent<CardInstance>();
            card.SetupCardInstance(data, isPlayer1 );

            deadCards.Add(card);
        }

        return deadCards;
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
            //Debug.LogWarning("Source is null -> " + (source == null).ToString());
            CardSlot[] enemySlots = GameManager.Instance.GetSlots(!source.IsPlayer1);
            CardSlot[] allySlot = GameManager.Instance.GetSlots(source.IsPlayer1);

            for(int i = 0; i < allySlot.Length; i++)
            {
                // Se achar o lugar do source, testamos se tem alguem em frente
                if (allySlot[i].CardInstance != null && allySlot[i].CardInstance.Id == source.Id)   // card achado
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
                if (allySlot[i].CardInstance != null && allySlot[i].CardInstance.Id != source.Id)
                {
                    //// tenta pegar a carta da esquerda
                    //if (i-1 > 0 && allySlot[i - 1].CardInstance != null)
                    //{
                    //    processedTargets.Add(allySlot[i-1].CardInstance);
                    //}
                    //// tenta pegar a carta da direita
                    //if (i+1  < allySlot.Length && allySlot[i + 1].CardInstance != null)
                    //{
                    //    processedTargets.Add(allySlot[i+1].CardInstance);

                    //}

                    processedTargets.Add(allySlot[i].CardInstance);
                }
            }
        }
        if ((target & Targeting.AllEnemyCards) != 0)
        {
            source = source as CardInstance;
            CardSlot[] enemySlots = GameManager.Instance.GetSlots(!source.IsPlayer1);
            //CardSlot[] allySlot = GameManager.Instance.GetSlots(source.IsPlayer1);

            for (int i = 0; i < enemySlots.Length; i++)
            {
                if (enemySlots[i].CardInstance != null)
                    processedTargets.Add(enemySlots[i].CardInstance);
            }
        }
        if ((target & Targeting.DeadCards) != 0)
        {
            List<CardInstance> deadCards = Instance.TurnOnDeadCardsPanel(source.IsPlayer1);

            if (deadCards == null)  return processedTargets.ToArray();

            foreach (CardInstance deadCard in deadCards)
            {
                processedTargets.Add(deadCard);
            }
        }
        if ((target & Targeting.Murderer) != 0)
        {
            CardInstance sour = source as CardInstance;

            if (sour != null && sour.Murderer != null)
            {
                processedTargets.Add(sour.Murderer);
            }
        }

        return processedTargets.ToArray();
    }

    public static CardSlot[] GetTargetSlot(IGameEntity source, Targeting target)
    {
        List<CardSlot> processedTargets = new List<CardSlot>();

        if ((target & Targeting.AllEnemyCards) != 0)
        {
            processedTargets.AddRange(GameManager.Instance.GetSlots(!source.IsPlayer1));
        }
        if ((target & Targeting.AdjacentAllies) != 0)
        {
            CardSlot[] slots = GameManager.Instance.GetSlots(source.IsPlayer1);

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].CardInstance != null && slots[i].CardInstance.Id != source.Id)
                {
                    processedTargets.Add(slots[i]);
                }
            }
        }
        if ((target & Targeting.EnemyInFront) != 0)
        {
            source = source as CardInstance;
            //Debug.LogWarning("Source is null -> " + (source == null).ToString());
            CardSlot[] enemySlots = GameManager.Instance.GetSlots(!source.IsPlayer1);
            CardSlot[] allySlot = GameManager.Instance.GetSlots(source.IsPlayer1);

            for (int i = 0; i < allySlot.Length; i++)
            {
                // Se achar o lugar do source, testamos se tem alguem em frente
                if (allySlot[i].CardInstance != null && allySlot[i].CardInstance.Id == source.Id)   // card achado
                {
                    processedTargets.Add(enemySlots[i]);

                    break;
                }
            }
        }
        if ((target & Targeting.Self) != 0)
        {
            CardInstance ci = source as CardInstance;
            if (ci != null && ci.CurrentSlot != null && ci.CurrentSlot.TryGetComponent<CardSlot>(out CardSlot slot))
                processedTargets.Add(slot);
        }

        return processedTargets.ToArray();
    }
}
