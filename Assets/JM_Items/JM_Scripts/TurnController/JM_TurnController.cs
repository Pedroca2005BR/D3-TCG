using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.GPUSort;


public enum GameStates
{
    opening,
    startingTurn,
    p1Choosing,
    p2Choosing,
    revealing,
    processing,
    endingTurn,
    finishingGame
}

public class JM_TurnController : MonoBehaviour
{
    [Header("Regras do Jogo")]
    public JM_RulesObject gameRules;
    public float initialTime = 0f;
    public bool iaGame = false;
    public bool lastTurn = false;
    public bool player1Played = false;
    public bool player2Played = false;
    public bool p1Entered = false;
    public bool p2Entered = false;
    public bool activeCorrotine = false;
    public bool dontAct = false;
    public int winner = 0;
    public int turn = 0;
    GameObject source;
    public GameObject board;
    public GameObject loadingScreen;
    public Hero hero1;
    public Hero hero2;


    [Header("State Machine")]
    public TMP_Text turnCounter;
    public TMP_Text loadingText;
    public GameObject loadingButton;
    public GameStates currentState;
    public GameObject endingScreen;
    public TMP_Text endingText;
    private Dictionary<GameStates, TurnBaseState> stateMap = new Dictionary<GameStates, TurnBaseState>();

    [Header("Controle do Deck")]
    public JM_DeckManager player1Deck;
    public JM_DeckManager player2Deck;
    public JM_HandUI p1Hand;
    public JM_HandUI p2Hand;
    public int player1DeckCount;
    public int player2DeckCount;
    public TMP_Text player1DeckText;
    public TMP_Text player2DeckText;
    [SerializeField] public JM_HandManager handManager;

    void Start()
    {
        InitializeStates();

        SwitchState(GameStates.opening);
    }

    void Update()
    {
        TurnBaseState currentStateObject = stateMap[currentState];

        currentStateObject.UpdateState(this);
    }

    public void SwitchState(GameStates newState)
    {
        stateMap[currentState].ExitState(this);

        currentState = newState;

        stateMap[newState].EnterState(this);
    }

    private void InitializeStates()
    {
        stateMap.Clear();

        stateMap[GameStates.opening] = new StartGameState();
        stateMap[GameStates.startingTurn] = new StartTurnState();
        stateMap[GameStates.p1Choosing] = new Player1ActionsState();
        stateMap[GameStates.p2Choosing] = new Player2ActionsState();
        stateMap[GameStates.revealing] = new RevealingCardState();
        stateMap[GameStates.processing] = new ProcessingGameState();
        stateMap[GameStates.endingTurn] = new EndTurnState();
        stateMap[GameStates.finishingGame] = new EndGameState();
    }

    public void InitializeHands()
    {
        StartCoroutine(InitializeHandsCor());
    }

    public IEnumerator InitializeHandsCor()
    {
        for (int i = 0; i < gameRules.initialHandSize; i++)
        {
            BuyCard(player1Deck, true);
            BuyCard(player2Deck, false);

            while(handManager.activeCoroutine > 0)
            {
                yield return null;
            }
        }

        SwitchState(GameStates.startingTurn);
    }

    public bool BuyCard(JM_DeckManager deck, bool isPlayer1)
    {
        return handManager.AddCard(deck, isPlayer1);
    }

    public void ShuffleDeck(List<CardData> deck)
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            CardData aux = deck[i];
            deck[i] = deck[j];
            deck[j] = aux;
        }
    }

    public void OrganizeDeck()
    {
        
        player1Deck.cards.AddRange(player1Deck.usedCards);
        player1Deck.usedCards.Clear();
        player1Deck.deadCards.Clear();
        ShuffleDeck(player1Deck.cards);


        player2Deck.cards.AddRange(player2Deck.usedCards);
        player2Deck.usedCards.Clear();
        player2Deck.deadCards.Clear();
        ShuffleDeck(player2Deck.cards);
    }

    public void EndTurn()
    {
        if (player1Played == false) player1Played = true;
        else if (player2Played == false) player2Played = true;
    }

    public void Reveal()
    {
        
        
        StartCoroutine(CardRevealing());
    }

    IEnumerator CardRevealing()
    {
        yield return ResolveEffectBus(GameStates.revealing);

        CardInstance[] cards = GameManager.Instance.GetAllCards();

        // Process kills
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].GetCurrentHealth() == 0)
            {
                CardInstance killer = cards[i].Die() as CardInstance;
                if (killer != null)
                {
                    killer.BecomeAKiller();
                }
                else
                {
                    Debug.LogError("Como tu morreu de morte morrida, querida??");
                }
            }
        }

        cards = GameManager.Instance.GetAllCards();

        // Process kills again
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].GetCurrentHealth() == 0)
            {
                CardInstance killer = cards[i].Die() as CardInstance;
                if (killer != null)
                {
                    killer.BecomeAKiller();
                }
                else
                {
                    Debug.LogError("Como tu morreu de morte morrida, querida??");
                }
            }
        }

        yield return new WaitForSeconds(1f);
        SwitchState(GameStates.processing);
    }

    public async Task ResolveEffectBus(GameStates state)
    {
        await EffectHandler.Instance.ResolveEffects(state);
    }

    public void UpdateDeckText()
    {
        player1DeckText.text = player1DeckCount.ToString();
        player2DeckText.text = player2DeckCount.ToString();
        turnCounter.text = turn.ToString();
        if (currentState == GameStates.p1Choosing) loadingText.text = "Aguardando segundo jogador";
        else if (currentState == GameStates.startingTurn) loadingText.text = "Aguardando primeiro jogador";
        else loadingText.text = "Organizando o tabuleiro";
    }

    public IEnumerator FlipBoard(GameStates changeState)
    {
        activeCorrotine = true;

        UpdateDeckText();

        loadingScreen.SetActive(true);

        StartCoroutine(FlipNewCard());

        if (changeState == GameStates.p2Choosing) board.transform.rotation = Quaternion.Euler(0, 0, 180);
        else board.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (p2Entered)
        {
            loadingButton.SetActive(false);
            yield return new WaitForSeconds(2f);
        }

        else
        {
            loadingButton.SetActive(true);
            while (!p2Entered)
                yield return null;
        }
        

        loadingScreen.SetActive(false);

        SwitchState(changeState);

        activeCorrotine = false;
    }

    public void Player2Entered()
    {
        if (!p1Entered) p1Entered = true;
        else if (!p2Entered) p2Entered = true;

    }

    public IEnumerator FlipNewCard()
    {
        CardSlot[] allySlot = GameManager.Instance.GetSlots(true);
        Debug.Log($"Comprimento = {allySlot.Length}");

        for (int i = 0; i < allySlot.Length; i++)
        {
            if (allySlot[i].CardInstance != null && allySlot[i].CardInstance.newCard)   // card achado
            {
                Debug.Log($"Chegou aqui i = {i}");
                allySlot[i].CardInstance.frontSide.SetActive(false);
                allySlot[i].CardInstance.backSide.SetActive(true);

            }
            else if (allySlot[i].CardInstance != null && !allySlot[i].CardInstance.newCard)
            {
                Debug.Log($"Veio aqui i = {i}");
                allySlot[i].CardInstance.frontSide.SetActive(true);
                allySlot[i].CardInstance.backSide.SetActive(false);
            }

            if (allySlot[i].CardInstance != null) allySlot[i].CardInstance.newCard = false;

        }

        yield break;
    }

    public void FinishGame()
    {
        if (winner == 1) endingText.text = "Vitória do Jogador 1!\nParabéns!";
        else if (winner == 2) endingText.text = "Vitória do Jogador 2!\nParabéns!";
        else
        {
            if (hero1.GetCurrentHealth() > hero2.GetCurrentHealth()) endingText.text = "Vitória do Jogador 1!\nParabéns!";
            else if (hero1.GetCurrentHealth() < hero2.GetCurrentHealth()) endingText.text = "Vitória do Jogador 2!\nParabéns!";
            else
            {
                if (handManager.player1Hand.Count > handManager.player2Hand.Count) endingText.text = "Vitória do Jogador 1!\nParabéns!";
                else if (handManager.player1Hand.Count < handManager.player2Hand.Count) endingText.text = "Vitória do Jogador 2!\nParabéns!";
                else endingText.text = "Não houve um vencedor!\nComo vocês conseguiram isso?";
            }
        }

        endingScreen.SetActive(true);

    }    

    public void ReplayGame()
    {
        SceneManager.LoadScene("Pre-Game");
    }
    

}
