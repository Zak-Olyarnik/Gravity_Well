using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls player movement.
public class PlayerController : MonoBehaviour
{
	
	public GameObject scoreDisplay;		// score UI to be displayed on hit by enemy
	public GameObject shot;				// player's shot
	public Transform shotSpawn;			// offset position to spawn player shots
	public AudioClip shoot;				// sound effect for player shoot
	public AudioClip hit;				// sound effect for enemy hit

	private Rigidbody2D rb;
	private Animator anim;
	private float speed = 2f;			// speed of movement
	private float shotSpeed = 4f;		// speed of shots
	private float scoreTime = .5f;		// length of time to display score UI
	private float fireRate = .3f;		// delay between firing shots
	private float nextFire;				// time next shot can be fired

	private bool grounded;					// detects collision with doors and platforms, for animation
	private bool collectible = false;		// if under influence of picking up a collectible
	private int collectibleWeight;			// essentially amount to "bob" when picking up a collectible
	private int frames;						// frames left under collectible's influence

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}

	void Update()
	{
		// x-movement based on player input
		Vector2 movement = new Vector2();
		movement.x = Input.GetAxis("Horizontal") * speed;
		if (movement.x < 0)     // sets direction sprites are facing
		{ transform.localRotation = Quaternion.Euler(0, 0, 0); }
		else if (movement.x > 0)
		{ transform.localRotation = Quaternion.Euler(0, 180, 0); }
		movement.y = 0;
		anim.SetFloat("movement", movement.magnitude);
		rb.velocity = movement;

		// shooting
		if (Input.GetButton("Fire1") && Time.time > nextFire)   // checks for fire button and if time delay has passed
		{
			AudioSource.PlayClipAtPoint(shoot, new Vector3(0, 0, -10), 1f);
			nextFire = Time.time + fireRate;
			GameObject s = Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			s.GetComponent<Rigidbody2D>().velocity = -transform.right * shotSpeed;
		}
	}

	// adjust player vertical movement due to "gravity" based on mass
	void FixedUpdate()
	{
		if (!grounded)				// "grounded" overrides all other animation, so start by turning that off if necessary
		{ anim.SetBool("grounded", false); }

		if (collectible)		// movement under influence of collectible, increases and adjusts direction of 
		{							// velocity for 10 frames to create "bobbing" effect
			rb.velocity -= 2f * new Vector2(0, collectibleWeight).normalized;
			frames--;
			if (frames == 0)
			{ collectible = false; }
		}
		else					// normal movement: velocity calculated as a ratio between player mass and g, then direction applied
		{
			if (GameController.playerMass > GameController.g)	// move player away from black hole (with appropriate animation)
			{
				if (GameController.dir < 0)
				{ anim.SetTrigger("hover"); }
				else if (!grounded)
				{ anim.SetTrigger("fall"); }
				rb.velocity -= GameController.dir * new Vector2(0, GameController.playerMass / GameController.g * .5f); 
			}
			else if (GameController.playerMass <= 0)	// this case stops division by zero or negative
			{
				anim.SetTrigger("fall");
				rb.velocity += GameController.dir * new Vector2(0, GameController.g * .5f); 
			}
			else if (GameController.playerMass < GameController.g)	// move player towards black hole
			{
				anim.SetTrigger("fall");
				rb.velocity += GameController.dir * new Vector2(0, GameController.g / GameController.playerMass * .5f); 
			}
		}
	}

	// reduce player mass and display UI when shot by enemy
	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "enemyShot")
		{
			AudioSource.PlayClipAtPoint(hit, new Vector3(0, 0, -10), 1f);
			Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 1, 0);
			GameObject score = Instantiate(scoreDisplay, spawnPos, new Quaternion(0, 0, 0, 0));
			Destroy(score, scoreTime);
			GameController.playerMass -= 5;
			Destroy(coll.gameObject);
		}
	}

	// changes animation on contact with ground / platforms
	void OnCollisionEnter2D(Collision2D coll)
	{
		if ((coll.gameObject.tag == "platform" || coll.gameObject.tag == "door") && GameController.dir > 0)
		{
			grounded = true;
			anim.SetBool("grounded", true);
		}
	}

	// changes animation on contact with ground / platforms
	void OnCollisionStay2D(Collision2D coll)
	{
		if ((coll.gameObject.tag == "platform" || coll.gameObject.tag == "door") && GameController.dir > 0)
		{
			grounded = true;
			anim.SetBool("grounded", true);
		}
	}

	// changes animation on leaving contact with ground / platforms
	void OnCollisionExit2D(Collision2D coll)
	{
		if ((coll.gameObject.tag == "platform" || coll.gameObject.tag == "door"))
		{ grounded = false; }
	}

	// adjusts velocity to make player "bob" immediately after collecting collectible
		// called from Collectible.cs
	public void CollVelocity(int weight)
	{
		collectible = true;
		collectibleWeight = weight;
		frames = 10;
	}
}
