using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InBuildDebugger : MonoBehaviour
{
    public static InBuildDebugger instance;
    public TextMeshProUGUI logText;
    public Image logBackground;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Log(Transform location, CardData card2)
    {
        logBackground.enabled = true;
        logText.gameObject.SetActive(true);
        logText.text += "Location: " + location.name + "\n" + AdvancedLogger.Log(card2) + "\n";
    }
    public void Log(CardData card)
    {
        logBackground.enabled = true;
        logText.gameObject.SetActive(true);
        logText.text += AdvancedLogger.Log(card) + "\n";
    }
}