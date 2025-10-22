using System.Collections.Generic;
using UnityEngine;

public static class DeckMapper
{
    public static DeckDTO ToDTO(JM_DeckBase deck)
    {
        var dto = new DeckDTO
        {
            id = string.IsNullOrEmpty(deck.name) ? System.Guid.NewGuid().ToString() : deck.name,
            name = deck.name,
            cardKeys = new List<string>(),
            version = 1
        };

        foreach (var c in deck.allCards)
        {
            if (c == null) continue;
            if (!string.IsNullOrEmpty(c.addressableKey)) dto.cardKeys.Add(c.addressableKey);
            else if (!string.IsNullOrEmpty(c.id)) dto.cardKeys.Add(c.id); // fallback se usar GUID como key
            else Debug.LogWarning($"CardData {c.name} não tem addressableKey nem id.");
        }
        return dto;
    }
}
