using UnityEngine;
using UnityEngine.Events;

public abstract class TutorialObjectBase : MonoBehaviour
{
    public abstract void StartStep();
    public virtual void StopStep()
    {
        gameObject.SetActive(false);
    }

    public virtual bool CanProceed()
    {
        return true;
    }
}
