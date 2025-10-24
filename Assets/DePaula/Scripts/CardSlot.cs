using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public CardInstance CardInstance { get; set; }

    public bool isPlayer1Slot = false;
    public bool empty = true;
    public List<SlotSavedAction> Actions { get; private set; } = new List<SlotSavedAction>();

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

                TryActivateEffect();

                
                    
                handUI.UpdateHandUI();
            }
        }
        else
        {
            Debug.Log("Nao eh carta");
            return;
        }
        
        
    }

    public bool TryActivateEffect()
    {
        if (empty) return false;

        foreach (var act in Actions)
        {
            act.Execute();
        }

        Actions.Clear();
        return true;
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
