using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DeckBase", menuName = "Scriptable Objects/DeckBase")]
[System.Serializable]
public class JM_DeckBase : ScriptableObject
{
    public List<CardData> allCards;  
}
