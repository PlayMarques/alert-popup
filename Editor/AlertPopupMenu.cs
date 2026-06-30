#if UNITY_EDITOR
using Playmarques.AlertPopup;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AlertPopupMenu
{
    [MenuItem("GameObject/UI (Canvas)/Alert Popup", false, 10)]
    public static void CreateAlertPopup(MenuCommand menuCommand)
    {
        AlertPopupLocalizationUtilities.EnsureTableExistsAndIsInitialized();

        string name = "AlertPopup";

        var prefab = AlertPopupUtilities.LoadPrefabFromAssets(name);
        if(prefab == null) prefab = AlertPopupUtilities.ClonePrefabFromResourcesToAssets(name, OnClone);
        var instance = AlertPopupUtilities.AddPrefabToScene(prefab, menuCommand);

        EditorSceneManager.MarkSceneDirty(instance.scene);
    }
    private static void OnClone(GameObject prefab)
    {
        var binder = prefab.GetComponent<AlertPopup>();
        binder.LocalizeStringEventsToDefault();
        EditorUtility.SetDirty(binder);
        EditorUtility.SetDirty(prefab);
    }
}
#endif
