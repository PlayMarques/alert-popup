#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class AlertPopupUtilities : MonoBehaviour
{
    public const string AssetMainPath = "Assets/Playmarques/AlertPopup/";
    public const string PrefabFolderPath = "Prefabs/";
    public static string EnsureFolderExists(string folderPath)
    {
        // Path that does not start with "Assets" is considered relative to the AssetMainPath
        if (!folderPath.StartsWith("Assets")) folderPath = AssetMainPath + folderPath;

        var parts = folderPath.Split('/');

        string currentPath = "Assets";

        for (int i = 1; i < parts.Length; i++)
        {
            string nextPath = currentPath + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, parts[i]);
            }

            currentPath = nextPath;
        }

        return folderPath;
    }

    public static GameObject AddPrefabToScene(GameObject prefab, MenuCommand menuCommand)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (instance == null) instance = Instantiate(prefab);

        GameObjectUtility.SetParentAndAlign(instance, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(instance, "Create Alert Popup");
        Selection.activeObject = instance;

        return instance;
    }
    public static GameObject LoadPrefabFromAssets(string name)
    {
        string path = AssetMainPath + PrefabFolderPath + name + ".prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        
        return prefab;
    }
    public static GameObject ClonePrefabFromResourcesToAssets(string name, Action<GameObject> OnClone = null)
    {
        string path = EnsureFolderExists(PrefabFolderPath) + name + ".prefab";
        var prefab = Resources.Load<GameObject>(name);

        if (prefab == null)
        {
            Debug.LogError(name + ".prefab does not found in Runtime/Resources please reinstall the package!");
            return null;
        }

        var tempInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if(tempInstance == null) tempInstance = Instantiate(prefab);

        OnClone?.Invoke(tempInstance);
        PrefabUtility.SaveAsPrefabAsset(tempInstance, path);
        DestroyImmediate(tempInstance);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return LoadPrefabFromAssets(name);
    }
}
#endif