#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
#endif

public static class EditorHelpers
{
#if UNITY_EDITOR
    public static void SetCardDataAndSave(CardDisplay display, CardData data)
    {
        if (display == null) return;

        // Para componentes em GameObjects de cena:
        Undo.RecordObject(display, "Set CardData");
        display.cardData = data;
        EditorUtility.SetDirty(display);

        // Marca a cena como modificada para que Unity peça salvar
        EditorSceneManager.MarkSceneDirty(display.gameObject.scene);
        // Opcional: salvar a cena automaticamente
        // EditorSceneManager.SaveOpenScenes();
    }

    public static void SetCardDataToPrefabAsset(string prefabPath, CardData data)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) return;

        var so = new SerializedObject(prefab);
        var prop = so.FindProperty("m_Script"); // não é o ideal; melhor usar SerializedObject do componente
        // exemplo para componente:
        // var compProp = so.FindProperty("components.Array.data[0].component");
        // Better approach: instantiate a temporary instance, modify component, apply to prefab:
        var temp = GameObject.Instantiate(prefab);
        var display = temp.GetComponent<CardDisplay>();
        if (display != null)
        {
            Undo.RecordObject(display, "Set CardData on prefab");
            display.cardData = data;
            EditorUtility.SetDirty(display);
            // Apply changes to prefab
            PrefabUtility.SaveAsPrefabAsset(temp, prefabPath);
        }
        GameObject.DestroyImmediate(temp);
    }
#endif

#if UNITY_EDITOR
    public static void SetSerializedCardData(CardDisplay display, CardData data)
    {
        var so = new SerializedObject(display);
        var prop = so.FindProperty("cardData");
        prop.objectReferenceValue = data;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(display);
        EditorSceneManager.MarkSceneDirty(display.gameObject.scene);
    }
#endif

}
