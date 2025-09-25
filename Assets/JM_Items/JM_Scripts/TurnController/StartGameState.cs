using UnityEngine;

public class StartGameState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        controller.ShuffleDeck(controller.playerDeck.cards);
        controller.BuyCard(controller.gameRules.initialHandSize);
    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.SwitchState(controller.startingState);
    }
}
