using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckSlot : MonoBehaviour, IDropHandler
{
    public DraggableDeckDisplay deckDisplay;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<DraggableDeckDisplay>(out DraggableDeckDisplay ddd))
        {
            ddd.transform.SetParent(transform, false);

            ddd.transform.position = transform.position;

            deckDisplay = ddd;

            ddd.canvasGroup.blocksRaycasts = false;
        }
    }

    public bool TryGetDTO(out string filename)
    {
        filename = null;
        if  (deckDisplay == null) { return false; }

        filename = deckDisplay.filename;
        return true;
    }
}
