using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;


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
    public bool p2Entered = false;
    public bool activeCorrotine = false;
    public bool dontAct = false;
    public int turn = 0;
    GameObject source;
    public GameObject board;
    public GameObject loadingScreen;


    [Header("State Machine")]
    public TMP_Text turnCounter;
    public TMP_Text loadingText;
    public GameObject loadingButton;
    public GameStates currentState;
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

    public void BuyCard(JM_DeckManager deck, bool isPlayer1)
    {
        if (!handManager.AddCard(deck, isPlayer1))
        {
            Debug.Log("Baralho vazio, agora eh tudo ou nada");
            lastTurn = true;
        }
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
        //if (gameRules.deck1AddressableKey != null && gameRules.deck1AddressableKey != "")
        //{
        //    // TO DO: pegar deck por string json;
        //}
        player1Deck.cards.AddRange(player1Deck.usedCards);
        player1Deck.usedCards.Clear();
        player1Deck.deadCards.Clear();
        ShuffleDeck(player1Deck.cards);

        //if (gameRules.deck1AddressableKey != null && gameRules.deck1AddressableKey != "")
        //{
        //    // TO DO: pegar deck por string json;
        //}

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
        //Adiciona a funcao de virar as cartas
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
        if (!p2Entered) p2Entered = true;

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
    
    

}
