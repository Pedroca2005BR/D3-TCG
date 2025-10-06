using UnityEngine;

public class StartTurnState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        controller.BuyCard(controller.player1Deck, controller.player1Hand);
        controller.BuyCard(controller.player2Deck, controller.player2Hand);
    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.SwitchState(GameStates.p1Choosing);
    }
}
