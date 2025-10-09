using UnityEngine;

public class RevealingCardState : TurnBaseState
{
    public override void EnterState(JM_TurnController controller)
    {
        controller.Reveal();
    }

    public override void UpdateState(JM_TurnController controller)
    {
        
    }
    
    public override void ExitState(JM_TurnController controller)
    {
        controller.StopAllCoroutines(); 
    }
}
