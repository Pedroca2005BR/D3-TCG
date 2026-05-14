using UnityEngine;
using UnityEngine.Events;

public abstract class TutorialObjectBase : MonoBehaviour
{
    TutorialController controller;

    public void SetController(TutorialController controller)
    {
        this.controller = controller;
    }

    public abstract void StartStep();
    public virtual void StopStep()
    {
        gameObject.SetActive(false);
    }

    public virtual bool CanProceed()
    {
        return true;
    }

    protected void ProceedNoQuestions()
    {
        if (controller != null)
        {
            controller.NextStep();
        }
    }
}
