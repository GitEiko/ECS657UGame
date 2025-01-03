using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 5;
    public List<Image> lifeIcons;
    public Vector3 checkpointPosition;
    private int currentLives;
    private CharacterController controller;
    public PlayerInteraction interaction;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentLives = maxLives;
        UpdateLivesUI();
    }

    public void TakeDamage(int damageAmount)
    {
        currentLives -= damageAmount;
        currentLives = Mathf.Clamp(currentLives, 0, maxLives);


        UpdateLivesUI();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (interaction.keypadPanel.activeSelf)
        {
            interaction.keypadPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerMovement.SetCanMoveAndLookAround(true);
        }
        if (interaction.paperPanel.activeSelf)
        {
            interaction.paperPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerMovement.SetCanMoveAndLookAround(true);
        }

        if (collision.gameObject.CompareTag("EnemyTest"))
        {
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
            controller.enabled = false;
            // Offset the player slightly above the checkpoint to avoid clipping into the ground
            Vector3 respawnPosition = checkpointPosition + Vector3.up * 1.0f; // Adjust '1.0f' as needed
            transform.position = respawnPosition;
            Debug.Log(transform.gameObject.name);


            // Reset player velocity to avoid carrying over momentum from before respawn
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            controller.enabled = true;
    }

    private void GameOver()
    {
        if (currentLives == 0)
        {
            SceneManager.LoadScene("MainMenu");
            Debug.Log("Game Over! You ran out of lives.");
        }
        Debug.Log("Game Over! You ran out of lives.");

    }

    private void UpdateLivesUI()
    {

        for (int i = 0; i < lifeIcons.Count; i++)
        {
            if (i < currentLives)
            {
                lifeIcons[i].enabled = true;
            }
            else
            {
                lifeIcons[i].enabled = false;
            }
        }
    }
}
