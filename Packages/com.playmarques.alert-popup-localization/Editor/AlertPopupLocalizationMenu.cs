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
        AlertPopupLocalizationTableUtility.EnsureTableExistsAndIsInitialized();
        AssetDatabase.SaveAssets();

        var prefab = Resources.Load<GameObject>("AlertPopupLocalized");

        if (prefab == null)
        {
            Debug.LogError("AlertPopup prefab does not found in Resources/AlertPopup.");
            return;
        }
        var binder = prefab.GetComponent<AlertPopupLocalized>();
        binder.LocalizeStringEventsToDefault();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        PrefabUtility.SavePrefabAsset(prefab);
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        if (instance == null)
        {
            instance = Object.Instantiate(prefab);
        }

        instance.name = "AlertPopupLocalized";

        GameObjectUtility.SetParentAndAlign(instance, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(instance, "Create Alert Popup");
        Selection.activeObject = instance;

        EditorSceneManager.MarkSceneDirty(instance.scene);
    }
}
#endif