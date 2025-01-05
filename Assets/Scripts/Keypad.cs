using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private string correctPassword;
    public bool isLastKeypad;

    // Returns the door associated with this keypad
    public GameObject GetDoorObject()
    {
        return door;
    }

    // Returns the correct password required to unlock this keypad
    public string GetCorrectPassword()
    {
        return correctPassword;
    }
}
