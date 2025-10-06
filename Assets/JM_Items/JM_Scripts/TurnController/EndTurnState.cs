using UnityEngine;

public class EndTurnState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        controller.player1Played = false;
        controller.player2Played = false;
    }

    public override void UpdateState(JM_TurnController controller)
    {
        if(controller.lastTurn) controller.SwitchState(controller.finishingState);
        else controller.SwitchState(controller.startingState);
        return;
    }
}
