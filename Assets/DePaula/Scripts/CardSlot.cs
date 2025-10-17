using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public CardInstance CardInstance { get; private set; }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped " + eventData.pointerDrag.name);

        if (eventData.pointerDrag.TryGetComponent<CardInstance>(out CardInstance cardInstance))
        {
            CardInstance = cardInstance;
            CardInstance.ConfirmPlay();
        }
        else
        {
            Debug.Log("Nao eh carta");
            return;
        }
        
        // Animation to play when card played
        CardInstance.transform.position = transform.position;   // por enquanto, so da snap pra posicao
    }
}
