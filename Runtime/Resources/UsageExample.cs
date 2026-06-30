using UnityEngine;
using Playmarques.AlertPopup;
using UnityEngine.UI;

public class UsageExample : MonoBehaviour
{
    private int inputTotal;
    private string displayedInput = string.Empty;

    /**
     * Example of how to use the AlertPopup
     * 
     * 1 - Attach this script to a GameObject in your scene.
     * 2 - Add these methods below to On click() events of your buttons in the scene.
     * PS: You must have the AlertPopup prefab in your scene for this to work. For that,
     * just right click on Canvas > UI (Canvas) > AlertPopup.
     */

    public void OpenInformationPopup()
    {
        AlertPopup.Show(AlertPopup.KeyDefaultMessage);
    }
    public void OpenInputPopup()
    {
        inputTotal = 0;
        displayedInput = string.Empty;

        string message = "Type and it will automatically sum the numbers";
        AlertPopup.Show(message, true, true, true).OnlyKeyNumbers().ListenerUpdateInput(SumNeighbors);
    }
    public void OpenConfirmPopup()
    {
        string message = "Are you sure you want to do this?";
        AlertPopup.Warning(message, true, true).ListenerConfirm(() =>
        {
            //Set a random color;
            GetComponent<Image>().color = new Color(Random.value, Random.value, Random.value);
        }).ListenerCancel(() =>
        {
            //Back to black color;
            GetComponent<Image>().color = Color.black;
        });
    }
    public void OpenErrorPopup()
    {
        AlertPopup.Error("You can't do that!");
    }

    private void SumNeighbors(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            inputTotal = 0;
            displayedInput = string.Empty;
            return;
        }

        string typedNumbers = input;

        if (input.StartsWith(displayedInput))
        {
            typedNumbers = input.Substring(displayedInput.Length);
        }
        else
        {
            inputTotal = 0;
        }

        foreach (char number in typedNumbers)
        {
            inputTotal += number - '0';
        }

        displayedInput = inputTotal.ToString();

        AlertPopup.Instance.SetInputValue(displayedInput);
    }

}
