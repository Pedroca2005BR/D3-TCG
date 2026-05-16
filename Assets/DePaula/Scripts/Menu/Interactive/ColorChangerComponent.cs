using DG.Tweening;
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
    [SerializeField] private int loopAmount = -1;
    [SerializeField] private float colorChangeDuration = 1f;
    [SerializeField] private Color targetColor = Color.white;
    [SerializeField] private Activator activator;
    private Color initialColor;
    Tweener colorTween;


    private void Start()
    {
        targetRenderer = GetComponent<Image>();
        initialColor = targetRenderer.color;
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
            targetRenderer.DOKill();
            targetRenderer.DOColor(initialColor, 1f).SetEase(Ease.Linear);
        }
    }

    public void ChangeColor()
    {
        if (colorTween != null && colorTween.IsPlaying())
        {
            return;
        }

        colorTween = targetRenderer.DOColor(targetColor, colorChangeDuration).SetLoops(loopAmount, LoopType.Yoyo).SetEase(Ease.Linear).OnComplete(() => targetRenderer.DOColor(initialColor, 0.1f).SetEase(Ease.Linear));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (activator == Activator.Click)
            ChangeColor();
    }
}
