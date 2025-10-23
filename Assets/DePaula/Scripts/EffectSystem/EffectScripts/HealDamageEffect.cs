using UnityEngine;

[CreateAssetMenu(fileName = "HealDamageEffect", menuName = "Scriptable Objects/Effects/HealDamageEffect")]
public class HealDamageEffect : EffectObject
{
    //public int regeneratingPower;


    public override int Resolve(CardInstance source, IGameEntity[] targets, int specialParam, int bonusParam = 0)
    {
        if (targets == null || targets.Length == 0)
        {
            Debug.LogError("Nothing to heal. Targets is empty or null.");
            return -1;
        }

        for(int i=0; i<targets.Length; i++)
        {
            targets[i].Heal(specialParam);
        }

        return specialParam;
    }
}
