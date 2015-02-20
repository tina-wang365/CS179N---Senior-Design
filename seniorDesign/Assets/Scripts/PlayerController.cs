using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	bool onGround = true; // global variable, bool variable that says if player is on ground or not
	float moveDirection = 1.0f; // true if the player is moving forward (to the right)
	float positionBeforeMove = 0.0f;
	//bool atEdge = false; //global variable, bool var to detect when the player is on edge
	List<GameObject> platforms = new List<GameObject>();
	
	// collision detection between player and various objects
	void OnCollisionEnter (Collision collider)
	{
		// removes forces from acting on player if the player touches the ground
		if ((collider.gameObject.name.IndexOf ("Floor") != -1 || collider.gameObject.name.IndexOf ("Platform") != -1 
		     || collider.gameObject.name.IndexOf ("platform") != -1) && !onGround)
		{
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			onGround = true;
		}
		if(collider.gameObject.name.IndexOf("spike") != -1)
		{

		}

		if(collider.gameObject.name.Equals("Platform") && collider.gameObject.GetComponent<MeshRenderer>().material.color.Equals(Color.blue))
		{
			float angle = Mathf.Deg2Rad * collider.gameObject.transform.rotation.eulerAngles.z + Mathf.PI / 2f;
			Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

			foreach(ContactPoint contact in collider.contacts)
			{
				if(contact.normal == direction)
				{
					//rigidbody.velocity = direction * 100f;
					rigidbody.AddForce(direction * 5000f);
					break;
				}
			}
		}
		else if(collider.gameObject.name.Equals("Platform") && collider.gameObject.GetComponent<MeshRenderer>().material.color.Equals(Color.red))
		{
			float angle = Mathf.Deg2Rad * collider.gameObject.transform.rotation.eulerAngles.z;
			Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
			Vector3 position = collider.contacts[0].point - collider.gameObject.GetComponent<Transform>().position;

			//rigidbody.velocity = direction * 100f;
			rigidbody.AddForce((Vector3.Dot(position, direction) < 0f ? 1f : -1f) * direction * 5000f);
		}
	}

	public void addPlatform(GameObject platform)
	{
		platforms.Add(platform);
	}

	public void removePlatform(GameObject platform)
	{
		platforms.Remove(platform);
	}

	// character jump function
	void Jump()
	{
		onGround = false;
		float JumpSpeed = 700.0f;
		float forceX = 150.0f;
		
		rigidbody.AddForce (Vector3.up * JumpSpeed);
		rigidbody.AddForce (moveDirection * forceX, 0.0f, 0.0f);
	}
	
	// update function
	void FixedUpdate()
	{
		//float groundHeight = 0.0f;
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveSpeed = 0.2f;
		
		positionBeforeMove = transform.position.x;
		
		if(onGround)
		{
			//Added conditoin to check if the player is at the Edge, then you CANNOT move
//			if(atEdge)
//			{
//				//set player velocity to ZERO
//				rigidbody.velocity = Vector3.zero;
//			}

			//otherwise you are not at the edge and you can left or right
			//else{
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
				//remove this, I believe we dont need it if the player is on the ground
				//groundHeight = transform.position.y; // gets the height of the ground the player is on
			//}

//			if(GameObject.Find ("Platform"))
//			{
//				//this works to find when a platform is created
//				//Debug.Log("got one");
//				//if there is a platform created Jump to it
//				Jump ();
//			}
		}
		
		// program enters character jump function if spacebar is pressed
		if (Input.GetKey (KeyCode.Space) && onGround)//transform.position.y == groundHeight && onGround)
		{
			Jump();
			//onGround = false; // player is not on ground for duration of the jump
			//atEdge = false;
		}
	}

	//OntriggerEnter function to check when the player has reached the edge
//	void OnTriggerEnter(Collider other)
//	{
//		if(other.gameObject.tag == "edge")
//		{
//			atEdge = true;
//			//rigidbody.velocity = Vector3.zero;
//			//Debug.Log ("player is WITHIN EDGE!\n");
//		}
//	}
}
