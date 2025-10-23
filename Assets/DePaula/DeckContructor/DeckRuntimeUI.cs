// DeckRuntimeUI.cs
// Runtime UI for creating/saving decks. Updated to:
// - Bootstrap library automatically from Addressables settings (Editor helper) or by Addressables label at runtime
// - Use a visual CardDisplay prefab (instead of a simple Button) to represent each card in library and deck lists
// - Exposes AddCard(CardData card) and AddCardByAddressableKeyAsync(string key)
// - Keeps loaded Addressables handles so you can release them when appropriate
//
// Place in: Assets/Scripts/UI/DeckRuntimeUI.cs
// Requires: JM_DeckBase, CardData, DeckPersistence, DeckMapper

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;
using TMPro;

//#if ENABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
//#endif

#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings;
using UnityEditor;
using UnityEngine.Events;
using UnityEditor.AddressableAssets;
#endif

public class DeckRuntimeUI : MonoBehaviour
{
    #region Singleton
    public static DeckRuntimeUI Instance { get; private set; }
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

    [Header("UI References")]
    public TMP_InputField deckNameInput;
    public Button createDeckButton;
    public Button saveDeckButton;
    public Transform deckListContent;          // parent for cards in the current deck
    public Transform libraryContent;           // parent for library cards

    [Header("Prefabs")]
    public GameObject cardDisplayPrefab;       // Prefab that implements a visual display for a CardData (see CardDisplay example below)

    [Header("Options")]
    public bool allowDuplicates = true;

    // runtime state
    JM_DeckBase currentDeck;

    // cached UI entries
    List<GameObject> currentDeckUIEntries = new List<GameObject>();
    List<GameObject> libraryUIEntries = new List<GameObject>();

    // store addressable handles so they can be released later if desired
//#if ENABLE_ADDRESSABLES
    List<AsyncOperationHandle> addressableHandles = new List<AsyncOperationHandle>();
//#endif

    void Start()
    {
        if (createDeckButton != null) createDeckButton.onClick.AddListener(CreateNewDeckFromUI);
        if (saveDeckButton != null) saveDeckButton.onClick.AddListener(() => SaveCurrentDeck());

        //await PopulateLibraryFromAddressableKeysAsync(GatherCardDataKeysFromAddressableSettings());
    }

    // ---------------- Public API ----------------
    public void CreateNewDeckFromUI()
    {
        CreateNewDeck(null);
    }

    public void CreateNewDeck(string name)
    {
        deckNameInput.gameObject.SetActive(true);
        if (name == null)
        {
            deckNameInput.text = "New Deck";
        }
        else
        {
            deckNameInput.text = name;
        }

        currentDeck = ScriptableObject.CreateInstance<JM_DeckBase>();
        currentDeck.name = string.IsNullOrEmpty(name) ? "Deck_" + System.Guid.NewGuid().ToString() : name;
        currentDeck.allCards = new List<CardData>();
        RefreshDeckUI();
    }

    // The function you asked for: add card by CardData reference
    public void AddCard(CardData card)
    {
        if (card == null) return;
        if (currentDeck == null) CreateNewDeck("Deck_Auto");

        if (!allowDuplicates && currentDeck.allCards.Exists(c => c == card))
        {
            Debug.Log("Card already in deck and duplicates not allowed: " + card.cardName);
            return;
        }

        currentDeck.allCards.Add(card);
        AddDeckCardUIEntry(card);
    }

    public void RemoveCard(CardData card)
    {
        if (card == null || currentDeck == null) return;
        if (currentDeck.allCards.Remove(card)) RefreshDeckUI();
    }

    public void SaveCurrentDeck()
    {
        if (currentDeck == null)
        {
            Debug.LogWarning("No deck to save. Create one first.");
            return;
        }

        if (deckNameInput != null && !string.IsNullOrEmpty(deckNameInput.text)) currentDeck.name = deckNameInput.text;

        try
        {
            string fileName = DeckPersistence.SaveDeck(currentDeck);
            Debug.Log($"Deck saved: {fileName}");
            if (DeckListManager.Instance != null) DeckListManager.Instance.RefreshList();   // Adicionado para deixar mais dinamico
            deckNameInput.gameObject.SetActive(false);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to save deck: " + ex.Message);
        }
    }

    // ---------------- Addressable helper (new) ----------------

    /// <summary>
    /// Loads a CardData by addressable key (or GUID/key depending on how you indexed Addressables) and adds it to the current deck.
    /// If Addressables are not available, attempts to fallback to Resources by path/name.
    /// This method awaits the load and returns when the card has been added (or failed).
    /// </summary>
    public async Task AddCardByAddressableKeyAsync(string key)
    {
        if (string.IsNullOrEmpty(key)) return;

//#if ENABLE_ADDRESSABLES
        try
        {
            var handle = Addressables.LoadAssetAsync<CardData>(key);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                // Keep handle to control lifetime (caller can call ReleaseLoadedAddressables later)
                addressableHandles.Add(handle);

                AddCard(handle.Result);
                return;
            }
            else
            {
                Debug.LogWarning($"AddCardByAddressableKeyAsync: Addressables failed to load key {key} (status: {handle.Status}). Trying Resources fallback.");
                // release if failed
                if (handle.IsValid()) Addressables.Release(handle);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("AddCardByAddressableKeyAsync: Addressables exception: " + ex.Message + ". Trying Resources fallback.");
        }
//#endif

        // Fallback to Resources (synchronous)
        // Try direct path
        var res = Resources.Load<CardData>(key);
        if (res != null)
        {
            AddCard(res);
            return;
        }

        // Try search all CardData resources by name
        var all = Resources.LoadAll<CardData>("");
        foreach (var c in all)
        {
            if (c == null) continue;
            if (string.Equals(c.name, key, System.StringComparison.OrdinalIgnoreCase) || string.Equals(c.cardName, key, System.StringComparison.OrdinalIgnoreCase))
            {
                AddCard(c);
                return;
            }
        }

        Debug.LogWarning($"AddCardByAddressableKeyAsync: Could not resolve CardData for key '{key}' via Addressables or Resources.");
    }

    /// <summary>
    /// Releases all stored Addressables handles that were kept by AddCardByAddressableKeyAsync/Populate methods.
    /// Call this when you no longer need the loaded assets (e.g., leaving scene).
    /// </summary>
    public void ReleaseLoadedAddressables()
    {
//#if ENABLE_ADDRESSABLES
        if (addressableHandles == null || addressableHandles.Count == 0) return;
        foreach (var h in addressableHandles)
        {
            if (h.IsValid()) Addressables.Release(h);
        }
        addressableHandles.Clear();
//#else
        //Debug.LogWarning("ReleaseLoadedAddressables called but Addressables not enabled in this build.");
//#endif
    }

    // ---------------- Library bootstrap helpers ----------------

    // Editor-only: scan Addressables settings and gather all entries whose asset is of type CardData.
    // This populates the library by addressable key (address field if present, otherwise GUID).
    public void BootstrapLibraryFromAddressablesEditor()
    {
#if UNITY_EDITOR
        var keys = GatherCardDataKeysFromAddressableSettings();
        if (keys == null || keys.Count == 0)
        {
            Debug.LogWarning("No CardData addressable entries found in Addressables settings.");
            return;
        }

        // start async population (fire-and-forget is fine here)
        _ = PopulateLibraryFromAddressableKeysAsync(keys);
#else
        Debug.LogWarning("BootstrapLibraryFromAddressablesEditor is Editor-only.");
#endif
    }

//#if ENABLE_ADDRESSABLES
    // Runtime: bootstrap library by Addressables label (requires you to tag CardData entries with the label)
    public async Task BootstrapLibraryFromAddressablesLabelAsync(string label)
    {
        if (string.IsNullOrEmpty(label)) { Debug.LogWarning("Label is empty"); return; }

        var locHandle = Addressables.LoadResourceLocationsAsync(label, typeof(CardData));
        await locHandle.Task;

        if (locHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var keys = locHandle.Result.Select(loc => loc.PrimaryKey).Distinct().ToList();
            await PopulateLibraryFromAddressableKeysAsync(keys);
        }
        else
        {
            Debug.LogWarning($"Failed to load resource locations for label {label}: {locHandle.Status}");
        }

        Addressables.Release(locHandle);
    }
//#endif

    // Populate library by a list of addressable keys (async). Works with Addressables.LoadAssetAsync per key.
    public async Task PopulateLibraryFromAddressableKeysAsync(List<string> keys)
    {
        ClearLibraryUI();
        if (keys == null || keys.Count == 0)
        {
            Debug.LogWarning("No keys found!");
            return;
        }

//#if ENABLE_ADDRESSABLES
        foreach (var key in keys)
        {
            var handle = Addressables.LoadAssetAsync<CardData>(key);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                // keep handle
                addressableHandles.Add(handle);
                CreateLibraryEntry(handle.Result);
            }
            else
            {
                Debug.LogWarning($"Failed to load CardData with key {key}: {handle.Status}");
                if (handle.IsValid()) Addressables.Release(handle);
            }
        }
//#else
//        Debug.LogWarning("Addressables not enabled in this build. Cannot populate from addressable keys.");
//#endif
    }

    // Fallback: populate library from Resources (synchronous)
    public void PopulateLibraryFromResources(string resourcesPath = "")
    {
        ClearLibraryUI();
        CardData[] all = string.IsNullOrEmpty(resourcesPath) ? Resources.LoadAll<CardData>("") : Resources.LoadAll<CardData>(resourcesPath);
        foreach (var c in all) CreateLibraryEntry(c);
    }

    // ---------------- UI helpers (CardDisplay-based) ----------------

    void RefreshDeckUI()
    {
        foreach (var go in currentDeckUIEntries) Destroy(go);
        currentDeckUIEntries.Clear();

        if (currentDeck == null || currentDeck.allCards == null) return;

        foreach (var card in currentDeck.allCards) AddDeckCardUIEntry(card);
    }

    void AddDeckCardUIEntry(CardData card)
    {
        if (cardDisplayPrefab == null || deckListContent == null) return;
        var go = Instantiate(cardDisplayPrefab, deckListContent, false);

        var display = go.GetComponent<CardDisplay>();
        if (display != null)
        {
            // we pass a lambda that removes this card when clicked in the deck list
            display.Initialize(card, () => { RemoveCard(card); });
        }
        else
        {
            // fallback: try to set basic UI (if prefab is simple button/text)
            var btn = go.GetComponent<Button>();
            var txt = go.GetComponentInChildren<Text>();
            if (txt != null) txt.text = card != null ? card.cardName : "NULL";
            if (btn != null) { btn.onClick.RemoveAllListeners(); btn.onClick.AddListener(() => RemoveCard(card)); }
        }

        currentDeckUIEntries.Add(go);
    }

    void CreateLibraryEntry(CardData card)
    {
        if (cardDisplayPrefab == null || libraryContent == null) return;
        var go = Instantiate(cardDisplayPrefab, libraryContent, false);

        var display = go.GetComponent<CardDisplay>();
        if (display != null)
        {
            
            // clicking a library entry will add the card to the current deck
            display.Initialize(card, () => { AddCard(card); });
#if UNITY_EDITOR
            EditorHelpers.SetCardDataAndSave(display, card);
#endif
        }
        else
        {
            var btn = go.GetComponent<Button>();
            var txt = go.GetComponentInChildren<Text>();
            if (txt != null) txt.text = card != null ? card.cardName : "NULL";
            if (btn != null)
            {
                CardData captured = card;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => AddCard(captured));
            }
        }

        libraryUIEntries.Add(go);
    }

    void ClearLibraryUI()
    {
        foreach (var g in libraryUIEntries)
        {
            if (Application.isPlaying)
                Destroy(g);
            else
                DestroyImmediate(g);
        }
        libraryUIEntries.Clear();
    }

    // ---------------- Editor-only Addressables settings scanner ----------------
#if UNITY_EDITOR
    // Scans AddressableAssetSettings and returns a list of keys (entry.address or GUID) for entries whose asset is a CardData
    public static List<string> GatherCardDataKeysFromAddressableSettings()
    {
        var keys = new List<string>();
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return keys;

        foreach (var group in settings.groups)
        {
            if (group == null) continue;
            foreach (var entry in group.entries)
            {
                if (entry == null) continue;
                string assetPath = entry.AssetPath;
                if (string.IsNullOrEmpty(assetPath)) continue;

                var cd = AssetDatabase.LoadAssetAtPath<CardData>(assetPath);
                if (cd != null)
                {
                    string key = !string.IsNullOrEmpty(entry.address) ? entry.address : AssetDatabase.AssetPathToGUID(assetPath);
                    keys.Add(key);
                }
            }
        }

        return keys.Distinct().ToList();
    }
#endif
}
