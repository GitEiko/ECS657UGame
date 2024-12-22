using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private string correctPassword;

    public GameObject GetDoorObject()
    {
        return door;
    }

    public string GetCorrectPassword()
    {
        return correctPassword;
    }
}
