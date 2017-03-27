using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls collectible collection behavior.
public class Collectible : MonoBehaviour
{
	public int weight;						// value added to player's mass on pickup
	public GameObject scoreDisplayUp;		// score UI to be displayed on pickup, antigravity
	public GameObject scoreDisplayDown;		// score UI to be displayed on pickup, normal gravity
	public AudioClip collect;				// sound effect for picking up collectible

	private float scoreTime = .5f;			// length of time to display score UI
	private GameObject score;				// score UI object to be instantiated
	private Rigidbody2D rb;

	void Start()
	{ rb = GetComponent<Rigidbody2D>(); }

	// when collected, adjust player's mass and score, instantiate and set to destroy score UI, and destroy collectible
	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")	// collected by player
		{
			AudioSource.PlayClipAtPoint(collect, new Vector3(0, 0, -10), 1f);
			Vector3 spawnPos = new Vector3(coll.gameObject.transform.position.x, coll.gameObject.transform.position.y + 1, 0);
			coll.gameObject.GetComponent<PlayerController>().CollVelocity(weight);
			Destroy(gameObject);
			if (GameController.dir > 0)		// collectibles will affect player mass differently depending on gravity direction
			{
				GameController.playerMass += weight;
				score = Instantiate(scoreDisplayUp, spawnPos, new Quaternion(0, 0, 0, 0));
			}
			else
			{
				GameController.playerMass -= weight;
				score = Instantiate(scoreDisplayDown, spawnPos, new Quaternion(0, 0, 0, 0));
			}
			Destroy(score, scoreTime);
		}
		else if (coll.gameObject.tag == "playerShot" || coll.gameObject.tag == "enemyShot")		// shot, just destroy
		{
			AudioSource.PlayClipAtPoint(collect, new Vector3(0, 0, -10), 1f);
			Destroy(gameObject);
			Destroy(coll.gameObject);
		}
		else if (coll.gameObject.tag == "platform")		// ran into platform, slow it to that speed
		{
			GetComponent<Gravity>().enabled = false;
			rb.velocity = coll.GetComponent<Rigidbody2D>().velocity;
		}
		
	}

	// simple rotation
	void FixedUpdate()
	{ rb.transform.Rotate(Vector3.forward * 50f * Time.deltaTime); }
}
