using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public UnityAction OnCardPlayed; // Event to notify when a card is played in this slot
    public CardInstance CardInstance { get; set; }

    public bool isPlayer1Slot = false;
    public bool empty = true;
    public List<SlotSavedAction> Actions { get; private set; } = new List<SlotSavedAction>();

    public async void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped " + eventData.pointerDrag.name);

        if (eventData.pointerDrag.TryGetComponent<DraggableComponent>(out DraggableComponent draggable))
        {
            CardInstance cardInstance = draggable.CardInstance;
            bool isPlayer1 = cardInstance.IsPlayer1 == isPlayer1Slot;
            
            if (!empty || !isPlayer1)
            {
                draggable.DenyDrop();
                Debug.Log("Slot is not empty!");
            }
            else
            {
                PutCardInSlot(cardInstance);
                draggable.ConfirmDropped();

                OnCardPlayed?.Invoke(); // Notify listeners that a card has been played in this slot

                await CardInstance.ConfirmPlay(this); // Feedback to card to activate its own effect
                TryActivateSlotEffect(); // Feedback to slot to activate its own effect

                

                GameManager.Instance.UpdateHandsUI();   // Updates the hand UI to reflect the card being played and removed from hand
            }
        }
        else
        {
            Debug.Log("Nao eh carta");
            return;
        }
        
        
    }

    public bool TryActivateSlotEffect()
    {
        if (empty) return false;

        foreach (var act in Actions)
        {
            act.Execute();
        }

        Actions.Clear();
        return true;
    }


    public void PutCardInSlot(CardInstance cardInstance, bool dontReveal = false)
    {
        //Debug.Log("Put hihi!");
        cardInstance.transform.SetParent(transform, false);

        cardInstance.transform.position = transform.position;   // por enquanto, so da snap pra posicao

        empty = false;
        CardInstance = cardInstance;
        CardInstance.CurrentSlot = gameObject;

        if (!dontReveal)
        {
            // Send event to Turn Controller to store card played in turn history
            GameManager.Instance.turnController.StoreCardToBeRevealedLater(CardInstance.gameObject);
        }
        

        //return true;
    }
}

[System.Serializable]
public class SlotSavedAction
{
    CardInstance cardInstance;
    CardSlot slot;
    EffectObject effect;
    int specialParam;
    int bonusParam;
    float chance;

    public SlotSavedAction(CardInstance source, CardSlot tg, EffectObject effect, int specialParam, int bonusParam, float chance)
    {
        cardInstance = source;
        slot = tg;
        this.effect = effect;
        this.specialParam = specialParam;
        this.bonusParam = bonusParam;
        this.chance = chance;
    }

    public void Execute()
    {
        if (Random.Range(0f, 1f) > chance) return;    // failed

        IGameEntity[] tgs = {slot.CardInstance};
        EffectHandler.Instance.ActivateEffectImmediatly(effect, cardInstance, tgs, specialParam, bonusParam);
    }
}
