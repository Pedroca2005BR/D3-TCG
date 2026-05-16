using System.Collections;
using UnityEngine;

public class FightButtonGlueCode : MonoBehaviour
{
    [SerializeField] DeckSlot[] slots;
    RotateComponent rotateComponent;

    private void Start()
    {
        rotateComponent = GetComponent<RotateComponent>();

        foreach (var slot in slots)
        {
            slot.OnDeckDropped += TryProceed;
        }
    }

    public void TryProceed()
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
            rotateComponent.Rotate();
        }
    }


    private void OnDisable()
    {
        foreach (var slot in slots)
        {
            slot.OnDeckDropped -= TryProceed;
        }
    }

}
