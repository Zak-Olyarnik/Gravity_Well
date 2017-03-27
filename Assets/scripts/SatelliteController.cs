using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls special satellite collectible.
public class SatelliteController : MonoBehaviour 
{
	public int weight;						// value added to player's mass on pickup
	public GameObject scoreDisplay;		// score UI to be displayed on pickup, antigravity
	public AudioClip collect;				// sound effect for picking up collectible

	private float speed = .5f;				// movement speed
	private float scoreTime = .5f;			// length of time to display score UI
	private GameObject score;				// score UI object to be instantiated
	private Rigidbody2D rb;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		if (transform.position.x > 0)		// set speed based on side of screen spawned on
		{ speed = -speed; }
		Destroy(this.gameObject, 25f);		// destroys if not collected in 25s
	}

	void FixedUpdate()
	{ rb.velocity = Vector2.right * speed; }
	
	// when collected, adjust player's mass and score, instantiate and set to destroy score UI, and destroy collectible
	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
		{
			AudioSource.PlayClipAtPoint(collect, new Vector3(0, 0, -10), 1f);
			Vector3 spawnPos = new Vector3(coll.gameObject.transform.position.x, coll.gameObject.transform.position.y + 1, 0);
			coll.gameObject.GetComponent<PlayerController>().CollVelocity(weight);
			GameController.playerMass += weight;
			score = Instantiate(scoreDisplay, spawnPos, new Quaternion(0, 0, 0, 0));
			Destroy(gameObject);
			Destroy(score, scoreTime);
		}
	}
}
