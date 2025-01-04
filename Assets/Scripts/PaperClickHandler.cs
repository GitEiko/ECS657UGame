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
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hit;

    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            if (hit.transform == this.transform)
    //            {
    //                ShowMessage();
    //            }
    //        }
    //    }
    //}

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



