using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{

	// Use this for initialization
	public void NewGameBtn(string newGameLevel)
	{


		SceneManager.LoadScene(newGameLevel);
	}

	public void ExitGameBtn()
	{

		Application.Quit();
	}

}


// 21.9
// 6.5
// 7.06
// 25.8
// 1.22
// 3.83