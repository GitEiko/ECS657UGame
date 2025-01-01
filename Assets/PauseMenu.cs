using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    private PlayerInput playerInput;
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    private InputAction escapeAction;

    private void Start()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        escapeAction = playerInput.actions.FindAction("Escape");
        escapeAction.performed += OnEscape;
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (optionsMenuUI.activeSelf)
        {
            return;
        }
        else if (GamePaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        PlayerMovement.SetCanMoveAndLookAround(true);
        GamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        PlayerMovement.SetCanMoveAndLookAround(false);
        GamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    } 

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
