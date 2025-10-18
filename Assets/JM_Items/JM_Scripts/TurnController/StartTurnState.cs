using UnityEngine;

public class StartTurnState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        Debug.Log("StartTurnState");
        controller.turn++;
        controller.BuyCard(controller.player1Deck, true);
        controller.BuyCard(controller.player2Deck, false);
    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.SwitchState(GameStates.p1Choosing);
    }

    public override void ExitState(JM_TurnController controller)
    {
        
    }
}
