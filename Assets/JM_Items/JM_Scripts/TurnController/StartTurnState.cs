using UnityEngine;
using System.Collections;

public class StartTurnState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        Debug.Log("StartTurnState");
        controller.turn++;
        controller.UpdateDeckText();
        controller.StartCoroutine(BuyCardsNow(controller));
    }

    private IEnumerator BuyCardsNow(JM_TurnController controller)
    {
        controller.BuyCard(controller.player1Deck, true);
        controller.BuyCard(controller.player2Deck, false);

        while (controller.handManager.activeCoroutine > 0)
        {
            yield return null;
        }

        controller.SwitchState(GameStates.p1Choosing);
    }

    public override void UpdateState(JM_TurnController controller)
    {
       
    }

    public override void ExitState(JM_TurnController controller)
    {
        
    }
}
