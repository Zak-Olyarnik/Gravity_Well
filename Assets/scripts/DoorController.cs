using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls opening and closing of blast doors.
public class DoorController : MonoBehaviour
{
	public int sideFactor;			// 1 or -1 to control movement direction
	public AudioClip close;			// sound effect for door closing

	private float openedX = 5.6f;	// end position for opened door
	private float closedX = 1.9f;	// end position for closed door
	private Vector3 endPos;			// target position, switches between opened and closed

	void Start()
	{ endPos = transform.position; }

	public void OpenDoor()
	{ endPos = new Vector3(sideFactor * openedX, transform.position.y, 0); }

	public void CloseDoor()
	{
		AudioSource.PlayClipAtPoint(close, new Vector3(0, 0, -10), 1f);
		endPos = new Vector3(sideFactor * closedX, transform.position.y, 0);
	}

	// slowly move to target positions
	void FixedUpdate()
	{ transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * 1.4f); }
}
