using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JM_HandManager : MonoBehaviour
{
    [SerializeField] JM_RulesObject gameRules;
    [SerializeField] CardInstance cardInstance;
    [SerializeField] Transform player1HandUI;
    [SerializeField] Transform player2HandUI;
    public JM_TurnController turnController;    
    public List<CardInstance> player1Hand = new List<CardInstance>();
    public List<CardInstance> player2Hand = new List<CardInstance>();
    public int activeCoroutine = 0;

    public bool AddCard(JM_DeckManager deck, bool isPlayer1)
    {
        List<CardInstance> hand = isPlayer1 ? player1Hand : player2Hand;

        if (deck.cards.Count > 0 && hand.Count < gameRules.handSize)
        {
            CardData chosenCard = deck.cards[0];
            deck.cards.RemoveAt(0);
            deck.BoughtCard(chosenCard);

            if (isPlayer1)
                turnController.player1DeckCount = deck.cards.Count;
            else
                turnController.player2DeckCount = deck.cards.Count;

            turnController.UpdateDeckText();

            Transform handUI = isPlayer1 ? player1HandUI : player2HandUI;
            StartCoroutine(SpawnCard(chosenCard, isPlayer1, handUI, hand));

            return true;
        }
        else if (deck.cards.Count <= 0)
        {
            return false;
        }

        Debug.Log("Mao cheia");
        return true;
    }

    public bool AddZombieCard(CardData card, bool isPlayer1)
    {
        List<CardInstance> hand = isPlayer1 ? player1Hand : player2Hand;

        if (hand.Count < gameRules.handSize)
        {
            Transform handUI = isPlayer1 ? player1HandUI : player2HandUI;
            StartCoroutine(SpawnCard(card, isPlayer1, handUI, hand));

            return true;
        }
        

        Debug.Log("Mao cheia");
        return false;
    }
    
    public IEnumerator SpawnCard(CardData data, bool isPlayer1, Transform handUI, List<CardInstance> hand)
    {
        activeCoroutine++;
        
        yield return new WaitForSeconds(0.5f);

        CardInstance newCard = Instantiate(cardInstance, handUI);
        newCard.SetupCardInstance(data, isPlayer1);

        JM_HandUI handScript = handUI.GetComponent<JM_HandUI>();
        handScript.cardsInHand.Add(newCard);
        handScript.UpdateHandUI();

        hand.Add(newCard);

        Debug.Log("Carta comprada");

        activeCoroutine--;
    }
    
}
