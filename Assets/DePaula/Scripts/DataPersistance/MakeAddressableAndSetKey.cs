#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.IO;

public class MakeAddressableAndSetKey
{
    [MenuItem("Tools/Addressables/Mark selected as Addressable (key = GUID)")]
    public static void MarkSelectedAsAddressableWithGUID()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found. Create Addressables profile first (Window > Asset Management > Addressables > Groups).");
            return;
        }

        foreach (var obj in Selection.objects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath)) continue;

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = settings.FindAssetEntry(guid);
            if (entry == null)
            {
                var group = settings.DefaultGroup;
                entry = settings.CreateOrMoveEntry(guid, group);
                Debug.Log($"Created Addressable entry for {assetPath} in group {group.Name}");
            }

            // set the addressable key to GUID (ou qualquer string que desejar)
            entry.address = guid;

            // Attempt to set addressableKey field on the asset if it exists
            var so = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            var scriptable = so as ScriptableObject;
            if (scriptable != null)
            {
                var idField = scriptable.GetType().GetField("addressableKey");
                if (idField != null)
                {
                    idField.SetValue(scriptable, guid);
                    EditorUtility.SetDirty(scriptable);
                }
            }
        }
        AssetDatabase.SaveAssets();
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
        Debug.Log("Selected assets marked addressable and addressableKey set (GUID).");
    }

    [MenuItem("Tools/Addressables/Mark selected as Addressable (key = filename)")]
    public static void MarkSelectedAsAddressableWithFilename()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found.");
            return;
        }

        foreach (var obj in Selection.objects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath)) continue;

            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = settings.FindAssetEntry(guid);
            if (entry == null)
            {
                var group = settings.DefaultGroup;
                entry = settings.CreateOrMoveEntry(guid, group);
            }

            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            entry.address = fileName;

            var so = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            var scriptable = so as ScriptableObject;
            if (scriptable != null)
            {
                var idField = scriptable.GetType().GetField("addressableKey");
                if (idField != null)
                {
                    idField.SetValue(scriptable, fileName);
                    EditorUtility.SetDirty(scriptable);
                }
            }
        }
        AssetDatabase.SaveAssets();
        settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
        Debug.Log("Selected assets marked addressable and addressableKey set (filename).");
    }
}
#endif
