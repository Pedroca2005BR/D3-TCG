using UnityEngine;
using DG.Tweening;

public class PulsatingComponent : MonoBehaviour
{
    public float pulsateScale = 1.2f;
    public float pulsateDuration = 0.5f;

    private void Start()
    {
        Pulsate();
    }

    private void Pulsate()
    {
        transform.DOScale(pulsateScale, pulsateDuration).SetLoops(-1, LoopType.Yoyo);
    }
}