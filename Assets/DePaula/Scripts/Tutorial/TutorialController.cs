using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialController : MonoBehaviour
{
    [Header("Rules Reference")]
    [SerializeField] JM_RulesObject rules;

    [Header("Tutorial Steps")]
    [SerializeField] TutorialObjectBase[] tutorialSteps;
    private int currentStepIndex = 0;

    private void Start()
    {
        foreach (var step in tutorialSteps)
        {
            step.SetController(this);
        }

        currentStepIndex = 0;
        if (rules.gameMode == GameMode.Tutorial)
        {
            NextStep();
        }
    }

    public void NextStepByUI(InputAction.CallbackContext context)
    {
        if (context.performed && tutorialSteps[currentStepIndex - 1].CanProceed()) 
            NextStep();
    }

    public void NextStep()
    {
        // Checks if we're still in tutorial mode before proceeding
        if (rules.gameMode != GameMode.Tutorial)
        {
            Debug.LogWarning("Not in tutorial mode. Cannot proceed to next step.");
            return;
        }

        if (currentStepIndex > 0)
        {
            // Stop the previous step if it exists
            tutorialSteps[currentStepIndex - 1].StopStep();
        }
        if (currentStepIndex < tutorialSteps.Length)
        {
            tutorialSteps[currentStepIndex].StartStep();
            currentStepIndex++;
        }
    }

    public bool TryNextStep(TutorialObjectBase step)
    {
        if (currentStepIndex != 0 && step == tutorialSteps[currentStepIndex - 1])
        {
            NextStep();
            return true;
        }
        return false;
    }

    public void EndTutorial()
    {
        rules.gameMode = GameMode.MultiplayerLocal;
        PlayerPrefs.SetInt("HasPlayedBefore", 1);
    }

    public void EnterTutorialMode()
    {
        rules.gameMode = GameMode.Tutorial;
    }

    public bool IsInTutorialMode()
    {
        return rules.gameMode == GameMode.Tutorial;
    }
}
