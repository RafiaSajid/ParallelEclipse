using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required for Button component/CanvasGroup
using UnityEngine.SceneManagement;

public class roombutton : MonoBehaviour
{
    // The button GameObject or its parent container (CanvasGroup is best)
    // Drag your button here in the Inspector!
    public GameObject buttonToAppear; 

    // Time delay in seconds (made it float for better compatibility with WaitForSeconds)
    public float delay = 10f; 

    void Awake()
    {
        // 1. Ensure the button is hidden immediately when the scene starts
        if (buttonToAppear != null)
        {
            buttonToAppear.SetActive(false);
        }
    }

    void Start()
    {
        // 2. Start the timer
        StartCoroutine(ShowButtonAfterDelay());
    }

    IEnumerator ShowButtonAfterDelay()
    {
        Debug.Log("Waiting for " + delay + " seconds...");
        
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // 3. After the delay, show the button
        if (buttonToAppear != null)
        {
            buttonToAppear.SetActive(true);
            Debug.Log("Button is now visible and functional.");
        }
    }

    // This method is called when the user clicks the button
    public void OnButtonClick()
    {
        // 4. Load the scene
        SceneManager.LoadScene("Post-apocalyptic"); 
    }
}