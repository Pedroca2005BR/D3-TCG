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
    [Header("Display Info")]
    [SerializeField] TextMeshProUGUI nameComponent;
    [SerializeField] GameObject descriptionImage;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI healthComponent;
    [SerializeField] TextMeshProUGUI attackComponent;
    [SerializeField] Image cardArtComponent;
    [SerializeField] Image backgroundComponent;

    [Header("Interaction")]
    [SerializeField] GameObject shadePanel; // Will be turned on when the card is clicked, to give feedback that it was selected.

    UnityAction eventToCall; // the clickable area (assign on prefab)
    IEnumerator descriptionCoroutine;
    [HideInInspector] public CardData cardData;

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
        if (backgroundComponent != null && data.backgroundArt != null) backgroundComponent.sprite = data.backgroundArt;
        if (attackComponent != null) attackComponent.text = data.attack.ToString();
        if (healthComponent != null) healthComponent.text = data.health.ToString();
        if (descriptionText != null) descriptionText.text = data.cardDescription;
        if (descriptionImage != null) descriptionImage.SetActive(false);

        if (onClick != null) eventToCall = onClick;
        cardData = data;
    }

    public void ToggleDisponibility(bool isAvailable)
    {
        shadePanel.SetActive(!isAvailable);
        transform.GetComponent<CanvasGroup>().blocksRaycasts = isAvailable; // block clicks when shaded, allow clicks when not shaded
    }
    

    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log((eventToCall == null).ToString() + (DeckRuntimeUI.Instance == null) + (cardData == null));
        if (eventToCall != null)
        {
            Debug.Log("Card removed from deck: " + cardData.cardName);
            eventToCall.Invoke();
        }
            
        else
        {
            if (DeckRuntimeUI.Instance.AddCard(cardData))
            {
                ToggleDisponibility(false); // feedback that the card was selected
                Debug.Log("Card added to deck: " + cardData.cardName);
            }
        }
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
