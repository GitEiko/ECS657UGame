using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 5;
    public List<Image> lifeIcons;
    public Vector3 checkpointPosition;
    private int currentLives;

    void Start()
    {
        currentLives = maxLives;
        UpdateLivesUI();
    }

    public void TakeDamage(int damageAmount)
    {
        currentLives -= damageAmount;
        currentLives = Mathf.Clamp(currentLives, 0, maxLives);

        Debug.Log($"Player took {damageAmount} damage! Current Lives: {currentLives}");

        UpdateLivesUI();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyTest"))
        {
            Debug.Log("enemy collision");
            TakeDamage(1);
            if (currentLives > 0)
            {
                RespawnPlayer();
            }
            else
            {
                GameOver();
            }
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

    private void UpdateLivesUI()
    {
        Debug.Log($"Updating Lives UI... Current Lives: {currentLives}");

        for (int i = 0; i < lifeIcons.Count; i++)
        {
            if (i < currentLives)
            {
                lifeIcons[i].enabled = true;
                Debug.Log($"Life {i + 1} is ON");
            }
            else
            {
                lifeIcons[i].enabled = false;
                Debug.Log($"Life {i + 1} is OFF");
            }
        }
    }
}
