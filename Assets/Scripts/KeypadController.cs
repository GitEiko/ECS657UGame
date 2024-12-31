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

    private string currentInput = "";

    private void Start()
    {
        keypadPanel.SetActive(false); 
    }

    public void ShowKeypad()
    {
        keypadPanel.SetActive(true);
        currentInput = "";
        UpdateDisplay();
    }

    public void HideKeypad()
    {
        keypadPanel.SetActive(false);
    }

    public void AddDigit(string digit)
    {
        if (currentInput.Length < 10) 
        {
            currentInput += digit;
            UpdateDisplay();
        }
    }

    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    public void SubmitPassword()
    {
        if (currentInput == currentKeypad.GetCorrectPassword()) 
        {
            passwordDisplay.text = "Correct";
            currentKeypad.GetDoorObject().tag = "Door";
            currentKeypad.tag = "Untagged";
        }
        else
        {
            passwordDisplay.text = "Incorrect"; 
        }
    }

    public void SetCurrentKeypad(Keypad currentKeypad)
    {
        this.currentKeypad = currentKeypad; 
    }
 
    private void UpdateDisplay()
    {
        passwordDisplay.text = currentInput;
    }
}

