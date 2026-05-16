using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class RotateComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum Activator
    {
        External = -1,
        Hover = 0,
        Click = 1
    }

    [SerializeField] private Vector3 desiredRotation;
    [SerializeField] private int loopAmount = -1;
    [SerializeField] private float rotationDuration = 1f;
    [SerializeField] private Activator activator;
    private Vector3 initialRotation;



    private void Start()
    {
        initialRotation = transform.rotation.eulerAngles;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (activator == Activator.Hover)
            Rotate();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (activator == Activator.Hover)
        {
            transform.DOKill();
            transform.DORotate(initialRotation, 1f).SetEase(Ease.OutBounce);
        }
    }

    public void Rotate()
    {
        transform.DORotate(desiredRotation, rotationDuration, RotateMode.FastBeyond360).SetLoops(loopAmount, LoopType.Incremental).SetEase(Ease.OutBounce);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (activator == Activator.Click)
            Rotate();
    }


}
