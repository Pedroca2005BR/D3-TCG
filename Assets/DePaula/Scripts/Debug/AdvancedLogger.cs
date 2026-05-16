using System.Linq;
using UnityEngine;

public static class AdvancedLogger
{
    public static void Log(JM_DeckBase deck)
    {
        string allCards = string.Join(", ", deck.allCards.Select(card => card.name));
        Debug.Log($"Deck contains {deck.allCards.Count} cards: {allCards}");
    }

    public static string Log(CardData card)
    {
        return $"Card Name: {card.name}, Type: {card.type}, Addressable Key: {card.addressableKey}";
    }
}
