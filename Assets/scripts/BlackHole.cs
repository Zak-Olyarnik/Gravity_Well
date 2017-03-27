using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Destroy objects when they contact the black hole.
public class BlackHole : MonoBehaviour
{
	public bool isActive;			// controls if gravity is currently towards this black hole
	public GameObject sprite;		// sprite as a child game object for correct rotation

	private AudioSource sound;					// black hole ambient sound
	private float soundIncreaseTime = 120f;		// time to max volume
	private float rotZ = 0;						// sprite rotation angle

	void Start()
	{ sound = GetComponent<AudioSource>(); }

	// contact with black hole
	void OnTriggerEnter2D(Collider2D coll)
	{
		if (isActive)
		{
			if (coll.gameObject.tag == "Player")		// player died, end game
			{ Invoke("LoadMenu", 1.5f); }
			Destroy(coll.gameObject);
		}
	}

	// loads the menu, called with delay via Invoke
	void LoadMenu()
	{ SceneManager.LoadScene("menu"); }

	// increases sound volume, taking the desired time to reach max
	void FixedUpdate()
	{
		if (GameController.dir > 0)
		{ rotZ += Time.deltaTime * 20; }
		else
		{ rotZ -= Time.deltaTime * 20; }
		sprite.transform.localRotation = (Quaternion.Euler(0, 0, rotZ));

		if (sound.volume < 1)
		{ sound.volume += (Time.deltaTime / soundIncreaseTime); }
	}
}
