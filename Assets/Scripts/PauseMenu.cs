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

    // Initializes the player input and escape action, setting up the callback for the escape key
    private void Start()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        escapeAction = playerInput.actions.FindAction("Escape");
        escapeAction.performed += OnEscape;
    }

    // Triggered when the escape key is pressed. Pauses or resumes the game depending on the current state
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

    // Resumes the game by hiding the pause menu, restoring normal time flow, and locking the cursor
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        PlayerMovement.SetCanMoveAndLookAround(true);
        GamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Pauses the game by displaying the pause menu, freezing time, and unlocking the cursor
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        PlayerMovement.SetCanMoveAndLookAround(false);
        GamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Loads the "MainMenu" scene when called, going back to the main menu
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
        
    }

    // Exits the game application when called
    public void QuitGame()
    {
        Application.Quit();
    }
}
