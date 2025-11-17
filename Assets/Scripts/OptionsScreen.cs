using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsScreen : MonoBehaviour
{
    public void OnInstructionsButtonClick()
    {
        // Load the Instructions scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions", LoadSceneMode.Additive);
    }
}
