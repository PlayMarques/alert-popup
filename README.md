# alert-popup

Display a message to the player asking confirmation, input or just show an error.



Alert Popup Requeriments:

* Unity 6.3 LTS (6000.3+)
* TMP Essential Resources imported into the project
* Localization 1.5.11+

Usage Guide:
1. Right mouse button on your Canvas object in the hierarchy.
2. Find the option "UI (Canvas)".
3. Select "Alert Popup".
4. Add `using Playmarques.AlertPopup;` to your script.
5. Just call methods "AlertPopup.Show(...)" from anywhere on your code.
6. Make it looks nice by setting colors and sprites in Prefab script "Alert Popup" (optional).

Tips:
1. Use "Listener...(action)" to add an action that would run right after the user do something.
2. You may send an action with void or string parameter to ListenerConfirm. The second is for when having Text Input.
3. Avoid calling it from Awake since Instance may be still null while not initialized.
4. You have to set up all your localized messages for alert on the table AlertPopup.
5. If the message sent to AlertPopup.Show(...) is found as a key on the AlertPopup table, the localized value will be displayed. If it is not found, the message itself will be displayed.

Examples:
1. AlertPopup.Error(exception.Message);
2. AlertPopup.Show("LocalizedEntry", true, true);
3.  string message = "Are you sure you want delete your account? Type 'YES' for sure";
    AlertPopup.Warning(message, true, true, true).ListenerConfirm(DeleteAccount, true).ListenerUpdateInput(OnTypeYes).LimitInput(3);
    private void OnTypeYes(string input)
    {
        if(input.ToUpper() == "YES")
        {
            AlertPopup.Instance.DisableConfirm(false);
        } else
        {
            AlertPopup.Instance.DisableConfirm(true);
        }
    }
4. AlertPopup.Instance.SetInputValue("123");
