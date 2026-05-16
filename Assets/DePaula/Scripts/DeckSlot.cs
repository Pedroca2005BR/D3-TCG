using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DeckSlot : MonoBehaviour, IDropHandler
{
    public UnityAction OnDeckDropped; // Event to notify when a deck is dropped in this slot
    public DraggableDeckDisplay deckDisplay;
    [SerializeField] private RectTransform _contentGroup;

    [Header("Visual")]
    [SerializeField] PulsatingComponent pulsatingComponent;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<DraggableDeckDisplay>(out DraggableDeckDisplay ddd))
        {
            if (deckDisplay != null)
            {
                deckDisplay.transform.SetParent(_contentGroup, false);  // retorna o antigo pro content group
                deckDisplay.dropped = false;
                deckDisplay.canvasGroup.blocksRaycasts = true;
            }

            ddd.transform.SetParent(transform, false);

            ddd.dropped = true;

            ddd.transform.position = transform.position;

            deckDisplay = ddd;

            ddd.canvasGroup.blocksRaycasts = false;

            OnDeckDropped?.Invoke(); // Notify listeners that a deck has been dropped in this slot
            pulsatingComponent.gameObject.SetActive(false);
            deckDisplay.onEndDragAction += CheckIfStillSelected;
        }
    }

    public bool TryGetDTO(out string filename)
    {
        CheckIfStillSelected();

        filename = null;
        if (deckDisplay == null) { return false; }

        filename = deckDisplay.filename;
        return true;
    }

    private void CheckIfStillSelected()
    {
        if (deckDisplay != null && deckDisplay.transform.parent != transform)
        {
            deckDisplay.onEndDragAction -= CheckIfStillSelected;
            deckDisplay = null;
            pulsatingComponent.gameObject.SetActive(true);
        }
    }
}
