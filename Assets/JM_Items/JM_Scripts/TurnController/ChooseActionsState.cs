using UnityEngine;

public class ChooseActionsState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        controller.initialTime = 0f;
    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.initialTime += Time.deltaTime;

        if (controller.initialTime >= controller.gameRules.turnTime)
        {
            controller.initialTime = 0f;
            controller.SwitchState(controller.revealingState);
            return;
        }
    }
}
