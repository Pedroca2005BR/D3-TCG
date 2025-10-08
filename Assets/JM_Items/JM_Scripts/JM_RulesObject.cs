using UnityEngine;

[CreateAssetMenu(fileName = "RulesObject", menuName = "Scriptable Objects/RulesObject")]
public class JM_RulesObject : ScriptableObject
{
    public float turnTime;

    public int handSize;

    public int initialHandSize;

    public int arenaSlots;

    public int maxCards;

    public int maxDrawCards;

    public int heroHealth;

    //public int difficulty;

}
