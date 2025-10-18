using UnityEngine;
using System.Collections;
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
            StartCoroutine(SpawnCardVisual(chosenCard, isPlayer1, handUI, hand));

            return true;
        }
        else if (deck.cards.Count <= 0)
        {
            return false;
        }

        Debug.Log("Mao cheia");
        return true;
    }
    
    public IEnumerator SpawnCardVisual(CardData data, bool isPlayer1, Transform handUI, List<CardInstance> hand)
    {
        yield return new WaitForSeconds(2f);

        CardInstance newCard = Instantiate(cardInstance, handUI);
        newCard.SetupCardInstance(data, isPlayer1);

        JM_HandUI handScript = handUI.GetComponent<JM_HandUI>();
        handScript.cardsInHand.Add(newCard);
        handScript.UpdateHandUI();

        hand.Add(newCard);
        
        Debug.Log("Carta comprada");
    }
    
}
