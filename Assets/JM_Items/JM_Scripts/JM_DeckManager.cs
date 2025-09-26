using System.Collections.Generic;

[System.Serializable]
public class JM_DeckManager
{
    public List<CardData> cards = new List<CardData>();
    public List<CardData> usedCards = new List<CardData>();
    public JM_RulesObject baseConfig;
    public JM_DeckBase cardDatabase; 

    public bool AddCard(CardData card) 
    {
        if (cards.Count >= baseConfig.maxCards) return false;
        cards.Add(card);
        return true;
    }

    public bool RemoveCard(CardData card)
    {
        if(cards.Count <= 0) return false;
        if(!cards.Contains(card)) return false;
        cards.Remove(card);
        return true;
    }

    public void BoughtCard(CardData card)
    {
        usedCards.Add(card);
    }
}
