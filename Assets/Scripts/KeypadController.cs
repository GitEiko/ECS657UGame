using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
public class KeypadController : MonoBehaviour
{
    [SerializeField] private GameObject keypadPanel; 
    [SerializeField] private TextMeshProUGUI passwordDisplay;
    [SerializeField] private Keypad currentKeypad;

    public GameObject exitDoor;
    private string currentInput = "";

    // Initializes the keypad panel to be hidden at the start of the game
    private void Start()
    {
        keypadPanel.SetActive(false); 
    }

    // Displays the keypad UI and resets the current input
    public void ShowKeypad()
    {
        keypadPanel.SetActive(true);
        currentInput = "";
        UpdateDisplay();
    }

    // Hides the keypad UI
    public void HideKeypad()
    {
        keypadPanel.SetActive(false);
    }

    // Appends a digit to the current input if the maximum input length is not exceeded and updates the display
    public void AddDigit(string digit)
    {
        if (currentInput.Length < 15) 
        {
            currentInput += digit;
            UpdateDisplay();
        }
    }

    // Clears the current input and updates the display
    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    // Checks the entered password against the correct password, updates the UI, and modifies the door's state based on the result
    public void SubmitPassword()
    {
        if (currentInput == currentKeypad.GetCorrectPassword()) 
        {
            passwordDisplay.text = "Correct";
            currentKeypad.GetDoorObject().tag = "Door";
            currentKeypad.tag = "Untagged";
            if (currentKeypad.isLastKeypad)
            {
                exitDoor.tag = "FinalDoor";
            }
        }
        else
        {
            passwordDisplay.text = "Incorrect"; 
        }
    }

    // Sets the active keypad being interacted with
    public void SetCurrentKeypad(Keypad currentKeypad)
    {
        this.currentKeypad = currentKeypad; 
    }

    // Updates the password display to reflect the current input
    private void UpdateDisplay()
    {
        passwordDisplay.text = currentInput;
    }
}

