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
    [SerializeField] JM_TurnController turnController;
    [SerializeField] EffectHandler effectHandler;

    [Header("Card Slots")]
    [SerializeField] CardSlot[] slotsP1;
    [SerializeField] CardSlot[] slotsP2;

    [Header("Heroes")]
    [SerializeField] Hero P1;
    [SerializeField] Hero P2;

    [Header("Decks")]
    [SerializeField] JM_DeckManager deckP1;
    [SerializeField] JM_DeckManager deckP2;

    // TO DO: TODA A MECANICA DE CONTROLE DO JOGO



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
}
