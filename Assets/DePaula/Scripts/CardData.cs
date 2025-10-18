using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
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
}

[System.Serializable]
public struct EffectActivationData
{
    public EffectObject effect;
    public TimeToActivate timeToActivate;
    //public TargetLimiterData targets;
    public Targeting targeting;
    public int specialParameter;
}

public enum TimeToActivate
{
    Passive = 0,
    OnReveal = 1,
    OnAttack = 2,
    OnKill = 3,
    OnTakeDamage = 4,
    OnDeath = 5,
    OnPlay = 6
}
