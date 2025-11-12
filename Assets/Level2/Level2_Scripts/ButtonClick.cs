using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
    void onClick(){
        SceneManager.LoadScene("Level2_Buiding");
    }
}
