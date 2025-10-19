using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using System.Collections;

public class CardInstance : MonoBehaviour, IGameEntity, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public enum CardMode
    {
        InHand = 0,
        InPlay = 1,
        Dormant = 2,
        Dead = 3
    }

    [Header("Card Info")]
    public CardData cardData;
    

    [Header("Display Info")]
    [SerializeField] TextMeshProUGUI nameComponent;
    [SerializeField] Image descriptionImage;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI healthComponent;
    [SerializeField] TextMeshProUGUI attackComponent;
    [SerializeField] Image cardArtComponent;
    [SerializeField] Image backgroundComponent;


    HealthSystemTemplate healthSystem, attackSystem;
    public CardMode Mode { get; private set; }
    int turnsToSleep = 0;

    // ------------------------------------------------------------------------------------------GameEntity Stuff
    public bool IsPlayer1 => isPlayer1;
    public string Id => id;

    public GameObject GameObject => gameObject;

    string id;
    bool isPlayer1;
    // ------------------------------------------------------------------------------------------GameEntity Stuff

    // ----------------------------------- Draggable stuff
    public GameObject[] slotObjects;
    public float snapRange = 1f;
    private static List<RectTransform> slots;
    private Vector3 velocity = Vector3.zero;
    private Vector3 dragTargetPosition;
    public float dragSmoothSpeed = 10f;
    private static bool slotsInitialized = false;
    bool isBeingDragged;
    Vector3 dragOffset;
    bool canBeSelected;
    GameObject targetPrefab;
    Vector3 initialPosition;

    // --------------------- Special visuals
    IEnumerator descriptionCoroutine;


    public void SetupCardInstance(CardData data, bool isPlayer1)
    {
        // Coloca a informacao da carta na instancia
        cardData = data;

        // Prepara componentes nao visuais
        healthSystem = new HealthSystemTemplate(cardData.health);
        attackSystem = new HealthSystemTemplate(cardData.attack);
        Mode = CardMode.InHand;
        this.isPlayer1 = isPlayer1;
        id = Guid.NewGuid().ToString();

        // Prepara textos
        nameComponent.text = cardData.cardName;
        descriptionText.text = cardData.cardDescription;
        ChangeHealthComponent();
        ChangeAttackComponent();

        // Prepara artes
        cardArtComponent.sprite = cardData.cardArt;
        backgroundComponent.sprite = cardData.backgroundArt;
        descriptionImage.gameObject.SetActive(false);

        // Prepara corotinas
        descriptionCoroutine = DescriptionAppearTimer();

        
    }

    #region HealthMethods

    public void TakeDamage(int amount)
    {
        healthSystem.TakeDamage(amount);
        ChangeHealthComponent();
    }

    public void Heal(int amount)
    {
        healthSystem.Heal(amount);
        ChangeHealthComponent();
    }

    public int GetCurrentHealth()
    {
        return healthSystem.CurrentHealth;
    }

    private void ChangeHealthComponent()
    {
        // Altera o valor do componente
        healthComponent.text = healthSystem.CurrentHealth.ToString();

        if (healthSystem.IsDamaged())
        {
            healthComponent.color = Color.red;
        }
        else if (healthSystem.WasBuffed)
        {
            healthComponent.color = Color.green;
        }
        else
        {
            healthComponent.color = Color.white;
        }
    }
    #endregion

    public void FinishedPlayCard()
    {
        EnqueueEffects(TimeToActivate.OnReveal);
        // TO DO: Put card down
    }
    public void Attack()
    {
        EnqueueEffects(TimeToActivate.OnAttack);
        // TO DO: Attack stuff
    }

    private void EnqueueEffects(TimeToActivate state)
    {
        List<EffectActivationData> effects = cardData.GetEffectsByTime(state);

        foreach (EffectActivationData effect in effects)
        {
            GameAction res = new GameAction(this, effect);
            EffectHandler.Instance.EnqueueEffect(res);
        }        
    }

    public int GetCurrentAttack()
    {
        return attackSystem.CurrentHealth;
    }

    private void ChangeAttackComponent()
    {
        // Altera o valor do componente
        attackComponent.text = attackSystem.CurrentHealth.ToString();

        if (attackSystem.IsDamaged())
        {
            attackComponent.color = Color.red;
        }
        else if (attackSystem.WasBuffed)
        {
            attackComponent.color = Color.green;
        }
        else
        {
            attackComponent.color = Color.white;
        }
    }

    public bool Buff(IGameEntity source, Stat stat, int amount)
    {
        if ((stat & Stat.Health) != 0)
        {
            healthSystem.BuffMaxHealth(source, amount, true);
            return true;
        }
        if ((stat & Stat.Attack) != 0)
        {
            attackSystem.BuffMaxHealth(source, amount, true);
            return true;
        }

        return false;
    }

    public bool TryUndoBuff(IGameEntity source)
    {
        if (healthSystem.TryUndoBuff(source) || attackSystem.TryUndoBuff(source))
        {
            ChangeHealthComponent();
            ChangeAttackComponent();
            return true;
        }

        return false;
    }

    public bool MakeInert(int amount)
    {
        turnsToSleep += amount;
        return true;
    }

    public bool TryRevive()
    {
        // TO DO: Mecanica de reviver
        throw new NotImplementedException();
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

        InitializeSlots();

        RectTransform closestSlot = null;
        float closestDistance = float.MaxValue;

        Vector3 cardPos = transform.position;

        foreach (RectTransform slot in slots)
        {
            float dist = Vector3.Distance(cardPos, slot.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestSlot = slot;
            }
        }

        

        if (closestSlot != null && closestDistance < snapRange)
        {
            transform.SetParent(closestSlot);
            transform.position = new Vector3(
                closestSlot.position.x,
                closestSlot.position.y,
                transform.position.z
            );
            ConfirmPlay(); 
        }
        else
        {
            StartCoroutine(Return());
        }
    }
    
    private void InitializeSlots()
    {
        if (slotsInitialized) return;

        slots = new List<RectTransform>();

        slotObjects = GameObject.FindGameObjectsWithTag("Slot");

        foreach (GameObject go in slotObjects)
        {
            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt != null)
            {
                slots.Add(rt);
            }
        }

        slotsInitialized = true;
    }

    IEnumerator Return()
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
        if (Mode == CardMode.InHand)
        {
            initialPosition = transform.position;
            isBeingDragged = true;

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

    public void ConfirmPlay()
    {
        Mode = CardMode.InPlay;

        List<EffectActivationData> effects = cardData.GetEffectsByTime(TimeToActivate.OnPlay);

        foreach (EffectActivationData effect in effects)
        {
            GameAction res = new GameAction(this, effect);
            RewindableActionsController.Instance.CardPlayed(res);
        }
    }

    public void PossibleTargetToClick()
    {
        canBeSelected = true;
    }

    public void SelectionOver()
    {
        canBeSelected = false;
        if (targetPrefab != null)
        {
            Destroy(targetPrefab);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canBeSelected)
        {            
            OnPointerExit(eventData);

            // Pega uma copia do alvo pra ficar la na carta
            targetPrefab = Instantiate(TargetSelector.Instance.Selected(this), Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y)), Quaternion.identity, transform);
            canBeSelected = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // TODO : Animation for hovering
        StartCoroutine(descriptionCoroutine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // TODO: Stop animation for hovering
        StopCoroutine(descriptionCoroutine);
        descriptionImage.gameObject.SetActive(false);
    }

    IEnumerator DescriptionAppearTimer()
    {
        yield return new WaitForSeconds(1f);
        descriptionImage.gameObject.SetActive(true);    // TO DO: Adicionar easing
    }
}
