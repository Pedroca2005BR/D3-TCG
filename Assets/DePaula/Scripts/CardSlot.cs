using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public CardInstance CardInstance { get; private set; }

    public bool isPlayer1Slot = false;
    public bool empty = true;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped " + eventData.pointerDrag.name);

        if (eventData.pointerDrag.TryGetComponent<CardInstance>(out CardInstance cardInstance))
        {
            bool isPlayer1 = cardInstance.IsPlayer1 == isPlayer1Slot;

            JM_HandUI handUI = cardInstance.GetComponentInParent<JM_HandUI>();
            
            if (!empty || !isPlayer1)
            {
                cardInstance.StartCoroutine(cardInstance.ReturnToHand());
            }
            else
            {
                cardInstance.transform.SetParent(transform, false);

                cardInstance.transform.position = transform.position;   // por enquanto, so da snap pra posicao

                cardInstance.dropped = true;
                empty = false;
                CardInstance = cardInstance;
                // Animation to play when card played

                CardInstance.ConfirmPlay();

                
                    
                handUI.UpdateHandUI();
            }
        }
        else
        {
            Debug.Log("Nao eh carta");
            return;
        }
        
        
    }
}
