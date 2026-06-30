using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

namespace Playmarques.AlertPopup
{
    public class AlertPopup : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI messageField;

    [Header("Localization")]
    [SerializeField] private LocalizeStringEvent messageStringEvent;
    [SerializeField] private LocalizeStringEvent btnConfirmStringEvent;
    [SerializeField] private LocalizeStringEvent btnCancelStringEvent;

    [Header("Ballon Sprite by Types")]
    [SerializeField] private Image imageBallon;
    [SerializeField] private Sprite spriteBallon;
    [SerializeField] private Sprite spriteWarningBallon;
    [SerializeField] private Sprite spriteErrorBallon;

    [Header("Buttons")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnCancel;

    [Header("Ballon Color by Types")]
    [SerializeField] private Color colorInfoBg = Color.white;
    [SerializeField] private Color colorWarningBg = Color.yellow;
    [SerializeField] private Color colorErrorBg = Color.red;

    [SerializeField] private Color colorInfoTxt = Color.black;
    [SerializeField] private Color colorWarningTxt = Color.black;
    [SerializeField] private Color colorErrorTxt = Color.white;

    [Header("Button Color by Types")]
    [SerializeField] private Color colorBtnInfoBg = Color.black;
    [SerializeField] private Color colorBtnWarningBg = Color.black;
    [SerializeField] private Color colorBtnErrorBg = Color.black;

    [SerializeField] private Color colorBtnInfoTxt = Color.white;
    [SerializeField] private Color colorBtnWarningTxt = Color.white;
    [SerializeField] private Color colorBtnErrorTxt = Color.white;



    private string inputValue;
    private StringTable currentTable;
    private Task localizationTask;
    private bool localizationReady;

    public static AlertPopup Instance;
    public const string KeyTable = "AlertPopup";
    public const string KeyDefaultMessage = "DefaultAlertMessage";
    public const string KeyBtnConfirm = "BtnConfirm";
    public const string KeyBtnCancel = "BtnCancel";


#if UNITY_EDITOR
    public void LocalizeStringEventsToDefault()
    {
        messageStringEvent.StringReference.SetReference(KeyTable, KeyDefaultMessage);
        btnConfirmStringEvent.StringReference.SetReference(KeyTable, KeyBtnConfirm);
        btnCancelStringEvent.StringReference.SetReference(KeyTable, KeyBtnCancel);
    }
#endif

    private void Awake()
    {
        Instance = this; //This is not a persistent GO so that's why each scene the instance has to change!
        btnConfirm.onClick.AddListener(OnConfirm);
        btnCancel.onClick.AddListener(OnCancel);
        inputField.onValueChanged.AddListener(OnUpdateInput);
        ResetState();
        localizationTask = InitializeLocalization();
    }
    private void OnDestroy()
    {
        Instance = null;
        btnConfirm.onClick.RemoveListener(OnConfirm);
        btnCancel.onClick.RemoveListener(OnCancel);
        inputField.onValueChanged.RemoveListener(OnUpdateInput);

    }


    private async Task InitializeLocalization()
    {
        await LocalizationSettings.InitializationOperation.Task;
        await LocalizationSettings.StringDatabase.PreloadTables(KeyTable).Task;
        currentTable = await LocalizationSettings.StringDatabase.GetTableAsync(KeyTable).Task;
        localizationReady = true;
    }
    private Task EnsureLocalizationReady()
    {
        if (localizationReady) return Task.CompletedTask;

        localizationTask ??= InitializeLocalization();
        return localizationTask;
    }
    private async void SetLocalizedArguments(LocalizeStringEvent stringEvent, params object[] entries)
    {
        await EnsureLocalizationReady();

        object[] args = new object[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            //If the entry is found in the table, just use it, otherwise use the entry itself as argument
            string entry = entries[i].ToString();
            string arg = await LocalizationSettings.StringDatabase.GetLocalizedStringAsync(KeyTable, entry).Task;
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


    #region Button Confirm
    private Action Confirm;
    public AlertPopup ListenerConfirm(Action Confirm, bool disable = false)
    {
        this.Confirm = Confirm;
        return DisableConfirm(disable);
    }
    public AlertPopup DisableConfirm(bool disable = true)
    {
        btnConfirm.interactable = !disable;
        return this;
    }
    private void OnConfirm()
    {
        ConfirmInput?.Invoke(inputValue);
        Confirm?.Invoke();
        Hide();
    }
    #endregion

    #region Button Cancel
    private Action Cancel;
    public AlertPopup ListenerCancel(Action Cancel, bool disable = false)
    {
        this.Cancel = Cancel;
        return DisableCancel(disable);
    }
    public AlertPopup DisableCancel(bool disable = true)
    {
        btnCancel.interactable = !disable;
        return this;
    }
    private void OnCancel()
    {
        Cancel?.Invoke();
        Hide();
    }
    #endregion

    #region Input Field
    private Action<string> UpdateInput;
    public AlertPopup ListenerUpdateInput(Action<string> UpdateInput, bool disable = false, int limit = -1)
    {
        this.UpdateInput = UpdateInput;
        return LimitInput(limit).DisableInput(disable);
    }
    public AlertPopup DisableInput(bool disable = true)
    {
        inputField.interactable = !disable;
        return this;
    }
    public AlertPopup SetInputValue(string value, bool notify = false)
    {
        inputValue = value ?? string.Empty;

        if (notify)
        {
            inputField.text = inputValue;
        }
        else
        {
            inputField.SetTextWithoutNotify(inputValue);
        }

        inputField.MoveTextEnd(false);
        return this;
    }
    public AlertPopup LimitInput(int characterLimit = -1)
    {
        inputField.characterLimit = characterLimit < 0 ? inputField.characterLimit : characterLimit;
        return this;
    }
    public AlertPopup OnlyKeyNumbers(bool numericKeyboard = true)
    {
        if (numericKeyboard)
        {
            inputField.keyboardType = TouchScreenKeyboardType.NumberPad;
            inputField.contentType = TMP_InputField.ContentType.Custom;
            inputField.characterValidation = TMP_InputField.CharacterValidation.Integer;
        }
        else
        {
            inputField.keyboardType = TouchScreenKeyboardType.Default;
            inputField.contentType = TMP_InputField.ContentType.Alphanumeric;
            inputField.characterValidation = TMP_InputField.CharacterValidation.None;
        }
        return this;
    }
    private void OnUpdateInput(string value)
    {
        inputValue = value;
        UpdateInput?.Invoke(value);
    }
    


    private Action<string> ConfirmInput;
    public AlertPopup ListenerConfirm(Action<string> ConfirmInput, bool disable = false)
    {
        this.ConfirmInput = ConfirmInput;
        return DisableConfirm(disable);
    }
    #endregion

    #region Ballon Types
    public AlertPopup AsInfo()
    {
        if (spriteBallon != null) imageBallon.sprite = spriteBallon;
        imageBallon.color = colorInfoBg;
        messageField.color = colorInfoTxt;

        btnConfirm.image.color = colorBtnInfoBg;
        btnCancel.image.color = colorBtnInfoBg;

        btnConfirm.GetComponentInChildren<TextMeshProUGUI>().color = colorBtnInfoTxt;
        btnCancel.GetComponentInChildren<TextMeshProUGUI>().color = colorBtnInfoTxt;

        return this;
    }
    public AlertPopup AsWarning()
    {
        if (spriteWarningBallon != null) imageBallon.sprite = spriteWarningBallon;
        imageBallon.color = colorWarningBg;
        messageField.color = colorWarningTxt;

        btnConfirm.image.color = colorBtnWarningBg;
        btnCancel.image.color = colorBtnWarningBg;

        btnConfirm.GetComponentInChildren<TextMeshProUGUI>().color = colorBtnWarningTxt;
        btnCancel.GetComponentInChildren<TextMeshProUGUI>().color = colorBtnWarningTxt;

        return this;
    }
    public AlertPopup AsError()
    {
        if (spriteErrorBallon != null) imageBallon.sprite = spriteErrorBallon;
        imageBallon.color = colorErrorBg;
        messageField.color = colorErrorTxt;

        btnConfirm.image.color = colorBtnErrorBg;
        btnCancel.image.color = colorBtnErrorBg;

        btnConfirm.GetComponentInChildren<TextMeshProUGUI>().color = colorBtnErrorTxt;
        btnCancel.GetComponentInChildren<TextMeshProUGUI>().color = colorBtnErrorTxt;
        return this;
    }
    #endregion


    public static AlertPopup Show(string message, bool confirm = true, bool cancel = false, bool input = false)
    {
        return Instance.Display(message, confirm, cancel, input).AsInfo();
    }
    public static AlertPopup Warning(string message, bool confirm = true, bool cancel = false, bool input = false)
    {
        return Instance.Display(message, confirm, cancel, input).AsWarning();
    }
    public static AlertPopup Error(string message, bool confirm = true, bool cancel = false)
    {
        return Instance.Display(message, confirm, cancel, false).AsError();
    }



    #region Handle Message
    private AlertPopup Display(string message, bool confirm, bool cancel, bool input)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if (!string.IsNullOrEmpty(message)) SetMessage(message);

        btnConfirm.gameObject.SetActive(confirm);
        btnCancel.gameObject.SetActive(cancel);
        inputField.gameObject.SetActive(input);

        return this;
    }
    private async void SetMessage(string message)
    {

        await EnsureLocalizationReady();

        if (currentTable != null && currentTable.GetEntry(message) != null)
        {
            messageStringEvent.enabled = true;
            messageStringEvent.StringReference.SetReference(KeyTable, message);
            messageStringEvent.RefreshString();
            return;
        }

        messageStringEvent.enabled = false;
        messageField.text = message;
    }
    private void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        ResetState();
    }
    private void ResetState()
    {
        Confirm = null;
        Cancel = null;
        UpdateInput = null;
        ConfirmInput = null;

        SetInputValue("");
        LimitInput(18).OnlyKeyNumbers(false);
        DisableConfirm(false).DisableCancel(false).DisableInput(false);

    }
    #endregion
    }
}
