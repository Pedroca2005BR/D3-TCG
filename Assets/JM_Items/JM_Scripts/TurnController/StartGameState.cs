using UnityEngine;

public class StartGameState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        controller.turn = 0;
        controller.ShuffleDeck(controller.player1Deck.cards);
        controller.ShuffleDeck(controller.player2Deck.cards);
        controller.initializeHands();
    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.SwitchState(GameStates.startingTurn);
    }

    public override void ExitState(JM_TurnController controller)
    {
        
    }
}
