using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls enemy behavior.
public class EnemyController : MonoBehaviour
{
	public GameObject shot;				// enemy's projectile
	public AudioClip die;				// sound effect for enemy death
	public AudioClip shoot;				// sound effect for enemy shoot

	private GameObject player;			// reference to player, the target
	private float shotSpeed = 6f;		// speed shots move
	private float finalX = 3.46f;		// x-position to stop and attack player
	private Vector3 endPos;				// final enemy position
	private float shootTime = 7.5f;		// frequency of enemy fire

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");	// generally this is a bad approach, but I kept getting null
																	// references when trying to use the Singleton pattern
		InvokeRepeating("Shoot", shootTime, shootTime);			// start shooting at player
		
		if (transform.position.x < 0)		// set end position and rotation based on side of screen spawned on
		{
			transform.localRotation = Quaternion.Euler(0, 180, 33);
			endPos = new Vector3(-finalX, transform.position.y, 0); 
		}
		else
		{ endPos = new Vector3(finalX, transform.position.y, 0); }
	}
	
	// slowly move from offscreen spawn to end position
	void FixedUpdate()
	{ transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * .5f); }

	// enemy shooting
	void Shoot()
	{
		if (player != null)
		{
			AudioSource.PlayClipAtPoint(shoot, new Vector3(0, 0, -10), 1f);
			Vector3 dir = (player.transform.position - transform.position).normalized;	// get direction to player
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;				// convert to quaternion
			Quaternion rot = Quaternion.Euler(new Vector3(0, 0, angle));
			GameObject bolt = Instantiate(shot, transform.position, rot);				// shoot shot with velocity
			bolt.GetComponent<Rigidbody2D>().velocity = dir * shotSpeed;
			Destroy(bolt, 5f);															// timed destroy if target is not hit
		}
	}

	// destroy on contact with player shot or collectible
	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "playerShot" || coll.gameObject.tag == "collectible")
		{
			AudioSource.PlayClipAtPoint(die, new Vector3(0, 0, -10), 1f);
			Destroy(coll.gameObject);
			Destroy(this.gameObject);
		}
	}
}
