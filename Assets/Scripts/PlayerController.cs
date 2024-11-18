using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameManager1 gameManager1;

    void Start()
    {
        gameManager1 = GameObject.Find("GameManager1").GetComponent<GameManager1>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyTest"))
        {
            Debug.Log("Player hit by enemy!");
            gameManager1.LoseLife();
        }
    }
}
