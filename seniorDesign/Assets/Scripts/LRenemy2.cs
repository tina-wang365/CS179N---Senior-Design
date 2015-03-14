using UnityEngine;
using System.Collections;

public class LRenemy2 : MonoBehaviour {

	private int counter = 0;
	private float moveSpeed = -0.3f;
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
		
		Vector3 movement = new Vector3 (transform.position.x + moveSpeed, transform.position.y, transform.position.z);
		
		rigidbody.MovePosition (movement);
	}
}
