using System.Collections;
using UnityEngine;

public class EndTurnState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        Debug.Log("EndTurnState");

        controller.StartCoroutine(ProcessDeaths(controller));
    }

    public override void UpdateState(JM_TurnController controller)
    {

    }
    
    public override void ExitState(JM_TurnController controller)
    {
        
    }
    
    IEnumerator ProcessDeaths(JM_TurnController controller)
    {
        yield return controller.ResolveEffectBus(GameStates.endingTurn);

        controller.player1Played = false;
        controller.player2Played = false;
        controller.p2Entered = false;
        controller.p1Entered = false;

        if (controller.lastTurn) controller.SwitchState(GameStates.finishingGame);
        else controller.SwitchState(GameStates.startingTurn);
    }
}
