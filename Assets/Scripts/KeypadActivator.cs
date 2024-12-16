using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadActivator : MonoBehaviour
{
    [SerializeField] private GameObject keypadPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keypadPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keypadPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked; 
            Cursor.visible = false; 
        }
    }
}



