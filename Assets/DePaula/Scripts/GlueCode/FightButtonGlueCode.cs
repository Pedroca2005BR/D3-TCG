using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FightButtonGlueCode : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] DeckSlot[] slots;
    RotateComponent rotateComponent;
    ColorChangerComponent colorChangerComponent;
    ShakeComponent shakeComponent;

    private void Start()
    {
        rotateComponent = GetComponent<RotateComponent>();
        colorChangerComponent = GetComponent<ColorChangerComponent>();
        shakeComponent = GetComponent<ShakeComponent>();
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
    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (var slot in slots)
        {
            if (slot.deckDisplay == null)
            {
                shakeComponent.Shake();
                colorChangerComponent.ChangeColor();
                return;
            }
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
