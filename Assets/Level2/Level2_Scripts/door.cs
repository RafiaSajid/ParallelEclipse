using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public bool isLocked = true;
    public GameManager gameManager;
    public int requiredKeys = 3;

    public void TryOpenDoor()
    {
        if (gameManager != null && gameManager.GetKeysCount() >= requiredKeys)
        {
            Debug.Log("Door unlocked!");
            // Play animation, open door, or load next scene
            isLocked = false;
            SceneManager.LoadScene("PasswordRoom");
        }
        else
        {
            Debug.Log("Door locked. You need more keys!");
        }
    }
}
