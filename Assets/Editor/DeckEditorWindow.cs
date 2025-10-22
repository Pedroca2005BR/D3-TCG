// DeckEditorWindow.cs
// Editor window to Save / Load / Delete Deck JSONs using the DeckMapper + DeckPersistence + Addressables workflow.
// Place in Assets/Editor/DeckEditorWindow.cs
// Requires the following to exist in the project (from previous steps):
// - JM_DeckBase (ScriptableObject for deck)
// - DeckDTO, DeckMapper, DeckPersistence (or equivalent functions shown earlier)
// - CardData scriptable objects with addressableKey/id filled (optional — this window can auto-fill them)
// Usage: Window appears at Tools > Decks > Deck Manager

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

// Addressables editor API
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class DeckEditorWindow : EditorWindow
{
    Vector2 leftScroll, rightScroll;
    List<string> deckFiles = new List<string>();
    int selectedIndex = -1;

    // cache
    DeckDTO selectedDto;
    JM_DeckBase loadedDeck;

    const string DecksFolderName = "decks"; // relative to persistentDataPath

    [MenuItem("Tools/Decks/Deck Manager")]
    public static void OpenWindow()
    {
        var w = GetWindow<DeckEditorWindow>("Deck Manager");
        w.minSize = new Vector2(700, 300);
        w.RefreshDeckFiles();
    }

    void OnEnable()
    {
        RefreshDeckFiles();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        DrawLeftPanel();
        DrawRightPanel();

        EditorGUILayout.EndHorizontal();
    }

    void DrawLeftPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(300));
        EditorGUILayout.LabelField("Saved Decks", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh")) RefreshDeckFiles();
        if (GUILayout.Button("Open Decks Folder")) OpenDecksFolder();
        EditorGUILayout.EndHorizontal();

        leftScroll = EditorGUILayout.BeginScrollView(leftScroll);
        if (deckFiles.Count == 0) EditorGUILayout.HelpBox("No decks saved yet.", MessageType.Info);

        for (int i = 0; i < deckFiles.Count; i++)
        {
            string name = Path.GetFileName(deckFiles[i]);
            GUIStyle style = (i == selectedIndex) ? EditorStyles.whiteLabel : EditorStyles.label;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(name, style))
            {
                selectedIndex = i;
                LoadSelectedDTO();
            }
            if (GUILayout.Button("X", GUILayout.Width(24)))
            {
                if (EditorUtility.DisplayDialog("Delete deck?", $"Delete {name}? This will remove the file from persistent data path.", "Delete", "Cancel"))
                {
                    DeleteDeckFile(deckFiles[i]);
                    RefreshDeckFiles();
                    selectedIndex = -1;
                    selectedDto = null;
                    loadedDeck = null;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Save selected JM_DeckBase as Deck JSON"))
        {
            SaveSelectedJMDeckBase();
            RefreshDeckFiles();
        }

        if (GUILayout.Button("Save selected JM_DeckBase as..."))
        {
            SaveSelectedJMDeckBaseAs();
            RefreshDeckFiles();
        }

        // New button: mark cards from selected JM_DeckBase as Addressable
        if (GUILayout.Button("Mark cards in selected JM_DeckBase as Addressable (GUID)"))
        {
            var so = Selection.activeObject as JM_DeckBase;
            if (so == null)
            {
                EditorUtility.DisplayDialog("Error", "Select a JM_DeckBase asset in Project window first.", "OK");
            }
            else
            {
                int count = MarkCardsAddressableFromDeck(so);
                EditorUtility.DisplayDialog("Addressables", $"Processed {count} CardData assets. Check Console for details.", "OK");
            }
        }

        EditorGUILayout.EndVertical();
    }

    void DrawRightPanel()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Deck Details", EditorStyles.boldLabel);

        if (selectedIndex < 0 || selectedIndex >= deckFiles.Count)
        {
            EditorGUILayout.HelpBox("Select a deck from the left or save a JM_DeckBase using the buttons.", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.LabelField("File:", Path.GetFileName(deckFiles[selectedIndex]));

        if (selectedDto == null)
        {
            EditorGUILayout.HelpBox("Failed to parse DTO. Press Refresh or re-save the deck.", MessageType.Warning);
        }
        else
        {
            rightScroll = EditorGUILayout.BeginScrollView(rightScroll);

            EditorGUILayout.LabelField("Deck ID:", selectedDto.id);
            EditorGUILayout.LabelField("Deck name:", selectedDto.name);
            EditorGUILayout.LabelField("Version:", selectedDto.version.ToString());

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Card Keys (addressable keys / ids)", EditorStyles.boldLabel);
            if (selectedDto.cardKeys != null && selectedDto.cardKeys.Count > 0)
            {
                for (int i = 0; i < selectedDto.cardKeys.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.SelectableLabel(selectedDto.cardKeys[i], GUILayout.Height(16));
                    if (GUILayout.Button("Ping", GUILayout.Width(40)))
                    {
                        PingCardAsset(selectedDto.cardKeys[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else EditorGUILayout.HelpBox("No cards in this deck.", MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load (Editor - AssetDatabase)"))
            {
                LoadDeckEditorMode(deckFiles[selectedIndex]);
            }
            if (GUILayout.Button("Load in Playmode (Addressables)"))
            {
                EditorUtility.DisplayDialog("Info", "This will only work in Playmode or in a built player where Addressables are built. Use the runtime loader or test via Playmode.", "OK");
            }
            EditorGUILayout.EndHorizontal();

            if (loadedDeck != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Loaded deck (Editor) - Cards:", EditorStyles.boldLabel);
                foreach (var c in loadedDeck.allCards)
                {
                    if (c == null) EditorGUILayout.LabelField("Missing CardData");
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(c.name);
                        if (GUILayout.Button("Select", GUILayout.Width(60)))
                        {
                            Selection.activeObject = c;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("Mark loaded deck cards as Addressable (GUID)"))
                {
                    int processed = MarkCardsAddressableFromLoadedDeck();
                    EditorUtility.DisplayDialog("Addressables", $"Processed {processed} CardData assets. Check Console for details.", "OK");
                }
            }

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndVertical();
    }

    // ---------------- helpers ----------------
    static string DecksFolderPath()
    {
        string folder = Path.Combine(Application.persistentDataPath, DecksFolderName);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        return folder;
    }

    void RefreshDeckFiles()
    {
        var folder = DecksFolderPath();
        deckFiles = Directory.GetFiles(folder, "*.json").ToList();
        selectedDto = null;
        loadedDeck = null;
        Repaint();
    }

    void OpenDecksFolder()
    {
        EditorUtility.RevealInFinder(DecksFolderPath());
    }

    void LoadSelectedDTO()
    {
        if (selectedIndex < 0 || selectedIndex >= deckFiles.Count) { selectedDto = null; return; }
        string json = File.ReadAllText(deckFiles[selectedIndex]);
        selectedDto = null;
        try { selectedDto = JsonUtility.FromJson<DeckDTO>(json); }
        catch { selectedDto = null; }
    }

    void DeleteDeckFile(string path)
    {
        if (File.Exists(path)) File.Delete(path);
    }

    void LoadDeckEditorMode(string filePath)
    {
        // Uses DeckPersistence.LoadDeckEditor (Editor-only) if available
        // If not available, attempt to read DTO and resolve assets by GUID using AssetDatabase
        string fileName = Path.GetFileName(filePath);
        if (typeof(DeckPersistence) != null)
        {
            try
            {
                // call the existing Editor loader if present
                loadedDeck = DeckPersistence.LoadDeckEditor(fileName);
                if (loadedDeck == null) EditorUtility.DisplayDialog("Load", "Deck loaded but empty or not found.", "OK");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("DeckPersistence.LoadDeckEditor failed: " + ex.Message + ". Falling back to internal resolver.");
                FallbackLoadEditor(filePath);
            }
        }
        else
        {
            FallbackLoadEditor(filePath);
        }
    }

    void FallbackLoadEditor(string filePath)
    {
        // read DTO and resolve card IDs as GUIDs via AssetDatabase
        string json = File.ReadAllText(filePath);
        DeckDTO dto = JsonUtility.FromJson<DeckDTO>(json);
        loadedDeck = ScriptableObject.CreateInstance<JM_DeckBase>();
        loadedDeck.allCards = new List<CardData>();

        if (dto.cardKeys != null)
        {
            foreach (var id in dto.cardKeys)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(id);
                if (string.IsNullOrEmpty(path)) // try by filename
                {
                    var guids = UnityEditor.AssetDatabase.FindAssets("t:CardData " + id);
                    if (guids != null && guids.Length > 0) path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                }

                if (!string.IsNullOrEmpty(path))
                {
                    var card = UnityEditor.AssetDatabase.LoadAssetAtPath<CardData>(path);
                    if (card != null) loadedDeck.allCards.Add(card);
                }
            }
        }

        Repaint();
    }

    void SaveSelectedJMDeckBase()
    {
        var so = Selection.activeObject as JM_DeckBase;
        if (so == null)
        {
            EditorUtility.DisplayDialog("Error", "Select a JM_DeckBase asset in Project window first.", "OK");
            return;
        }

        try
        {
            // Uses DeckPersistence.SaveDeck if available
            DeckPersistence.SaveDeck(so);
            EditorUtility.DisplayDialog("Saved", "Deck saved to persistent data path.", "OK");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to save deck via DeckPersistence: " + ex.Message);
            // Fallback: quick local save using Mapper + JsonUtility
            var dto = DeckMapper.ToDTO(so);
            if (string.IsNullOrEmpty(dto.id)) dto.id = System.Guid.NewGuid().ToString();
            string json = JsonUtility.ToJson(dto, true);
            string file = Path.Combine(DecksFolderPath(), dto.id + ".json");
            File.WriteAllText(file, json);
            EditorUtility.DisplayDialog("Saved (fallback)", "Deck saved to: " + file, "OK");
            RefreshDeckFiles();
        }
    }

    void SaveSelectedJMDeckBaseAs()
    {
        var so = Selection.activeObject as JM_DeckBase;
        if (so == null)
        {
            EditorUtility.DisplayDialog("Error", "Select a JM_DeckBase asset in Project window first.", "OK");
            return;
        }

        string defaultName = so.name + ".json";
        string folder = DecksFolderPath();
        string path = EditorUtility.SaveFilePanel("Save deck as JSON", folder, defaultName, "json");
        if (string.IsNullOrEmpty(path)) return;

        var dto = DeckMapper.ToDTO(so);
        if (string.IsNullOrEmpty(dto.id)) dto.id = System.Guid.NewGuid().ToString();
        string json = JsonUtility.ToJson(dto, true);

        // atomic write
        string tmp = path + ".tmp";
        File.WriteAllText(tmp, json);
        if (File.Exists(path)) File.Delete(path);
        File.Move(tmp, path);

        EditorUtility.DisplayDialog("Saved", "Deck saved to: " + path, "OK");
        RefreshDeckFiles();
    }

    void PingCardAsset(string keyOrId)
    {
        // Try GUID -> path -> ping; if not, try finding by filename
        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(keyOrId);
        if (string.IsNullOrEmpty(path))
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:CardData " + keyOrId);
            if (guids != null && guids.Length > 0) path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
        }

        if (!string.IsNullOrEmpty(path))
        {
            var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path);
            if (obj != null) EditorGUIUtility.PingObject(obj);
        }
        else
        {
            EditorUtility.DisplayDialog("Not found", "Could not find an asset for key/id: " + keyOrId, "OK");
        }
    }

    // ---------------- Addressables helper functions ----------------
    // Marks CardData assets referenced by the provided JM_DeckBase as addressable entries using GUID address.
    int MarkCardsAddressableFromDeck(JM_DeckBase deckAsset)
    {
        if (deckAsset == null || deckAsset.allCards == null) return 0;
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found. Create Addressables settings via Window > Asset Management > Addressables > Groups.");
            return 0;
        }

        int processed = 0;
        foreach (var card in deckAsset.allCards)
        {
            if (card == null) continue;
            string path = AssetDatabase.GetAssetPath(card);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning($"CardData {card.name} is not an asset on disk. Skipping.");
                continue;
            }

            string guid = AssetDatabase.AssetPathToGUID(path);
            var entry = settings.FindAssetEntry(guid);
            if (entry == null)
            {
                var group = settings.DefaultGroup;
                entry = settings.CreateOrMoveEntry(guid, group);
                Debug.Log($"Created addressable entry for {path} in group {group.Name}");
            }

            // set address to GUID (stable) and set addressableKey field on the scriptable asset if present
            entry.address = guid;

            var idField = card.GetType().GetField("addressableKey");
            if (idField != null)
            {
                idField.SetValue(card, guid);
                EditorUtility.SetDirty(card);
            }

            processed++;
        }

        AssetDatabase.SaveAssets();
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
        Debug.Log($"Marked {processed} CardData assets as Addressable (GUID key).");
        return processed;
    }

    int MarkCardsAddressableFromLoadedDeck()
    {
        if (loadedDeck == null || loadedDeck.allCards == null) return 0;
        return MarkCardsAddressableFromDeck(loadedDeck);
    }
}
#endif
