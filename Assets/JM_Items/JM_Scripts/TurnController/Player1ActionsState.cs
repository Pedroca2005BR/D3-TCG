using UnityEngine;

public class Player1ActionsState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        Debug.Log("P1TurnState");
        controller.dontAct = false;
        controller.p1Hand.UpdateHandUI();
        controller.p2Hand.UpdateHandUI();
    }

    public override void UpdateState(JM_TurnController controller)
    {

        if (controller.player1Played && !controller.dontAct)
        {
            controller.dontAct = true;
            controller.StartCoroutine(controller.FlipBoard(GameStates.p2Choosing));
            return;
        }
    }

    public override void ExitState(JM_TurnController controller)
    {
        
    }
}
