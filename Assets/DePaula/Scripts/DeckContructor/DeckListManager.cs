using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.UI;

public class DeckListManager : MonoBehaviour
{
    #region Singleton
    public static DeckListManager Instance { get; private set; }
    // Singleton pattern
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion


    [Header("References")]
    public Transform contentParent;             // container (should have VerticalLayoutGroup + ContentSizeFitter)
    public GameObject deckDisplayPrefab;        // prefab with DeckDisplay component
    public DeckRuntimeUI deckRuntimeUI;        // reference to your DeckRuntimeUI instance
    public ConfirmationComponent confirmationComponent; // reference to a confirmation dialog component

    [Header("Options")]
    public bool showDeleteButton = true;        // if true, shows delete button on each DeckDisplay

    // internal list to clear UI entries
    List<GameObject> spawnedEntries = new List<GameObject>();

    void Start()
    {
        RefreshList();
    }

    /// <summary>
    /// Re-popula a lista baseada nos arquivos salvos (DeckPersistence.ListDeckFiles).
    /// </summary>
    public void RefreshList()
    {
        // clear current UI
        foreach (var go in spawnedEntries) Destroy(go);
        spawnedEntries.Clear();

        // get deck filenames
        var files = DeckPersistence.ListDeckFiles();

        foreach (var fileName in files)
        {
            var dto = DeckPersistence.ReadDeckDTO(fileName); // fileName is returned by ListDeckFiles
            string displayName = dto != null && !string.IsNullOrEmpty(dto.name) ? dto.name : Path.GetFileNameWithoutExtension(fileName);

            // instantiate
            var go = Instantiate(deckDisplayPrefab, contentParent, false);
            var disp = go.GetComponent<DeckDisplay>();
            if (disp != null)
            {
                // capture local var for closure
                string capturedFile = fileName;
                disp.Initialize(displayName,
                    onLoad: () => { TryLoadDeck(capturedFile); },
                    onDelete: showDeleteButton ? () => { TryDeleteDeck(capturedFile); }
                : (System.Action)null,
                    fileName
                );

                // optionally hide delete button if not desired
                if (!showDeleteButton && disp.deleteButton != null) disp.deleteButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("DeckListManager: prefab does not have DeckDisplay component.");
            }

            spawnedEntries.Add(go);
        }
    }

    void TryDeleteDeck(string deckFileName)
    {
        confirmationComponent.ShowConfirmation(
            "Tem certeza que deseja deletar este deck? Esta ação não pode ser desfeita.",
            onConfirmEvent: () => { DeleteDeckFileAndRefresh(deckFileName); }
        );
    }

    void TryLoadDeck(string deckFileName)
    {
        if (deckRuntimeUI == null)
        {
            Debug.LogWarning("DeckListManager: deckRuntimeUI not assigned.");
            return;
        }
        if (!deckRuntimeUI.IsCurrentDeckSaved())
        {
            // Show confirmation dialog before loading new deck
            confirmationComponent.ShowConfirmation(
                "Seu deck atual não foi salvo. Tem certeza que deseja carregar um novo deck?",
                onConfirmEvent: () => { _ = LoadDeckAndShowAsync(deckFileName); }
            );
        }
        else
        {
            // No unsaved changes, load directly
            _ = LoadDeckAndShowAsync(deckFileName);
        }
    }

    /// <summary>
    /// Async load: limpa o DeckRuntimeUI e carrega o deck (via addressable keys).
    /// </summary>
    async Task LoadDeckAndShowAsync(string deckFileName)
    {
        if (deckRuntimeUI == null)
        {
            Debug.LogWarning("DeckListManager: deckRuntimeUI not assigned.");
            return;
        }

        // Read DTO
        var dto = DeckPersistence.ReadDeckDTO(deckFileName);
        if (dto == null)
        {
            Debug.LogWarning("DeckListManager: could not read DTO for " + deckFileName);
            return;
        }

        // Create a new deck in runtime UI and set its name
        string deckName = !string.IsNullOrEmpty(dto.name) ? dto.name : Path.GetFileNameWithoutExtension(deckFileName);
        deckRuntimeUI.CreateNewDeck(deckName);

        // If DTO has cardKeys, load them via AddCardByAddressableKeyAsync (DeckRuntimeUI handles addressables)
        if (dto.cardKeys != null)
        {
            foreach (var key in dto.cardKeys)
            {
                if (string.IsNullOrEmpty(key)) continue;
                try
                {
                    // AddCardByAddressableKeyAsync handles addressables; await to keep order
                    await deckRuntimeUI.AddCardByAddressableKeyAsync(key);
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"DeckListManager: failed to add card key {key}: {ex.Message}");
                }
            }
        }

        deckRuntimeUI.LoadComplete();
    }

    void DeleteDeckFileAndRefresh(string deckFileName)
    {
        bool ok = DeckPersistence.DeleteDeck(deckFileName);
        if (ok)
        {
            RefreshList();
        }
        else
        {
            Debug.LogWarning("DeckListManager: failed to delete " + deckFileName);
        }
    }
}
