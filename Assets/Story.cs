using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.LoadScene("Main Scene", LoadSceneMode.Single);
    }
}
