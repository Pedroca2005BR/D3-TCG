using UnityEngine;

public class ProcessingGameState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        Debug.Log("ProcessingState");

        ProcessAttacks();
    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.SwitchState(GameStates.endingTurn);
    }

    public override void ExitState(JM_TurnController controller)
    {
        
    }

    private void ProcessAttacks()
    {
        IGameEntity[] targets;
        int damage;
        CardInstance[] cards = GameManager.Instance.GetAllCards();

        for (int i = 0; i < cards.Length; i++)
        {
            targets = TargetSelector.GetTargets(cards[i], cards[i].AttackTargeting);
            //Debug.LogError(targets.Length.ToString() + " " + cards.Length.ToString());

            for(int j=0; j < targets.Length; j++)
            {
                damage = cards[i].GetAttackDamage(targets[j]);

                if (damage > 0)
                {
                    targets[j].TakeDamage(cards[i], damage);
                }
            }
        }

        // Process kills
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].GetCurrentHealth() == 0)
            {
                CardInstance killer = cards[i].Die() as CardInstance;
                if (killer != null)
                {
                    killer.BecomeAKiller();
                }
                else
                {
                    Debug.LogError("Como tu morreu de morte morrida, querida??");
                }
            }
        }
    }
}
