using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{
    // Loads the "Main Scene" when the script is enabled.
    private void OnEnable()
    {
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
    }
}
