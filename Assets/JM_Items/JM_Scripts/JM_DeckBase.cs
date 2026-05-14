using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DeckBase", menuName = "Scriptable Objects/DeckBase")]
[System.Serializable]
public class JM_DeckBase : ScriptableObject
{
    public List<CardData> allCards; 
    public bool isTutorialDeck = false; // flag para indicar se este é um deck de tutorial
}
