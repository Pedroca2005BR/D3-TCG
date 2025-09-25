using UnityEngine;

public class RevealingCardState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        
    }

    public override void UpdateState(JM_TurnController controller)
    {
        controller.SwitchState(controller.processingState);
    }
}
