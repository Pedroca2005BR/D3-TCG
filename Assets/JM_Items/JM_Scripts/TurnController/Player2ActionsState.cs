using UnityEngine;

public class Player2ActionsState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        Debug.Log("P2TurnState");
        controller.p1Hand.UpdateHandUI();
        controller.p2Hand.UpdateHandUI();

        if (controller.iaGame && !controller.player2Played)
        {
            //chama a ia;

            controller.player2Played = true;
        }
    }

    public override void UpdateState(JM_TurnController controller)
    {

        if (controller.player2Played)
        {
            controller.player2Played = true;
            controller.StartCoroutine(controller.FlipBoard(GameStates.revealing));
            return;
        }
    }

    public override void ExitState(JM_TurnController controller)
    {
        
    }
}
