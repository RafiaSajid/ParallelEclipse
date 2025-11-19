using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene3_load : MonoBehaviour
{
    float delay = 5f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadLevel31());
    }

    // Update is called once per frame
    IEnumerator LoadLevel31()
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("Zone5");
    }
}
