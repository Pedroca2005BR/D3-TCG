using UnityEngine;

public class EndTurnState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        Debug.Log("EndTurnState");
        controller.player1Played = false;
        controller.player2Played = false;
    }

    public override void UpdateState(JM_TurnController controller)
    {
        if (controller.lastTurn) controller.SwitchState(GameStates.finishingGame);
        else controller.SwitchState(GameStates.startingTurn);
        return;
    }
    
    public override void ExitState(JM_TurnController controller)
    {
        
    }
}
