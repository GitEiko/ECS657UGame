using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
public class KeypadController : MonoBehaviour
{
    [SerializeField] private GameObject keypadPanel; 
    [SerializeField] private TextMeshProUGUI passwordDisplay; 

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
        if (currentInput.Length < 8) 
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
        if (currentInput == "1234") 
        {
            passwordDisplay.text = "Correct"; 
        }
        else
        {
            passwordDisplay.text = "Incorrect"; 
        }

       
    }

    private void UpdateDisplay()
    {
        passwordDisplay.text = currentInput;
    }
}

