using UnityEngine;

public class ProcessingGameState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        Debug.Log("ProcessingState");
    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.SwitchState(GameStates.endingTurn);
    }

    public override void ExitState(JM_TurnController controller)
    {
        
    }
}
