using UnityEngine;

[CreateAssetMenu(fileName = "RulesObject", menuName = "Scriptable Objects/RulesObject")]
public class JM_RulesObject : ScriptableObject
{
    public float turnTime;

    public int deckSize;

    public int handSize;

    public int initialHandSize;

    public int arenaSlots;

    public int maxCards;

    public int maxDrawCards;

    public int heroHealth;

    public JM_DeckBase deck1;
    public JM_DeckBase deck2;

    public GameMode gameMode;

    //public int difficulty;

}

public enum GameMode
{
    MultiplayerLocal = 0,
    IA = 1
}