using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorChangerComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum Activator
    {
        External = -1,
        Hover = 0,
        Click = 1
    }

    private Image targetRenderer;
    private TextMeshProUGUI targetText;
    [SerializeField] private int loopAmount = -1;
    [SerializeField] private float colorChangeDuration = 1f;
    [SerializeField] private Color targetColor = Color.white;
    [SerializeField] private Activator activator;
    private Color initialImageColor;
    private Color initialTextColor;
    Tweener colorTween;


    private void Start()
    {
        targetRenderer = GetComponent<Image>();
        targetText = GetComponent<TextMeshProUGUI>();
        if (targetRenderer != null)
            initialImageColor = targetRenderer.color;
        if (targetText != null)
        {
            initialTextColor = targetText.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (activator == Activator.Hover)
            ChangeColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (activator == Activator.Hover)
        {
            if (targetRenderer != null)
            {
                targetRenderer.DOKill();
                targetRenderer.DOColor(initialImageColor, 1f).SetEase(Ease.Linear);
            }
            if (targetText != null)
            {
                targetText.DOKill();
                targetText.DOColor(initialTextColor, 1f).SetEase(Ease.Linear);
            }
        }
    }

    public void ChangeColor()
    {
        if (colorTween != null && colorTween.IsPlaying())
        {
            return;
        }
        if (targetRenderer != null)
            colorTween = targetRenderer.DOColor(targetColor, colorChangeDuration).SetLoops(loopAmount, LoopType.Yoyo).SetEase(Ease.Linear).OnComplete(() => targetRenderer.DOColor(initialImageColor, 0.1f).SetEase(Ease.Linear));
        if (targetText != null)
            colorTween = targetText.DOColor(targetColor, colorChangeDuration).SetLoops(loopAmount, LoopType.Yoyo).SetEase(Ease.Linear).OnComplete(() => targetText.DOColor(initialTextColor, 0.1f).SetEase(Ease.Linear));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (activator == Activator.Click)
            ChangeColor();
    }
}
