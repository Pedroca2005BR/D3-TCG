using UnityEngine;

public class StartGameState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        controller.ShuffleDeck(controller.player1Deck.cards);
        controller.ShuffleDeck(controller.player2Deck.cards);
        controller.initializeHands();
    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.SwitchState(GameStates.startingTurn);
    }
}
