using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

namespace Playmarques.AlertLocalization
{
    public class AlertPopupLocalized : MonoBehaviour
    {
        [Header("Localization")]
        [SerializeField] private LocalizeStringEvent messageStringEvent;
        [SerializeField] private LocalizeStringEvent btnConfirmStringEvent;
        [SerializeField] private LocalizeStringEvent btnCancelStringEvent;

        private static AlertPopupLocalized Instance;
        public const string KEY_TABLE = "AlertPopup";
        public const string KEY_DEFAULT_MESSAGE = "DefaultAlertMessage";
        public const string KEY_BTN_CONFIRM = "BtnConfirm";
        public const string KEY_BTN_CANCEL = "BtnCancel";

#if UNITY_EDITOR
        public void LocalizeStringEventsToDefault()
        {
            messageStringEvent.StringReference.SetReference(KEY_TABLE, KEY_DEFAULT_MESSAGE);
            btnConfirmStringEvent.StringReference.SetReference(KEY_TABLE, KEY_BTN_CONFIRM);
            btnCancelStringEvent.StringReference.SetReference(KEY_TABLE, KEY_BTN_CANCEL);
        }
#endif
        private async void Awake()
        {
            Instance = this;
            await LocalizationSettings.InitializationOperation.Task;
            await LocalizationSettings.StringDatabase.PreloadTables(KEY_TABLE).Task;
        }
        private async void SetLocalizedArguments(LocalizeStringEvent stringEvent, params object[] entries)
        {
            object[] args = new object[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                //If the entry is found in the table, just use it, otherwise use the entry itself as argument
                string entry = entries[i].ToString();
                string arg = await LocalizationSettings.StringDatabase.GetLocalizedStringAsync(KEY_TABLE, entry).Task;
                args[i] = string.IsNullOrEmpty(arg) ? entries[i] : arg;
            }

            stringEvent.StringReference.Arguments = args;
            stringEvent.RefreshString();
        }


        public static void SetMessageArgs(params object[] argOrEntries)
        {
            Instance.SetLocalizedArguments(Instance.messageStringEvent, argOrEntries);
        }
        public static void SetButtonConfirmArgs(params object[] argOrEntries)
        {
            Instance.SetLocalizedArguments(Instance.btnConfirmStringEvent, argOrEntries);
        }
        public static void SetButtonCancelArgs(params object[] argOrEntries)
        {
            Instance.SetLocalizedArguments(Instance.btnCancelStringEvent, argOrEntries);
        }

        public static AlertPopup Show(string entry, bool confirm = true, bool cancel = false, bool input = false)
        {
            Instance.messageStringEvent.StringReference.SetReference(KEY_TABLE, entry);
            Instance.messageStringEvent.RefreshString();
            return AlertPopup.Show(null, confirm, cancel, input);
        }
        public static AlertPopup Warning(string entry, bool confirm = true, bool cancel = false, bool input = false)
        {
            Instance.messageStringEvent.StringReference.SetReference(KEY_TABLE, entry);
            Instance.messageStringEvent.RefreshString();
            return AlertPopup.Warning(null, confirm, cancel, input);
        }
        public static AlertPopup Error(string entry, bool confirm = true, bool cancel = false)
        {
            Instance.messageStringEvent.StringReference.SetReference(KEY_TABLE, entry);
            Instance.messageStringEvent.RefreshString();
            return AlertPopup.Error(null, confirm, cancel);
        }
    }
}
