using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 5; 
    public List<Image> lifeIcons; 

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


