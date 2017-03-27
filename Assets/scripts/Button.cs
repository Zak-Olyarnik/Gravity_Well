using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls buttons which switch gravity and doors.
public class Button : MonoBehaviour
{
	public GameController gc;			// reference to GameController, so its SwitchGravity() can be called
	public Sprite pressedSprite;		// sprites to switch when pressed
	public Sprite depressedSprite;

	private bool pressed;				// stops rapid-fire switching

	// switches gravity on player contact or shot
	void OnTriggerEnter2D(Collider2D coll)
	{
		if ((coll.gameObject.tag == "Player" || coll.gameObject.tag == "playerShot") && !pressed)
		{
			gc.SwitchGravity();
			Press();
			if (coll.gameObject.tag == "playerShot")
			{ Destroy(coll.gameObject); }
		}
	}

	// switches to pressed sprite and sets pressed bool to stop rapid-fire switching
	void Press()
	{
		pressed = true;
		GetComponent<SpriteRenderer>().sprite = pressedSprite;
		Invoke("Depress", 1f);
	}

	// switches back to normal sprite after 1s
	void Depress()
	{
		pressed = false;
		GetComponent<SpriteRenderer>().sprite = depressedSprite;
	}
}
