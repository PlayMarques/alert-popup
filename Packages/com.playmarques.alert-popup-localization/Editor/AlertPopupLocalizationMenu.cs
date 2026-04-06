#if UNITY_EDITOR
using Playmarques.AlertLocalization;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AlertPopupLocalizationMenu
{
    [MenuItem("GameObject/UI (Canvas)/Alert Popup Localized", false, 10)]
    public static void CreateAlertPopup(MenuCommand menuCommand)
    {
        AlertPopupLocalizationUtilities.EnsureTableExistsAndIsInitialized();

        string name = "AlertPopupLocalized";
        var prefab = AlertPopupUtilities.LoadPrefabFromAssets(name);
        if (prefab == null) prefab = AlertPopupUtilities.ClonePrefabFromResourcesToAssets(name, OnClone);

        var instance = AlertPopupUtilities.AddPrefabToScene(prefab, menuCommand);

        EditorSceneManager.MarkSceneDirty(instance.scene);
    }
    private static void OnClone(GameObject prefab)
    {
        var binder = prefab.GetComponent<AlertPopupLocalized>();
        binder.LocalizeStringEventsToDefault();
        EditorUtility.SetDirty(binder);
        EditorUtility.SetDirty(prefab);
        
    }
}
#endif