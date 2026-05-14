using TMPro;
using UnityEngine;

public class TutorialText : TutorialObjectBase
{
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField][TextArea] private string text;

    public override void StartStep()
    {
        gameObject.SetActive(true);
        textMesh.text = text;
    }
}
