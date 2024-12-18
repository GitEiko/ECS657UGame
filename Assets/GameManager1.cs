using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager1 : MonoBehaviour
{
    public int playerLives = 5;  // Number of lives
    public Vector3 checkpointPosition;  // Respawn location

    void Start()
    {
        // Set the initial checkpoint to the starting position of the player
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            checkpointPosition = player.transform.position;
        }
    }

    public void LoseLife()
    {
        playerLives--;

        if (playerLives > 0)
        {
            RespawnPlayer();
        }
        else
        {
            GameOver();
        }
    }

    private void RespawnPlayer()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            // Offset the player slightly above the checkpoint to avoid clipping into the ground
            Vector3 respawnPosition = checkpointPosition + Vector3.up * 1.0f; // Adjust '1.0f' as needed
            player.transform.position = respawnPosition;

            // Reset player velocity to avoid carrying over momentum from before respawn
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }


    private void GameOver()
    {
        Debug.Log("Game Over! You ran out of lives.");
        
    }
}
