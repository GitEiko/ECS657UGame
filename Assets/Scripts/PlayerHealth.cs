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

    // Initializes the player's health, sets up the controller, and updates the UI to reflect the current lives
    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentLives = maxLives;
        UpdateLivesUI();
    }

    // Reduces the player's lives by the specified damage amount and updates the UI accordingly
    public void TakeDamage(int damageAmount)
    {
        currentLives -= damageAmount;
        currentLives = Mathf.Clamp(currentLives, 0, maxLives);


        UpdateLivesUI();
    }

    // Checks for collisions, handles interactions with the keypad and paper, and applies damage when colliding with enemies
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

    // Respawns the player at the checkpoint position, slightly above the ground, and resets the player's velocity to avoid momentum carryover
    private void RespawnPlayer()
    {
            controller.enabled = false;
            // Offset the player slightly above the checkpoint to avoid clipping into the ground
            Vector3 respawnPosition = checkpointPosition + Vector3.up * 1.0f;
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

    // Ends the game when the player's lives reach zero and transitions to the "LosingCutscene" scene
    private void GameOver()
    {
        if (currentLives == 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene("LosingCutscene");
            Debug.Log("Game Over! You ran out of lives.");
        }
        Debug.Log("Game Over! You ran out of lives.");

    }

    // Updates the UI to show the correct number of life icons based on the player's current lives
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
