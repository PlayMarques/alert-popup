using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageField;

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

    public static AlertPopup Instance;


    private void Awake()
    {
        Instance = this; //Tihs is not a persistent GO so that's why each scene the instance has to change!
        btnConfirm.onClick.AddListener(OnConfirm);
        btnCancel.onClick.AddListener(OnCancel);
        inputField.onValueChanged.AddListener(OnUpdateInput);
        ResetState();
    }
    private void OnDestroy()
    {
        Instance = null;
        btnConfirm.onClick.RemoveListener(OnConfirm);
        btnCancel.onClick.RemoveListener(OnCancel);
        inputField.onValueChanged.RemoveListener(OnUpdateInput);

    }



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



    private AlertPopup Display(string message, bool confirm, bool cancel, bool input)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        if(!string.IsNullOrEmpty(message)) messageField.text = message;

        btnConfirm.gameObject.SetActive(confirm);
        btnCancel.gameObject.SetActive(cancel);
        inputField.gameObject.SetActive(input);

        return this;
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

        inputValue = "";
        inputField.text = inputValue;
        LimitInput(18).OnlyKeyNumbers(false);
        DisableConfirm(false).DisableCancel(false).DisableInput(false);

    }
}