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

        if ((controller.initialTime >= controller.gameRules.turnTime && controller.player1Played) || controller.player2Played)
        {
            controller.player1Played = false;
            controller.player2Played = false;
            controller.initialTime = 0f;
            controller.SwitchState(controller.revealingState);
            return;
        }
        else if ((controller.initialTime >= controller.gameRules.turnTime && !controller.player1Played) || controller.player1Played)
        {
            controller.player1Played = true;
            controller.initialTime = 0f;
            controller.SwitchState(controller.choosingState);
        }
    }
}
