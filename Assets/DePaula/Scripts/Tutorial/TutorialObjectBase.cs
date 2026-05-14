using UnityEngine;
using UnityEngine.Events;

public abstract class TutorialObjectBase : MonoBehaviour
{
    TutorialController controller;

    public void SetController(TutorialController controller)
    {
        this.controller = controller;
    }

    public virtual void StartStep()
    {
        gameObject.SetActive(true);
    }

    public virtual void StopStep()
    {
        gameObject.SetActive(false);
    }

    public virtual bool CanProceed()
    {
        return true;
    }

    protected bool TryProceed()
    {
        if (controller != null)
        {
            return controller.TryNextStep(this);
        }
        return false;
    }
}
