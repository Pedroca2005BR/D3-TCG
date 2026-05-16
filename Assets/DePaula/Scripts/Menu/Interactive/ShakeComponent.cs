using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShakeComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum Activator
    {
        External = -1,
        Hover = 0,
        Click = 1
    }

    [SerializeField] private int loopAmount = -1;
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private float shakeStrength = 10f;
    [SerializeField] private Activator activator;
    private Vector3 initialPosition;
    Tweener shakeTween;



    private void Start()
    {
        initialPosition = transform.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (activator == Activator.Hover)
            Shake();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (activator == Activator.Hover)
        {
            transform.DOKill();
            transform.DOMove(initialPosition, 1f).SetEase(Ease.Linear);
        }
    }

    public void Shake()
    {
        if (shakeTween != null && shakeTween.IsPlaying())
        {
            return;
        }

        shakeTween = transform.DOShakePosition(shakeDuration, shakeStrength, vibrato: 10).SetLoops(loopAmount, LoopType.Incremental).SetEase(Ease.Linear).OnComplete(() => transform.DOMove(initialPosition, 0.1f).SetEase(Ease.Linear));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (activator == Activator.Click)
            Shake();
    }
}
