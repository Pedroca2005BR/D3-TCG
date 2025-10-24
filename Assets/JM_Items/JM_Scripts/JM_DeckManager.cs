using System.Collections.Generic;
using System.Diagnostics;

[System.Serializable]
public class JM_DeckManager
{
    public List<CardData> cards = new List<CardData>();
    public List<CardData> usedCards = new List<CardData>();
    public Dictionary<CardData, CardSlot> deadCards = new Dictionary<CardData, CardSlot>();
    public JM_RulesObject baseConfig;
    public JM_DeckBase cardDatabase;

    public void Setup(JM_RulesObject baseConfig, bool isPlayer1)
    {
        this.baseConfig = baseConfig;

        if (baseConfig.gameMode == GameMode.MultiplayerLocal)
        {
            if (isPlayer1)
            {
                cardDatabase = baseConfig.deck1;
            }
            else
            {
                cardDatabase = baseConfig.deck2;
            }
        }
        else
        {
            // tem que fazer a coisa da ia ainda
            throw new System.NotImplementedException();
        }

        if (cardDatabase != null)
        {
            cards = cardDatabase.allCards;
            deadCards = new Dictionary<CardData, CardSlot>();
            usedCards = new List<CardData>();
        }
    }

    public bool AddCard(CardData card)
    {
        if (cards.Count >= baseConfig.maxCards) return false;
        cards.Add(card);
        return true;
    }

    public bool RemoveCard(CardData card)
    {
        if (cards.Count <= 0) return false;
        if (!cards.Contains(card)) return false;
        cards.Remove(card);
        return true;
    }

    public void BoughtCard(CardData card)
    {
        usedCards.Add(card);
    }

    public void DeadCards(CardData card, CardSlot slot)
    {
        deadCards[card] = slot;
    }
}
