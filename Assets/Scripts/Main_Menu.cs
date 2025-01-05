using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    // Loads the game scene asynchronously
    public void Play()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    // Exits the application
    public void Quit_Game()
    {
        Application.Quit();  
    }


}
