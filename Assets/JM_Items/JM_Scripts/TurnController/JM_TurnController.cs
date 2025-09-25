using UnityEngine;
using System.Collections.Generic;

public class JM_TurnController : MonoBehaviour
{
    [Header("Regras do Jogo")]
    public JM_RulesObject gameRules;
    public float initialTime = 0f;
    public bool lastTurn = false;

    [Header("State Machine")]
    TurnBaseState currentState;
    public StartGameState openingState = new StartGameState();
    public StartTurnState startingState = new StartTurnState();
    public ChooseActionsState choosingState = new ChooseActionsState();
    public RevealingCardState revealingState = new RevealingCardState();
    public ProcessingGameState processingState = new ProcessingGameState();
    public EndTurnState endingState = new EndTurnState();
    public EndGameState finishingState = new EndGameState();

    [Header("Controle do Deck")]
    public JM_DeckBase playerDeck;
    public List<CardData> playerHand = new List<CardData>();

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

    public void BuyCard(int amount){
        for(int i = 0; i<amount; i++)
        {
            if(playerDeck.cards.Count <= 0) {
                Debug.Log("Baralho acabou, tudo ou nada");
                lastTurn = true;
                break;
            }
            
            if(playerHand.Count >= gameRules.handSize){
                Debug.Log("Mao cheia");
                break;
            }

            CardData choosenCard = playerDeck.cards[0];
            playerDeck.cards.RemoveAt(0);
            playerDeck.BoughtCard(choosenCard);
            playerHand.Add(choosenCard);

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

    public void OrganizeDeck(){
        playerDeck.cards.AddRange(playerDeck.usedCards);
        playerDeck.usedCards.Clear();
        ShuffleDeck(playerDeck.cards);
    }
}
