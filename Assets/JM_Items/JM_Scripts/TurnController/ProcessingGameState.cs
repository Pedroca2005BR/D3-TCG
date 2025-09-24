using UnityEngine;

public class ProcessingGameState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {

    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.SwitchState(controller.endingState);
    }
}
