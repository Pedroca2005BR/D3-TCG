using UnityEngine;

public class RotationEffect : MonoBehaviour
{
    [SerializeField] GameObject slot1;
    [SerializeField] GameObject slot2;
    private Vector3 position1;
    private Vector3 position2;

    void Start()
    {
        position1 = slot1.transform.position;
        position2 = slot2.transform.position;
    }

    private void OnEnable()
    {
        JM_TurnController.actualTurn += Rotation;
    }

    private void OnDisable()
    {
        JM_TurnController.actualTurn -= Rotation;
    }

    void Rotation(bool turn)
    {
        if(turn) {
            slot1.transform.position = position2;
            slot2.transform.position = position1;
        }
        else
        {
            slot1.transform.position = position1;
            slot2.transform.position = position2;
        }
    }
}
