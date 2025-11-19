using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadroom : MonoBehaviour
{
    // Start is called before the first frame update
    float delay = 5f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadLevel2());
    }

    // Update is called once per frame
    IEnumerator LoadLevel2()
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene("Room1");
    }
}
