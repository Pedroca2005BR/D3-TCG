using UnityEngine;

public class EndTurnState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {

    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.SwitchState(controller.startingState);
    }
}
