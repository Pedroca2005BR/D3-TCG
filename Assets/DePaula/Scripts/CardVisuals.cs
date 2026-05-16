using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardVisuals : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Sides")]
    [SerializeField] GameObject frontSide;
    [SerializeField] GameObject backSide;


    [Header("Display Info")]
    [SerializeField] TextMeshProUGUI nameComponent;
    [SerializeField] GameObject descriptionImage;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] TextMeshProUGUI healthComponent;
    [SerializeField] TextMeshProUGUI attackComponent;
    [SerializeField] Image cardArtComponent;
    [SerializeField] Image backgroundComponent;

    [Header("Select Effect")]
    private int originalPos;
    //[SerializeField] private float verticalMoveAmount = 0.3f;
    [SerializeField] private float moveTime = 0.1f;
    [Range(0f, 2f), SerializeField] private float scaleAmount = 1.1f;
    //private Vector3 startPos;
    private Vector3 startScale;


    [Header("Effect Icons")]
    [SerializeField] GameObject AtkBuff;
    [SerializeField] GameObject AtkDebuff;
    [SerializeField] GameObject HPBuff;
    [SerializeField] GameObject HPDebuff;
    [SerializeField] GameObject inertEffect;

    // --------------------- Special visuals
    IEnumerator descriptionCoroutine;
    Tween shakeTween;

    public void Setup(CardData cardData, HealthSystemTemplate healthSystem, HealthSystemTemplate attackSystem)
    {
        // Prepara textos
        nameComponent.text = cardData.cardName;
        descriptionText.text = cardData.cardDescription;
        UpdateHealthUI(healthSystem);
        UpdateAttackUI(attackSystem);

        // Prepara artes
        cardArtComponent.sprite = cardData.cardArt;
        backgroundComponent.sprite = cardData.backgroundArt;
        descriptionImage.SetActive(false);
        AtkBuff.SetActive(false);
        AtkDebuff.SetActive(false);
        HPBuff.SetActive(false);
        HPDebuff.SetActive(false);
        inertEffect.SetActive(false);

        // Prepara corotinas
        descriptionCoroutine = DescriptionAppearTimer();
    }

    #region UI Update
    public void UpdateHealthUI(HealthSystemTemplate healthSystem)
    {
        // Altera o valor do componente
        healthComponent.text = healthSystem.CurrentHealth.ToString();


        if (healthSystem.CheckBuff(out bool good))
        {
            if (good)
            {
                HPBuff.SetActive(true);
                HPDebuff.SetActive(false);
                healthComponent.color = Color.green;
            }
            else
            {
                HPDebuff.SetActive(true);
                HPBuff.SetActive(false);
                healthComponent.color = Color.yellow;
            }
        }
        else
        {
            HPDebuff.SetActive(false);
            HPBuff.SetActive(false);
            healthComponent.color = Color.white;
        }

        // Dano tem preferencia
        if (healthSystem.IsDamaged())
        {
            healthComponent.color = Color.red;
        }
    }

    public void UpdateAttackUI(HealthSystemTemplate attackSystem)
    {
        // Altera o valor do componente
        attackComponent.text = attackSystem.CurrentHealth.ToString();


        if (attackSystem.CheckBuff(out bool good))
        {
            if (good)
            {
                AtkBuff.SetActive(true);
                AtkDebuff.SetActive(false);
                attackComponent.color = Color.green;
            }
            else
            {
                AtkDebuff.SetActive(true);
                AtkBuff.SetActive(false);
                attackComponent.color = Color.yellow;
            }
        }
        else
        {
            AtkDebuff.SetActive(false);
            AtkBuff.SetActive(false);
            attackComponent.color = Color.white;
        }
        // Dano tem preferencia
        if (attackSystem.IsDamaged())
        {
            attackComponent.color = Color.red;
        }
    }

    public void UpdateUIInertEffect(bool isActive)
    {
        inertEffect.SetActive(isActive);
    }

    #endregion

    #region Description
    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionCoroutine = DescriptionAppearTimer();
        StartCoroutine(descriptionCoroutine);

        if (shakeTween != null && shakeTween.IsPlaying())
        {
            return;
        }

        StartCoroutine(SelectionEffect(true));
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // TODO: Stop animation for hovering
        StartCoroutine(SelectionEffect(false));
        StopCoroutine(descriptionCoroutine);
        descriptionImage.SetActive(false);

    }

    IEnumerator DescriptionAppearTimer()
    {
        yield return new WaitForSeconds(1f);
        descriptionImage.SetActive(true);    // TO DO: Adicionar easing
        Debug.Log("Description!");
    }
    #endregion

    #region Card Flipping
    public void FlipCard(bool faceUp)
    {
        frontSide.SetActive(faceUp);
        backSide.SetActive(!faceUp);
    }
    #endregion

    #region Animations
    
    public IEnumerator RevealTime()
    {
        // TODO: Add reveal animation
        yield return new WaitForSeconds(0.2f);
    }

    #endregion

    #region Selection Effect

    private void Start()
    {
        //startPos = transform.position;
        startScale = transform.localScale;
    }

    private IEnumerator SelectionEffect (bool startAnimation)
    {
        //startPos = transform.position;
        //Vector3 endPosition;
        Vector3 endScale;
        originalPos = transform.GetSiblingIndex();

        float elapsedTime = 0f;

        while(elapsedTime <= moveTime)
        {
            elapsedTime += Time.deltaTime;

            if(startAnimation)
            {
                //endPosition = startPos + new Vector3(0f, verticalMoveAmount, 0f);
                transform.SetAsLastSibling();
                endScale = startScale * scaleAmount;
            }

            else
            {
                //endPosition = startPos;
                transform.SetSiblingIndex(originalPos);
                endScale = startScale;
            }

            //calculos de lerp

            //Vector3 lerpedPos = Vector3.Lerp(transform.position, endPosition, (elapsedTime/moveTime));
            Vector3 lerpedScale = Vector3.Lerp(transform.localScale, endScale, (elapsedTime/moveTime));

            //transform.position = lerpedPos;
            transform.localScale = lerpedScale;

            yield return null;
        }

    }

    public void Shake()
    {
        if (shakeTween != null && shakeTween.IsPlaying())
        {
            return;
        }

        shakeTween = transform.DOShakeRotation(0.5f, new Vector3(0, 0, 20), vibrato: 15);
    }

    #endregion
}
