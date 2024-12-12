using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 5; // Maximum number of lives
    public List<Image> lifeIcons; // List of the red squares (life indicators)

    private int currentLives; // Current number of lives

    void Start()
    {
        // Set current lives to the maximum number of lives
        currentLives = maxLives;
        UpdateLivesUI(); // Call this once at the start to ensure UI is correct
    }

    // Call this function when the player is hit
    public void TakeDamage(int damageAmount)
    {
        currentLives -= damageAmount;
        currentLives = Mathf.Clamp(currentLives, 0, maxLives);

        Debug.Log($"Player took {damageAmount} damage! Current Lives: {currentLives}");

        UpdateLivesUI();
    }

    private void UpdateLivesUI()
    {
        Debug.Log($"Updating Lives UI... Current Lives: {currentLives}");

        for (int i = 0; i < lifeIcons.Count; i++)
        {
            if (i < currentLives)
            {
                lifeIcons[i].enabled = true; // Show this life
                Debug.Log($"Life {i + 1} is ON");
            }
            else
            {
                lifeIcons[i].enabled = false; // Hide this life
                Debug.Log($"Life {i + 1} is OFF");
            }
        }
    }
}


