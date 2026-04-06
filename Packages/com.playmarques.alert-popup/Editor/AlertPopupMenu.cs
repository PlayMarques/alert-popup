#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AlertPopupMenu
{
    [MenuItem("GameObject/UI (Canvas)/Alert Popup", false, 10)]
    public static void CreateAlertPopup(MenuCommand menuCommand)
    {
        var prefab = Resources.Load<GameObject>("AlertPopup");

        if (prefab == null)
        {
            Debug.LogError("AlertPopup prefab does not found in Resources/AlertPopup.");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        if (instance == null)
        {
            instance = Object.Instantiate(prefab);
        }

        instance.name = "AlertPopup";

        GameObjectUtility.SetParentAndAlign(instance, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(instance, "Create Alert Popup");
        Selection.activeObject = instance;

        EditorSceneManager.MarkSceneDirty(instance.scene);
    }
}
#endif