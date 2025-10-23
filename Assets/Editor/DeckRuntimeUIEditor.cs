// Assets/Editor/DeckRuntimeUIEditor.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DeckRuntimeUI))]
public class DeckRuntimeUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // draw default inspector
        DrawDefaultInspector();

        EditorGUILayout.Space();

        // cast target
        DeckRuntimeUI runtime = (DeckRuntimeUI)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Bootstrap Library From Addressables (Editor)"))
        {
            // call the instance method (Editor-only)
            runtime.BootstrapLibraryFromAddressablesEditor();
            // mark dirty so changes to any addressableKey fields get saved
            EditorUtility.SetDirty(runtime);
            // also repaint scene/editor windows in case UI updates
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.RepaintProjectWindow();
        }

        if (GUILayout.Button("Refresh Library (from Addressables settings)"))
        {
            // convenience: call same method (it will start the async populate)
            runtime.BootstrapLibraryFromAddressablesEditor();
            EditorUtility.SetDirty(runtime);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // Provide a helpbox with small guidance
        EditorGUILayout.HelpBox("Use this to populate the runtime library from Addressables settings.\nMake sure Addressables package is installed and you have built Addressables groups if testing in a player.", MessageType.Info);
    }

    // Add a context menu on the component header as well (optional)
    [MenuItem("CONTEXT/DeckRuntimeUI/Bootstrap Library From Addressables (Editor)")]
    static void ContextBootstrap(MenuCommand command)
    {
        var runtime = (DeckRuntimeUI)command.context;
        runtime.BootstrapLibraryFromAddressablesEditor();
        EditorUtility.SetDirty(runtime);
    }

    // Also add a menu item that will call the method on the selected DeckRuntimeUI or first found in scene
    [MenuItem("Tools/Decks/Bootstrap Library From Addressables (Selected or First)")]
    static void MenuBootstrap()
    {
        DeckRuntimeUI runtime = null;
        if (Selection.activeGameObject != null)
            runtime = Selection.activeGameObject.GetComponent<DeckRuntimeUI>();

        if (runtime == null)
            runtime = Object.FindFirstObjectByType<DeckRuntimeUI>();

        if (runtime == null)
        {
            EditorUtility.DisplayDialog("Bootstrap Library", "No DeckRuntimeUI found in the scene. Select a GameObject with the component or add one.", "OK");
            return;
        }

        // Call the editor bootstrap method
        runtime.BootstrapLibraryFromAddressablesEditor();
        EditorUtility.SetDirty(runtime);
        Debug.Log("BootstrapLibraryFromAddressablesEditor called on DeckRuntimeUI: " + runtime.name);
    }
}
#endif
