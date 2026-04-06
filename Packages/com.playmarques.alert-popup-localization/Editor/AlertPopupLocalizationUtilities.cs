#if UNITY_EDITOR
using Playmarques.AlertLocalization;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public static class AlertPopupLocalizationUtilities
{
    private const string TableName = AlertPopupLocalized.KEY_TABLE;
    private const string TablesFolder = "Localization/Tables/";
    private const string LocalesFolder = "Localization/Locales/";
    private const string SettingsFolder = "Assets/Localization/Settings";
    private const string SettingsAssetName = "Localization Settings.asset";

    private static readonly Dictionary<string, string> DefaultEntries = new()
    {
        { AlertPopupLocalized.KEY_DEFAULT_MESSAGE, "loading..." },
        { AlertPopupLocalized.KEY_BTN_CONFIRM, "Confirm" },
        { AlertPopupLocalized.KEY_BTN_CANCEL, "Cancel" }
    };

    public static void EnsureTableExistsAndIsInitialized()
    {
        EnsureLocalizationSettingsExists();

        EnsureAtLeastOneLocaleExists();
        var locales = LocalizationEditorSettings.GetLocales();

        string tablePath = AlertPopupUtilities.EnsureFolderExists(TablesFolder);

        var collection = LocalizationEditorSettings.GetStringTableCollection(TableName);

        if (collection == null)
        {
            collection = LocalizationEditorSettings.CreateStringTableCollection(
                TableName,
                tablePath,
                locales
            );

            if (collection == null)
            {
                Debug.LogError("[AlertPopup] Fail creating the String Table Collection 'AlertPopup'.");
                return;
            }

            Debug.Log("[AlertPopup] String Table Collection 'AlertPopup' was created.");
        }

        AddMissingEntriesToAllLocales(collection);

        EditorUtility.SetDirty(collection);
        EditorUtility.SetDirty(collection.SharedData);

        foreach (var table in collection.StringTables)
        {
            EditorUtility.SetDirty(table);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private static void EnsureLocalizationSettingsExists()
    {
        var settings = LocalizationEditorSettings.ActiveLocalizationSettings;
        if (settings != null)
            return;

        AlertPopupUtilities.EnsureFolderExists(SettingsFolder);

        settings = ScriptableObject.CreateInstance<LocalizationSettings>();

        var assetPath = AssetDatabase.GenerateUniqueAssetPath(
            $"{SettingsFolder}/{SettingsAssetName}"
        );

        AssetDatabase.CreateAsset(settings, assetPath);

        // Define esse asset como o settings ativo do projeto
        LocalizationEditorSettings.ActiveLocalizationSettings = settings;

        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.LogWarning("[AlertPopup] There is no Localization Settings, then it was automatically created.");

        return;
    }
    private static void EnsureAtLeastOneLocaleExists()
    {
        var locales = LocalizationEditorSettings.GetLocales();
        if (locales != null && locales.Count > 0)
            return;

        var path = AlertPopupUtilities.EnsureFolderExists(LocalesFolder);

        var englishLocale = Locale.CreateLocale(SystemLanguage.English);
        var assetPath = AssetDatabase.GenerateUniqueAssetPath(path+"en.asset");

        AssetDatabase.CreateAsset(englishLocale, assetPath);
        LocalizationEditorSettings.AddLocale(englishLocale);

        EditorUtility.SetDirty(englishLocale);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.LogWarning("[AlertPopup] There is no Locale. English Locale was created.");
    }
    private static void AddMissingEntriesToAllLocales(StringTableCollection collection)
    {
        foreach (var pair in DefaultEntries)
        {
            var sharedEntry = collection.SharedData.GetEntry(pair.Key);
            long keyId;

            if (sharedEntry == null)
            {
                sharedEntry = collection.SharedData.AddKey(pair.Key);
                keyId = sharedEntry.Id;
            }
            else
            {
                keyId = sharedEntry.Id;
            }

            foreach (var table in collection.StringTables)
            {
                var entry = table.GetEntry(keyId);
                if (entry == null || string.IsNullOrEmpty(entry.LocalizedValue))
                {
                    table.AddEntry(keyId, pair.Value);
                }
            }
        }
    }
}
#endif