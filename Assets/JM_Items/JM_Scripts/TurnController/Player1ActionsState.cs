using UnityEngine;

public class Player1ActionsState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        Debug.Log("P1TurnState");
        controller.p1Hand.UpdateHandUI();
        controller.p2Hand.UpdateHandUI();
        controller.initialTime = 0f;
    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.initialTime += Time.deltaTime;

        if (controller.initialTime >= controller.gameRules.turnTime || controller.player1Played)
        {
            controller.player1Played = true;
            controller.initialTime = 0f;
            controller.SwitchState(GameStates.p2Choosing);
            return;
        }
    }

    public override void ExitState(JM_TurnController controller)
    {
        
    }
}
