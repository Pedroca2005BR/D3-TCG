using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string cardDescription;
    public CardType type;
    //[TextArea] public string flavorText;
    public int health;
    public int attack;
    public Effect[] effects;
}
