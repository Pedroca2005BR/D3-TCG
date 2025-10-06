using UnityEngine;
using System.Collections.Generic;

public class JM_TurnController : MonoBehaviour
{
    [Header("Regras do Jogo")]
    public JM_RulesObject gameRules;
    public float initialTime = 0f;
    public bool lastTurn = false;
    public bool player1Played = false;
    public bool player2Played = false;

    [Header("State Machine")]
    TurnBaseState currentState;
    public StartGameState openingState = new StartGameState();
    public StartTurnState startingState = new StartTurnState();
    public Player1ActionsState p1choosingState = new Player1ActionsState();
    public Player2ActionsState p2choosingState = new Player2ActionsState();
    public RevealingCardState revealingState = new RevealingCardState();
    public ProcessingGameState processingState = new ProcessingGameState();
    public EndTurnState endingState = new EndTurnState();
    public EndGameState finishingState = new EndGameState();

    [Header("Controle do Deck")]
    public JM_DeckManager player1Deck;
    public JM_DeckManager player2Deck;
    public List<CardData> player1Hand = new List<CardData>(); 
    public List<CardData> player2Hand = new List<CardData>();

    void Start()
    {
        currentState = openingState;

        currentState.EnterState(this);

    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(TurnBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    public void initializeHands()
    {
        for (int i = 0; i < gameRules.initialHandSize; i++)
        {
            BuyCard(player1Deck, player1Hand);
            BuyCard(player2Deck, player2Hand);
        }
    }

    public void BuyCard(JM_DeckManager deck, List<CardData> hand)
    {
        if (deck.cards.Count > 0 && hand.Count < gameRules.handSize)
        {
            CardData choosenCard = deck.cards[0];
            deck.cards.RemoveAt(0);
            deck.BoughtCard(choosenCard);
            hand.Add(choosenCard);
        }
        else if (deck.cards.Count <= 0)
        {
            Debug.Log("Baralho vazio, agora eh tudo ou nada");
            lastTurn = true;
        }
    }

    public void ShuffleDeck(List<CardData> deck)
    {
        for (int i = deck.Count-1; i>0; i--)
        {
            int j = Random.Range(0, i+1);

            CardData aux = deck[i];
            deck[i] = deck[j];
            deck[j] = aux;
        }
    }

    public void OrganizeDeck()
    {
        player1Deck.cards.AddRange(player1Deck.usedCards);
        player1Deck.usedCards.Clear();
        ShuffleDeck(player1Deck.cards);
        
        player2Deck.cards.AddRange(player2Deck.usedCards);
        player2Deck.usedCards.Clear();
        ShuffleDeck(player2Deck.cards);
    }
}
