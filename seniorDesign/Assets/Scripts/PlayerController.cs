using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	bool onGround = true; // global variable, bool variable that says if player is on ground or not
	float moveDirection = 1.0f; // true if the player is moving forward (to the right)
	float positionBeforeMove = 0.0f;
	
	// collision detection between player and various objects
	void OnCollisionEnter (Collision collider)
	{
		// removes forces from acting on player if the player touches the ground
		if((collider.gameObject.name.IndexOf("Floor") != -1 || collider.gameObject.name.IndexOf("Platform") != -1) && !onGround)
		{
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			onGround = true;
		}
		
		// puts player back at start if the player hits spikes
		if(collider.gameObject.name.IndexOf("spike") != -1)
		{
			/* CODE TO START POSITION HERE */
		}
	}
	
	// character jump function
	void Jump()
	{
		float JumpSpeed = 300.0f;
		float forceX = 150.0f;
		
		rigidbody.AddForce (Vector3.up * JumpSpeed);
		rigidbody.AddForce (moveDirection * forceX, 0.0f, 0.0f);
	}
	
	// update function
	void FixedUpdate()
	{
		float groundHeight = 0.0f;
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveSpeed = 0.2f;
		
		positionBeforeMove = transform.position.x;
		
		if(onGround)
		{
			// position updated for left/right movement
			Vector3 movement = new Vector3 (moveHorizontal * moveSpeed, 0.0f, 0.0f);
			Vector3 newPosition = new Vector3 (transform.position.x + movement.x, transform.position.y, transform.position.z);
			rigidbody.MovePosition (newPosition);
			
			if(positionBeforeMove > newPosition.x && moveDirection == 1.0f)
			{
				moveDirection *= -1;
			}
			if(positionBeforeMove < newPosition.x && moveDirection == -1.0f)
			{
				moveDirection *= -1;
			}
			
			groundHeight = transform.position.y; // gets the height of the ground the player is on
		}
		
		// program enters character jump function if spacebar is pressed
		if (Input.GetKey (KeyCode.Space) && transform.position.y == groundHeight && onGround)
		{
			Jump();
			onGround = false; // player is not on ground for duration of the jump
		}
	}
}
