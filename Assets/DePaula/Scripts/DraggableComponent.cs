using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableComponent : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private bool dropped = false;
    //private float snapRange = 1f;
    Vector3 velocity = Vector3.zero;
    Vector3 dragTargetPosition;
    bool isBeingDragged;
    Vector3 dragOffset;
    Vector3 initialPosition;

    public CardInstance CardInstance { get; private set; }



    public void Setup(CardInstance cardInstance)
    {
        CardInstance = cardInstance;
    }

    void Update()
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
        isBeingDragged = false;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (!dropped)
        {
            StartCoroutine(ReturnToHand());
        }
    }

    public void ConfirmDropped()
    {
        dropped = true;

        // I hate it but I don't know how to do it better without making a mess of the code, so here we are
        GameManager.Instance.UpdateHandsUI();

        transform.localPosition = Vector3.zero; // Snap to slot position

        GetComponent<CanvasGroup>().blocksRaycasts = true; // Makes the description box work when the card is in play

        this.enabled = false; // Disable dragging after confirming drop
    }

    public void DenyDrop()
    {
        dropped = false;
        StartCoroutine(ReturnToHand());
    }

    public IEnumerator ReturnToHand()
    {
        float t = 0f;
        Vector3 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * 5f;
            transform.position = Vector3.Lerp(startPos, initialPosition, t);
            yield return null;
        }

        transform.position = initialPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CardInstance.Mode == CardMode.InHand)
        {
            initialPosition = transform.position;
            isBeingDragged = true;
            dropped = false;

            Vector3 worldPoint;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                (RectTransform)transform.parent,
                eventData.position,
                Camera.main,
                out worldPoint
            );
            dragOffset = transform.position - worldPoint;

            dragTargetPosition = transform.position;

            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }
}
