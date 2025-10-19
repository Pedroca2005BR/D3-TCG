using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
    public int turn;

    [Header("State Machine")]
    public GameStates currentState;
    private Dictionary<GameStates, TurnBaseState> stateMap = new Dictionary<GameStates, TurnBaseState>();

    [Header("Controle do Deck")]
    public JM_DeckManager player1Deck;
    public JM_DeckManager player2Deck;
    [SerializeField] JM_HandManager handManager;

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

            yield return new WaitForSeconds(2f);
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
        StopAllCoroutines(); 
        
        StartCoroutine(CardRevealing());
    }

    IEnumerator CardRevealing()
    {
        yield return new WaitForSeconds(2f);
        //Adiciona a funcao de virar as cartas
        SwitchState(GameStates.processing);
    }
}
