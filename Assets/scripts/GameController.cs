using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Runs the game, including spawning and gravity control.
public class GameController : MonoBehaviour
{
	public static float g;				// controls game "gravity" due to black holes, kept as float to prevent int division
	public static int dir;				// controls gravity direction
	public static int time;				// time passed in game
	public static int playerMass;		// player mass
	
	public GameObject[] platforms;			// list of spawnable platform sets
	public GameObject[] collectibles;		// list of spawnable collectibles
	public GameObject[] doors;				// list of doors to open and close
	public GameObject[] blackHoles;			// black holes to be turned on and off
	public GameObject satellite;			// special satellite collectible
	public GameObject enemy;				// reference to enemy object
	public GameObject[] bars;				// "life" bars
	public Sprite[] flashingBars;			// life bar sprites to switch when flashing
	
	public Text timeText;				// UI
	public Text longestTimeText;
	public Text massText;
	public Text gravText;

	private float platformSpawnTime = 7f;			// frequency of platform spawn
	private float collectibleSpawnTime = 1.25f;		// frequency of collectible spawn
	private float border = 3.7f;			// x-bounds for collectible spawn
	private float spawnY = -5.2f;			// y-location for collectible and platform spawn
	private float sideSpawnX = 5.1f;		// position just offscreen to spawn (+ or -)
	private float enemySpawnY = 4f;			// bound (- to +) for location of enemy spawn
	private float satSpawnY = 3.5f;			// y-location of satellite collectible
	private float massPercent;				// percent on life bar attributed to player mass
	private float flashRate = .7f;			// delay between flashing
	private float nextFlash;				// time next flash can occur

	void Start ()
	{
		g = 10f;		// set initial values
		dir = 1;
		time = 0;
		playerMass = 20;
		longestTimeText.text = "Longest Survival: " + Menu.bestTime;
		
		Invoke("PlatformSpawn", 0f);		// start spawns and clock
		Invoke("CollectibleSpawn", 0f);
		InvokeRepeating("SatelliteSpawn", 40f, 40f);
		InvokeRepeating("EnemySpawn", 25f, 15f);
		InvokeRepeating("Clock", 1, 1);
	}
	
	// continually update player mass and gravity display
	void Update ()
	{
		g += Time.deltaTime * .5f;						// this is equivalent to 1 point every 2 seconds
		massPercent = playerMass / (playerMass + g);	// calculate percent of life bar
		if (massPercent < .5 && Time.time > nextFlash)	// start bar flashing if mass is less than half of total
		{
			bars[0].GetComponent<SpriteRenderer>().sprite = flashingBars[1];
			bars[1].GetComponent<SpriteRenderer>().sprite = flashingBars[1];
			nextFlash = Time.time + 2*flashRate;
			Invoke("ResetBarColor", flashRate);
		}
		bars[0].transform.localScale = new Vector3(1, massPercent, 1);		// stretch life bar to correct size
		bars[1].transform.localScale = new Vector3(1, massPercent, 1);
	}

	// switches direction of gravity when player shoots switch
	public void SwitchGravity()
	{
		dir *= -1;
		spawnY *= -1f;
		satSpawnY *= -1f;
		SwitchDoors();
	}

	// opens and closes doors when gravity switches.  Includes moving special collider that spans both doors,
		// necessary because player kept getting trapped between normal colliders.  Enable/disable didn't work
		// either because setting that property stopped the collider from detecting that the player had left
		// the ground and setting the "grounded" variable for animation appropriately
	void SwitchDoors()
	{
		if (dir < 0)
		{
			blackHoles[0].GetComponent<BlackHole>().isActive = false;
			blackHoles[1].GetComponent<BlackHole>().isActive = true;
			doors[0].GetComponent<DoorController>().CloseDoor();
			doors[1].GetComponent<DoorController>().CloseDoor();
			doors[2].GetComponent<DoorController>().OpenDoor();
			doors[3].GetComponent<DoorController>().OpenDoor();
			doors[4].transform.position = new Vector3(0, 4.05f, 0);		// move special collider into position
			doors[5].transform.position = new Vector3(0, -5.2f, 0);
		}
		else
		{
			blackHoles[0].GetComponent<BlackHole>().isActive = true;
			blackHoles[1].GetComponent<BlackHole>().isActive = false;
			doors[0].GetComponent<DoorController>().OpenDoor();
			doors[1].GetComponent<DoorController>().OpenDoor();
			doors[2].GetComponent<DoorController>().CloseDoor();
			doors[3].GetComponent<DoorController>().CloseDoor();
			doors[4].transform.position = new Vector3(0, 5.2f, 0);
			doors[5].transform.position = new Vector3(0, -4.05f, 0);
		}
	}

	// spawns platforms, slowly decreasing time between successive spawns
	void PlatformSpawn()
	{
		GameObject platform = platforms[Random.Range(0, platforms.Length)];
		Instantiate(platform, new Vector3(platform.transform.position.x, spawnY, 0), platform.transform.rotation);
		platformSpawnTime -= .25f;
		if (platformSpawnTime < 1)
		{ platformSpawnTime = 1f; }
		Invoke("PlatformSpawn", platformSpawnTime);
	}

	// spawns collectibles, slowly decreasing time between successive spawns
	void CollectibleSpawn()
	{
		float spawnX = Random.Range(-border, border);
		GameObject collectible = collectibles[Random.Range(0, collectibles.Length)];
		Instantiate(collectible, new Vector3(spawnX, spawnY, 0), collectible.transform.rotation);
		collectibleSpawnTime -= .01f;
		if (collectibleSpawnTime < 1)
		{ collectibleSpawnTime = 1f; }
		Invoke("CollectibleSpawn", collectibleSpawnTime);
	}

	// spawn satellite collectibles near the vertical edges of the screen
	void SatelliteSpawn()
	{
		float sX = Random.Range(0, 2) == 0 ? -sideSpawnX : sideSpawnX;		// pick a random side of the screen
		Instantiate(satellite, new Vector3(sX, satSpawnY, 0), satellite.transform.rotation);
	}

	// spawn enemies
	void EnemySpawn()
	{
		float esX = Random.Range(0, 2) == 0 ? -sideSpawnX : sideSpawnX;		// pick a random side of the screen
		float esY = Random.Range(-enemySpawnY, enemySpawnY);				// pick a random height
		Vector3 startPos = new Vector3(esX, esY, 0); 						// enemy starts slightly offscreen
		Instantiate(enemy, startPos, enemy.transform.rotation);
	}

	// switch life bar sprites back to original
	void ResetBarColor()
	{
		bars[0].GetComponent<SpriteRenderer>().sprite = flashingBars[0];
		bars[1].GetComponent<SpriteRenderer>().sprite = flashingBars[0];
	}

	// updates the simple seconds counter, which also serves as a score display
	void Clock()
	{
		time += 1;
		timeText.text = "Survival Time: " + time;
	}
}
