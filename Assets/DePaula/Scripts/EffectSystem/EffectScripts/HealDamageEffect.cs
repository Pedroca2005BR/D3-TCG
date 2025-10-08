using UnityEngine;

[CreateAssetMenu(fileName = "HealDamageEffect", menuName = "Scriptable Objects/Effects/HealDamageEffect")]
public class HealDamageEffect : EffectObject
{
    //public int regeneratingPower;


    public override void Resolve(CardInstance source, IGameEntity[] targets, int specialParam)
    {
        if (targets == null || targets.Length == 0)
        {
            Debug.LogError("Nothing to heal. Targets is empty or null.");
        }

        for(int i=0; i<targets.Length; i++)
        {
            IDamageable tg = targets[i] as IDamageable;
            tg.Heal(specialParam);
        }
    }
}
