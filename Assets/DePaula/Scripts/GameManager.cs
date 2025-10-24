using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    // Singleton pattern
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Rules Object")]
    public JM_RulesObject rules;

    [Header("Important Managers")]
    [SerializeField] public JM_TurnController turnController;
    [SerializeField] public EffectHandler effectHandler;

    [Header("Card Slots")]
    [SerializeField] CardSlot[] slotsP1;
    [SerializeField] CardSlot[] slotsP2;

    [Header("Heroes")]
    [SerializeField] Hero P1;
    [SerializeField] Hero P2;

    
    JM_DeckManager deckP1;
    JM_DeckManager deckP2;

    [Header("Hands")]
    public JM_HandManager HandManager;
    // TO DO: TODA A MECANICA DE CONTROLE DO JOGO

    private void Start()
    {
        deckP1 = turnController.player1Deck;
        deckP2 = turnController.player2Deck;
    }

    public JM_DeckManager GetDeck(bool isPlayer1)
    {
        if (!isPlayer1)
        {
            return deckP2;
        }

        return deckP1;
    }

    public Hero GetHero(bool isPlayer1)
    {
        if (!isPlayer1)
        {
            return P2;
        }

        return P1;
    }

    public CardSlot[] GetSlots(bool isPlayer1)
    {
        if (!isPlayer1) return slotsP2;
        return slotsP1;
    }

    public CardInstance[] GetAllCards()
    {
        List<CardInstance> cards = new List<CardInstance>();
        CardSlot[] enemySlots = GetSlots(false);
        CardSlot[] allySlot = GetSlots(true);

        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (enemySlots[i].CardInstance != null)
                cards.Add(enemySlots[i].CardInstance);
        }
        for (int i = 0; i < allySlot.Length; i++)
        {
            if (allySlot[i].CardInstance != null)
                cards.Add(allySlot[i].CardInstance);
        }

        return cards.ToArray();
    }
}
