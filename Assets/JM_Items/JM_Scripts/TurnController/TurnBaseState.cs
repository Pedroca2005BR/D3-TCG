using UnityEngine;

public abstract class TurnBaseState
{

    public abstract void EnterState(JM_TurnController controller);

    public abstract void UpdateState(JM_TurnController controller);
}
