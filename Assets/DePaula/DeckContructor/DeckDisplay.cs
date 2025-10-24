using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DeckDisplay : MonoBehaviour
{
    [Header("UI refs")]
    public TextMeshProUGUI deckNameText;
    public Button loadButton;
    public Button deleteButton; // optional; can be left null

    /// <summary>
    /// Inicializa a visual do deck display.
    /// onLoad é chamado quando o usuário clica para carregar.
    /// onDelete é chamado quando o usuário clica para deletar (se deleteButton existir).
    /// </summary>
    public virtual void Initialize(string deckName, Action onLoad, Action onDelete = null, string dto = null)
    {
        if (deckNameText != null) deckNameText.text = deckName ?? "(untitled)";

        if (loadButton != null)
        {
            loadButton.onClick.RemoveAllListeners();
            if (onLoad != null) loadButton.onClick.AddListener(() => onLoad());
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            if (onDelete != null)
                deleteButton.onClick.AddListener(() => onDelete());
            else
                deleteButton.gameObject.SetActive(false);
        }
    }
}
