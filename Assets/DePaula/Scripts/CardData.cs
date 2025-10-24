using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
[System.Serializable]
public class CardData : ScriptableObject
{
    public string id; // GUID persistente (preencher no editor)
    public string addressableKey; // opcional: key se usar Addressables

    public string cardName;
    [TextArea] public string cardDescription;
    public CardType type;
    //[TextArea] public string flavorText;
    public int health;
    public int attack;
    public EffectActivationData[] effects;
    public Sprite cardArt;
    public Sprite backgroundArt;

    public List<EffectActivationData> GetEffectsByTime(TimeToActivate state)
    {
        List<EffectActivationData> ead = new List<EffectActivationData>();
        
        for(int i = 0; i < effects.Length; i++)
        {
            if (state == effects[i].timeToActivate)
            {
                ead.Add(effects[i]);
            }
        }

        return ead;
    }

    public bool CheckIfCanUse(EffectActivationData effect, bool wasAlreadyUsed)
    {
        if (effect.singleUse && wasAlreadyUsed) return false;
        else return true;
    }
}

[System.Serializable]
public struct EffectActivationData
{
    public EffectObject effect;
    public TimeToActivate timeToActivate;
    //public TargetLimiterData targets;
    public Targeting targeting;
    public int specialParameter;
    public bool isSlotEffect;
    [Range(0f, 1f)] public float chance;
    public bool singleUse;
}

public enum TimeToActivate
{
    Passive = 0,
    OnReveal = 1,
    OnAttack = 2,
    OnKill = 3,
    OnTakeDamage = 4,
    OnDeath = 5,
    OnPlay = 6,
    OnStartOfTurn = 7
}
