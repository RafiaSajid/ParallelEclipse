using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextsceneL2 : MonoBehaviour
{

    float delay = 2f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadLevel3());
    }

    // Update is called once per frame
    IEnumerator LoadLevel3()
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("Level3_Instructions");
    }
}
