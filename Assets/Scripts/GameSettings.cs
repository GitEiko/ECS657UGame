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
    private void OnLevelWasLoaded(int level)
    {
        TryAssignMenu();
    }

    private void Start()
    {
        TryAssignMenu();
    }

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
        Debug.Log("PLAY CALED");
    }
}

