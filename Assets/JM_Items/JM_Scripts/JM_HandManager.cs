using UnityEngine;
using System.Collections.Generic;

public class JM_HandManager : MonoBehaviour
{
    [SerializeField] JM_RulesObject gameRules;
    [SerializeField] CardInstance cardInstance;
    [SerializeField] Transform player1HandUI;
    [SerializeField] Transform player2HandUI;
    public List<CardInstance> player1Hand = new List<CardInstance>();
    public List<CardInstance> player2Hand = new List<CardInstance>();

    public bool AddCard(JM_DeckManager deck, bool isPlayer1)
    {
        List<CardInstance> hand = isPlayer1 ? player1Hand : player2Hand;

        if (deck.cards.Count > 0 && hand.Count < gameRules.handSize)
        {
            CardData chosenCard = deck.cards[0];
            deck.cards.RemoveAt(0);
            deck.BoughtCard(chosenCard);

            Transform handUI = isPlayer1 ? player1HandUI : player2HandUI;
            CardInstance newCard = Instantiate(cardInstance, handUI);
            newCard.SetupCardInstance(chosenCard, isPlayer1);
            hand.Add(newCard);
            Debug.Log("Carta comprada");
            return true;
        }
        else if (deck.cards.Count <= 0)
        {
            return false;
        }

        Debug.Log("Mao cheia");
        return true;
    }
}
