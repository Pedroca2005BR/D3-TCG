using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DraggableDeckDisplay : DeckDisplay, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public string filename;
    public CanvasGroup canvasGroup;

    public override void Initialize(string deckName, Action onLoad, Action onDelete = null, string dto = null)
    {
        if (deckNameText != null) deckNameText.text = deckName ?? "(untitled)";
        this.filename = dto;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = new Vector3 (eventData.position.x, eventData.position.y, 0);
        transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }
}
