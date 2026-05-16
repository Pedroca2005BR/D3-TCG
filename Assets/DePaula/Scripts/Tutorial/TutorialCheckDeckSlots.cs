using System.Collections;
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
        StartCoroutine(WaitAndCheck());
    }

    IEnumerator WaitAndCheck()
    {
        yield return new WaitForSeconds(0.05f);
        int counter = 0;

        foreach (var slot in slots)
        {
            if (slot.deckDisplay != null)
            {
                counter++;
            }
        }

        if (counter == slots.Length)
        {
            //Debug.Log("All slots are filled, proceeding to the next step.");
            TryProceed();
        }
    }
}
