using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;
    [SerializeField] private Main_Menu menu;

    Button casual;
    Button normal;
    Button intense;

    public float PlayerSpeed { get; private set; }
    public float EnemySpeed { get; private set; }

    // Ensures a single instance of GameSettings persists across scenes, destroying duplicates
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Attempts to assign the menu reference when a new scene is loaded
    private void OnLevelWasLoaded(int level)
    {
        TryAssignMenu();
    }

    // Ensures the menu reference is assigned when the script starts
    private void Start()
    {
        TryAssignMenu();
    }

    // Assigns the Main_Menu component and difficulty buttons if the scene is the main menu, setting up their click listeners
    private void TryAssignMenu()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            menu = gameObject.GetComponent<Main_Menu>();
            if (menu == null)
            {
                Debug.LogWarning("Main_Menu not found in the current scene.");
            }
            GameObject.Find("DifficultyPanel").SetActive(true);
            casual = GameObject.Find("Casual").gameObject.GetComponent<Button>();
            normal = GameObject.Find("Normal").gameObject.GetComponent<Button>();
            intense = GameObject.Find("Intense").gameObject.GetComponent<Button>();
            casual.onClick.AddListener(() => GameSettings.Instance.SetDifficulty("Casual"));
            normal.onClick.AddListener(() => GameSettings.Instance.SetDifficulty("Normal"));
            intense.onClick.AddListener(() => GameSettings.Instance.SetDifficulty("Intense"));
            GameObject.Find("DifficultyPanel").SetActive(false);
        }
    }

    // Configures player and enemy speeds based on the selected difficulty level and starts the game
    public void SetDifficulty(string difficulty)
    {
        switch (difficulty)
        {
            case "Casual":
                PlayerSpeed = 12f;
                EnemySpeed = 17f;
                break;
            case "Normal":
                PlayerSpeed = 9f;
                EnemySpeed = 17f;
                break;
            case "Intense":
                PlayerSpeed = 8f;
                EnemySpeed = 18f;
                break;
        }
        menu.Play();
    }
}

