#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

public static class AlertPopupMenu
{
    [MenuItem("GameObject/UI (Canvas)/Alert Popup", false, 10)]
    public static void CreateAlertPopup(MenuCommand menuCommand)
    {
        string name = "AlertPopup";

        var prefab = AlertPopupUtilities.LoadPrefabFromAssets(name);
        if(prefab == null) prefab = AlertPopupUtilities.ClonePrefabFromResourcesToAssets(name);
        var instance = AlertPopupUtilities.AddPrefabToScene(prefab, menuCommand);

        EditorSceneManager.MarkSceneDirty(instance.scene);
    }
}
#endif