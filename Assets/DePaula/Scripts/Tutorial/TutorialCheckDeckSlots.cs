using UnityEngine;

public class TutorialCheckDeckSlots : TutorialObjectBase
{
    [SerializeField] DeckSlot[] slots;

    public override void StartStep()
    {
        gameObject.SetActive(true);

        foreach(var slot in slots)
        {
            slot.OnDeckDropped += CheckIfCanProceed;
        }
    }

    public override bool CanProceed()
    {
        foreach(var slot in slots)
        {
            if (slot.deckDisplay == null)
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
            TryProceed();
        }
    }
}
