using UnityEngine;
using TMPro; 

public class PaperClickHandler : MonoBehaviour
{
    public GameObject messageCanvas;
    public TMP_Text messageText;
    public PlayerHealth playerHealth;
    public bool isCheckpoint;
    [TextArea]
    public string paperMessage;

    // Displays the paper message on the canvas and updates the player's checkpoint position if the paper is a checkpoint
    public void ShowMessage()
    {
        if (messageCanvas != null && messageText != null)
        {
            messageCanvas.SetActive(true);
            messageText.text = paperMessage; 
        }
        if (isCheckpoint)
        {
            playerHealth.checkpointPosition = gameObject.transform.position;
        }
    }
}



