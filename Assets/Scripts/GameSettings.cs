using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;
    [SerializeField] private Main_Menu menu;

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

