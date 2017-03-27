using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Controls menu interactions.
public class Menu : MonoBehaviour
{
	public Text bestTimeText;				// displays high score
	public Text lastTimeText;				// displays last earned score
	static private bool firstTime = true;   // initial play flag
	static public int bestTime = 0;			// high score

	// displays scores only after first and subsequent plays
	void Start()
	{
		if (!firstTime)
		{
			if (GameController.time > bestTime)
			{ bestTime = GameController.time; }
			bestTimeText.text = "Longest Survival: " + bestTime;
			lastTimeText.text = "Last Survival: " + GameController.time;
		}
	}

	// initializes and loads main level
	public void StartClick()
	{
		firstTime = false;
		SceneManager.LoadScene("main");
	}

	// exits
	public void QuitClick()
	{ Application.Quit(); }
}