// DeckPersistence.cs
// Lightweight persistence layer for DeckEditorWindow
// Place in Assets/Scripts/ (or similar). This file provides Save / Read / Delete / List and an
// Editor-only LoadDeckEditor method that resolves CardData assets via AssetDatabase.
// Depends on: DeckDTO, DeckMapper, JM_DeckBase, CardData

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DeckPersistence
{
    const string DecksFolderName = "decks";

    static string Folder => Path.Combine(Application.persistentDataPath, DecksFolderName);

    static void EnsureFolder()
    {
        if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
    }

    /// <summary>
    /// Save a JM_DeckBase to persistentDataPath/decks as JSON. Returns the filename (eg "{id}.json").
    /// Uses DeckMapper.ToDTO to build the DTO.
    /// </summary>
    public static string SaveDeck(JM_DeckBase deck)
    {
        if (deck == null) throw new ArgumentNullException(nameof(deck));
        EnsureFolder();

        var dto = DeckMapper.ToDTO(deck);
        if (dto == null) throw new InvalidOperationException("DeckMapper.ToDTO returned null");

        if (string.IsNullOrEmpty(dto.id)) dto.id = Guid.NewGuid().ToString();

        string fileName = dto.id.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? dto.id : dto.id + ".json";
        string path = Path.Combine(Folder, fileName);

        string json = JsonUtility.ToJson(dto, true);

        // atomic write
        string tmp = path + ".tmp";
        File.WriteAllText(tmp, json);
        if (File.Exists(path)) File.Delete(path);
        File.Move(tmp, path);

        Debug.Log($"DeckPersistence: Saved deck '{dto.name}' to {path}");
        return fileName;
    }

    /// <summary>
    /// Read a DeckDTO from a filename (either full path or filename like "id.json" or just id).
    /// Returns null if not found or parse fails.
    /// </summary>
    public static DeckDTO ReadDeckDTO(string fileOrName)
    {
        if (string.IsNullOrEmpty(fileOrName)) return null;

        string path = ResolvePath(fileOrName);
        if (path == null || !File.Exists(path)) return null;

        try
        {
            string json = File.ReadAllText(path);
            var dto = JsonUtility.FromJson<DeckDTO>(json);
            return dto;
        }
        catch (Exception ex)
        {
            Debug.LogError("DeckPersistence: Failed to read deck DTO - " + ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Delete deck file by name or full path. Returns true if deleted.
    /// </summary>
    public static bool DeleteDeck(string fileOrName)
    {
        string path = ResolvePath(fileOrName);
        if (path == null || !File.Exists(path)) return false;
        File.Delete(path);
        Debug.Log($"DeckPersistence: Deleted deck file {path}");
        return true;
    }

    /// <summary>
    /// List deck filenames (just filenames, not full paths).
    /// </summary>
    public static List<string> ListDeckFiles()
    {
        EnsureFolder();
        return Directory.GetFiles(Folder, "*.json").Select(Path.GetFileName).ToList();
    }

    static string ResolvePath(string fileOrName)
    {
        if (string.IsNullOrEmpty(fileOrName)) return null;

        // If it's an absolute path that exists, return it
        if (Path.IsPathRooted(fileOrName) && File.Exists(fileOrName)) return fileOrName;

        // Trim .json if user passed id
        string tryName = fileOrName;
        if (!tryName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) tryName = tryName + ".json";

        string candidate = Path.Combine(Folder, tryName);
        if (File.Exists(candidate)) return candidate;

        // Maybe user passed filename already (with extension) but in a different case; try case-insensitive search
        EnsureFolder();
        var files = Directory.GetFiles(Folder, "*.json");
        var found = files.FirstOrDefault(f => string.Equals(Path.GetFileName(f), tryName, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(found)) return found;

        return null;
    }

#if UNITY_EDITOR
    // Editor-only loader which resolves CardData entries using AssetDatabase.
    public static JM_DeckBase LoadDeckEditor(string fileOrName)
    {
        var dto = ReadDeckDTO(fileOrName);
        if (dto == null) return null;

        var deck = ScriptableObject.CreateInstance<JM_DeckBase>();
        deck.allCards = new List<CardData>();

        if (dto.cardKeys == null || dto.cardKeys.Count == 0) return deck;

        foreach (var key in dto.cardKeys)
        {
            CardData card = ResolveCardInEditor(key);
            if (card != null) deck.allCards.Add(card);
            else Debug.LogWarning($"DeckPersistence: Could not resolve CardData for key '{key}' in Editor.");
        }

        return deck;
    }

    static CardData ResolveCardInEditor(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;

        // 1) Try GUID -> path
        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(key);
        if (!string.IsNullOrEmpty(path))
        {
            var cd = UnityEditor.AssetDatabase.LoadAssetAtPath<CardData>(path);
            if (cd != null) return cd;
        }

        // 2) Try exact filename match (without extension)
        var guidsByName = UnityEditor.AssetDatabase.FindAssets($"t:CardData {key}");
        if (guidsByName != null && guidsByName.Length > 0)
        {
            var cd = UnityEditor.AssetDatabase.LoadAssetAtPath<CardData>(UnityEditor.AssetDatabase.GUIDToAssetPath(guidsByName[0]));
            if (cd != null) return cd;
        }

        // 3) Brute-force search through all CardData assets and compare common fields (addressableKey, id, filename)
        var all = UnityEditor.AssetDatabase.FindAssets("t:CardData");
        foreach (var g in all)
        {
            string p = UnityEditor.AssetDatabase.GUIDToAssetPath(g);
            var cd = UnityEditor.AssetDatabase.LoadAssetAtPath<CardData>(p);
            if (cd == null) continue;

            // match by explicit fields if available
            var idField = cd.GetType().GetField("id");
            var addrField = cd.GetType().GetField("addressableKey");
            string idVal = idField != null ? idField.GetValue(cd) as string : null;
            string addrVal = addrField != null ? addrField.GetValue(cd) as string : null;

            if (!string.IsNullOrEmpty(idVal) && string.Equals(idVal, key, StringComparison.OrdinalIgnoreCase)) return cd;
            if (!string.IsNullOrEmpty(addrVal) && string.Equals(addrVal, key, StringComparison.OrdinalIgnoreCase)) return cd;

            if (string.Equals(Path.GetFileNameWithoutExtension(p), key, StringComparison.OrdinalIgnoreCase)) return cd;
        }

        return null;
    }
#endif
}
