using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttonscript : MonoBehaviour
{
    [SerializeField] private Button mybutton;

    void Start()
    {
        mybutton.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
    void OnDestroy()
    {
        mybutton.onClick.RemoveListener(OnButtonClick);
    }
}
