using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DraggableDeckDisplay : DeckDisplay, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public string filename;
    public bool dropped = false;
    public bool isBeingDragged = false;
    private Vector3 velocity = Vector3.zero;
    private Vector3 dragOffset;
    private Vector3 dragTargetPosition;
    public CanvasGroup canvasGroup;
    private Transform tempParent;
    private Transform currentParent;
    private Transform originalParent;

    public override void Initialize(string deckName, Action onLoad, Action onDelete = null, string dto = null)
    {
        if (deckNameText != null) deckNameText.text = deckName ?? "(untitled)";
        this.filename = dto;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = GameObject.FindGameObjectWithTag("deckContent").transform;
        tempParent = GameObject.FindGameObjectWithTag("tempParent").transform;
        currentParent = this.transform.parent;
        this.transform.SetParent(tempParent);
        isBeingDragged = true;
        dropped = false;
        Vector3 worldPoint;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)transform.parent,
            eventData.position,
            Camera.main,
            out worldPoint
        );
        
        canvasGroup.blocksRaycasts = false;
        dragOffset = transform.position - worldPoint;

        dragTargetPosition = transform.position;
    }

    public void Update()
    {
        if (isBeingDragged)
        {
            transform.position = Vector3.SmoothDamp(
            transform.position,
            dragTargetPosition,
            ref velocity,
            0.02f 
            );
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isBeingDragged) return;
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)transform.parent,
            eventData.position,
            Camera.main,
            out worldPoint
        );

        dragTargetPosition = new Vector3(worldPoint.x, worldPoint.y, transform.position.z) + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        isBeingDragged = false;

        if(!dropped)
        {
            this.transform.SetParent(originalParent);
           
        }
    }

}
