using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PulsatingComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public float pulsateScale = 1.2f;
    public float pulsateDuration = 0.5f;
    public enum Activator
    {
        External = -1,
        Hover = 0,
        Click = 1,
        OnStart = 2
    }

    [SerializeField] private int loopAmount = -1;
    [SerializeField] private Activator activator;
    private Vector3 initialScale;
    Tweener pulseTween;



    private void Start()
    {
        initialScale = transform.localScale;
        if (activator == Activator.OnStart)
        {
            Pulse();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (activator == Activator.Hover)
            Pulse();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (activator == Activator.Hover)
        {
            transform.DOKill();
            transform.DOScale(initialScale, 1f).SetEase(Ease.Linear);
        }
    }

    public void Pulse()
    {
        if (pulseTween != null && pulseTween.IsPlaying())
        {
            return;
        }

        pulseTween = transform.DOScale(pulsateScale, pulsateDuration).SetLoops(loopAmount, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (activator == Activator.Click)
            Pulse();
    }
}