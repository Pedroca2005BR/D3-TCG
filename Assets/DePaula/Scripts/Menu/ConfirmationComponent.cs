using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationComponent : MonoBehaviour
{
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public void ShowConfirmation(string message, UnityAction onConfirmEvent)
    {
        confirmationText.text = message;
        confirmationPanel.SetActive(true);

        // Clear previous listeners to avoid stacking them if the confirmation is shown multiple times
        // After clearing, add the new listeners for hiding the confirmation and executing the provided event
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(HideConfirmation);
        confirmButton.onClick.AddListener(onConfirmEvent);

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(HideConfirmation);
    }

    public void ShowConfirmation(string message, UnityAction onConfirmEvent, UnityAction onCancelEvent)
    {
        confirmationText.text = message;
        confirmationPanel.SetActive(true);

        // Clear previous listeners to avoid stacking them if the confirmation is shown multiple times
        // After clearing, add the new listeners for hiding the confirmation and executing the provided event
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(HideConfirmation);
        confirmButton.onClick.AddListener(onConfirmEvent);

        // Cancel button logic
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(HideConfirmation);
        cancelButton.onClick.AddListener(onCancelEvent);
    }

    public void HideConfirmation()
    {
        confirmationPanel.SetActive(false);
    }
}
