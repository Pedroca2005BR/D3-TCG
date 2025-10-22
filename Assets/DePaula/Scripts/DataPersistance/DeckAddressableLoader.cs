using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class DeckAddressableLoader
{
    static string Folder => Path.Combine(Application.persistentDataPath, "decks");

    public static DeckDTO ReadDeckDTO(string fileName)
    {
        string path = Path.Combine(Folder, fileName);
        if (!File.Exists(path)) return null;
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<DeckDTO>(json);
    }

    // Async load using Addressables. Returns a runtime JM_DeckBase (ScriptableObject instance in memory)
    public static async Task<JM_DeckBase> LoadDeckAsync(string fileName)
    {
        var dto = ReadDeckDTO(fileName);
        if (dto == null) return null;

        var deck = ScriptableObject.CreateInstance<JM_DeckBase>();
        deck.allCards = new List<CardData>();

        if (dto.cardKeys == null || dto.cardKeys.Count == 0) return deck;

        // Load multiple assets by keys (efficient)
        AsyncOperationHandle<IList<CardData>> handle = Addressables.LoadAssetsAsync<CardData>(
            dto.cardKeys,
            null,
            UnityEngine.AddressableAssets.Addressables.MergeMode.Union
        );

        await handle.Task; // aguarda conclusão

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var card in handle.Result)
            {
                if (card != null) deck.allCards.Add(card);
                else Debug.LogWarning("CardData was null after Addressables load.");
            }
        }
        else
        {
            Debug.LogWarning($"Addressables failed to load deck cards for {fileName}: {handle.Status}");
        }

        // IMPORTANT: release handle when you no longer need the loaded assets.
        // If you want to keep the assets loaded and referenced by deck, DO NOT release here.
        // If you want Addressables to manage lifetime, you can release after copying needed data.
        // For this example, we'll not release to keep assets usable; the caller must decide.
        // Addressables.Release(handle); // only if you want to unload them immediately

        return deck;
    }
}
