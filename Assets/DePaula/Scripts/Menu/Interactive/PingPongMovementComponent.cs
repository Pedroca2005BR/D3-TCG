using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PingPongMovementComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector3 desiredMovement;
    [SerializeField] private float duration;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Move();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOMove(initialPosition, duration).SetEase(Ease.OutBounce);
    }

    private void Move()
    {
        transform.DOMove(transform.position + desiredMovement, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InCubic);
    }
}
