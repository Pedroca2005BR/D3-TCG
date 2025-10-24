using UnityEngine;

public class StartGameState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        Debug.Log("StartGameState");
        controller.turn = 0;
        
        controller.hero1 = GameManager.Instance.GetHero(true);
        controller.hero2 = GameManager.Instance.GetHero(false);
        controller.hero1.Setup();
        controller.hero2.Setup();

        controller.player1Deck.Setup(controller.gameRules, true);
        controller.player2Deck.Setup(controller.gameRules, false);

        controller.ShuffleDeck(controller.player1Deck.cards);
        controller.ShuffleDeck(controller.player2Deck.cards);
        controller.InitializeHands();

        
    }

    public override void UpdateState(JM_TurnController controller)
    {
        
    }

    public override void ExitState(JM_TurnController controller)
    {
        
    }
}
