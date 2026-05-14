using UnityEngine;

public class TutorialCardSlot : TutorialObjectBase
{
    [SerializeField] private CardSlot[] cardSlots;

    public override void StartStep()
    {
        gameObject.SetActive(true);
        foreach (var slot in cardSlots)
        {
            slot.OnCardPlayed += CheckIfCanProceed;
        }
    }

    public override bool CanProceed()
    {
        foreach (var slot in cardSlots)
        {
            if (slot.empty)
            {
                return false;
            }
        }

        return true;
    }

    // If all slots are filled, proceed to the next step
    private void CheckIfCanProceed()
    {
        if (CanProceed())
        {
            ProceedNoQuestions();
        }
    }

}
