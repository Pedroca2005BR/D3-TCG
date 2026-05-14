using UnityEngine;

public class TutorialCheckDeckSlots : TutorialObjectBase
{
    [SerializeField] DeckSlot[] slots;

    public override void StartStep()
    {
        gameObject.SetActive(true);
    }

    public override bool CanProceed()
    {
        foreach(DeckSlot slot in slots)
        {
            if (slot.deckDisplay == null)
            {
                return false;
            }
        }

        return true;
    }
}
