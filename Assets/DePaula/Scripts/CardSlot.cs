using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public CardInstance CardInstance { get; set; }

    public bool isPlayer1Slot = false;
    public bool empty = true;

    public async void OnDrop(PointerEventData eventData)
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
                PutCardInSlot(cardInstance);

                cardInstance.dropped = true;
                
                // Animation to play when card played

                await CardInstance.ConfirmPlay(this);

                
                    
                handUI.UpdateHandUI();
            }
        }
        else
        {
            Debug.Log("Nao eh carta");
            return;
        }
        
        
    }

    public void PutCardInSlot(CardInstance cardInstance)
    {
        Debug.Log("Put hihi!");
        cardInstance.transform.SetParent(transform, false);

        cardInstance.transform.position = transform.position;   // por enquanto, so da snap pra posicao

        empty = false;
        CardInstance = cardInstance;

        //return true;
    }
}
