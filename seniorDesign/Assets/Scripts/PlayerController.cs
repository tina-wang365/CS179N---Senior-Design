using UnityEngine;
using System.Collections;

<<<<<<< Updated upstream
public class PlayerController : MonoBehaviour {
	public int speed;

=======
public class PlayerController : MonoBehaviour
{
	public bool pickedUp = false;
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

	void OnTriggerEnter(Collision other) {
		if (other.gameObject.tag == "Pickup") {
			other.gameObject.SetActive (false);
			pickedUp = true;
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
>>>>>>> Stashed changes
	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		//float moveVertical = Input.GetAxis("Vertical");
		
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, 0.0f);
		
		rigidbody.AddForce(movement * speed * Time.deltaTime);
	}
}
