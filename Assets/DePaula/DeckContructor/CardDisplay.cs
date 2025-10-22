// ---------------- Example CardDisplay component ----------------
// A small example script to put on your card display prefab. You said you can wire click behaviour in it,
// but here's a simple implementation that sets texts and image and exposes Initialize(CardData, Action).

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Visuals")]
    [Header("Display Info")]
    [SerializeField] TextMeshProUGUI nameComponent;
    [SerializeField] GameObject descriptionImage;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI healthComponent;
    [SerializeField] TextMeshProUGUI attackComponent;
    [SerializeField] Image cardArtComponent;
    [SerializeField] Image backgroundComponent;

    UnityAction eventToCall; // the clickable area (assign on prefab)
    IEnumerator descriptionCoroutine;

    // Initialize visuals and set click callback. The user can also override this script to add more functionality.
    public void Initialize(CardData data, UnityAction onClick)
    {
        if (data == null)
        {
            if (nameComponent != null) nameComponent.text = "(null)";
            return;
        }

        if (nameComponent != null) nameComponent.text = data.cardName;
        if (cardArtComponent != null && data.cardArt != null) cardArtComponent.sprite = data.cardArt;
        if (attackComponent != null) attackComponent.text = data.attack.ToString();
        if (healthComponent != null) healthComponent.text = data.health.ToString();
        if (descriptionText != null) descriptionText.text = data.cardDescription;
        if (descriptionImage != null) descriptionImage.SetActive(false);

        if (eventToCall != null)
        {
            //rootButton.onClick.RemoveAllListeners();
            if (onClick != null) eventToCall = onClick;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        eventToCall.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // TODO : Animation for hovering
        descriptionCoroutine = DescriptionAppearTimer();
        StartCoroutine(descriptionCoroutine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // TODO: Stop animation for hovering
        StopCoroutine(descriptionCoroutine);
        descriptionImage.SetActive(false);
    }

    IEnumerator DescriptionAppearTimer()
    {
        yield return new WaitForSeconds(1f);
        descriptionImage.SetActive(true);    // TO DO: Adicionar easing
        Debug.Log("Description!");
    }
}
