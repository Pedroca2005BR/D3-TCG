using UnityEngine;

public class JM_TurnController : MonoBehaviour
{
    [Header("Regras do Jogo")]
    public JM_RulesObject gameRules;
    public float maxTime;
    public float initialTime = 0f;


    TurnBaseState currentState;
    public StartTurnState startingState = new StartTurnState();
    public ChooseActionsState choosingState = new ChooseActionsState();
    public ProcessingGameState processingState = new ProcessingGameState();
    public EndTurnState endingState = new EndTurnState();

    void Start()
    {
        currentState = startingState;

        currentState.EnterState(this);

        if (gameRules != null)
        {
            maxTime = gameRules.turnTime;
        }

    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(TurnBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
}
