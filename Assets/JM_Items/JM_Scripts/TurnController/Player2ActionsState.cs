using UnityEngine;

public class Player2ActionsState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        controller.initialTime = 0f;

        if (controller.iaGame && !controller.player2Played)
        {
            //chama a ia;

            controller.player2Played = true;
        }
    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.initialTime += Time.deltaTime;

        if (controller.initialTime >= controller.gameRules.turnTime || controller.player2Played)
        {
            controller.player2Played = true;
            controller.initialTime = 0f;
            controller.SwitchState(GameStates.revealing);
            return;
        }
    }

    public override void ExitState(JM_TurnController controller)
    {
        
    }
}
