using UnityEngine;

public class TutorialWaitButtonClick : TutorialObjectBase
{
    bool buttonClicked = false;

    public override void StartStep()
    {
        gameObject.SetActive(true);
    }

    public override bool CanProceed()
    {
        return buttonClicked;
    }

    public void OnButtonClicked()
    {
        buttonClicked = true;
    }
}
