using UnityEngine;

public class EndGameState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        controller.initialTime = 0f;
        controller.lastTurn = false;
        controller.OrganizeDeck();
    }

    public override void UpdateState(JM_TurnController controller)
    {

    }

    public override void ExitState(JM_TurnController controller)
    {
        
    }
}
