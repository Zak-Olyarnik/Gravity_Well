using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implements "gravity" due to black hole on platform and collectible objects.
public class Gravity : MonoBehaviour
{
	private Rigidbody2D rb;

	void Start()
	{ rb = GetComponent<Rigidbody2D>(); }

	void FixedUpdate()
	{ rb.velocity = Vector2.up * GameController.dir * (GameController.g / rb.mass); }		// move object according to gravity
}
