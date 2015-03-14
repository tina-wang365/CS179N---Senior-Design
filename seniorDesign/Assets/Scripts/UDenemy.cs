using UnityEngine;
using System.Collections;

public class UDenemy : MonoBehaviour {

	private int counter = 0;
	private float moveSpeed = 500f;
	private float startTime = 0.0f;
	private float roundTimeLeft = 0.0f;
	private float roundTimeSeconds = 1.5f;
	
	void Awake()
	{
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update ()
	{
		roundTimeLeft = Time.time - startTime;
		if (roundTimeLeft >= roundTimeSeconds)
		{
			counter++;
			startTime = Time.time;
			roundTimeLeft = 0;
			if(counter % 2 == 1)
			{
				moveSpeed *= -1.0f;
				counter -= 2;
			}
		}
		
		Vector3 movement = new Vector3 (0.0f, moveSpeed, 0.0f);
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		rigidbody.AddForce (movement);
	}
}
