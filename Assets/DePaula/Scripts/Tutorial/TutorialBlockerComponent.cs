using UnityEngine;

public class TutorialBlockerComponent : MonoBehaviour
{
    [SerializeField] TutorialController controller;

    public bool IsBlocked()
    {
        return controller.IsInTutorialMode();
    }
}
