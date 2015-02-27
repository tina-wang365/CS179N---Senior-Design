using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIScript : MonoBehaviour
{
	float x, y, z;
	float moveDirection;// = 1.0f; // true if the player is moving forward (to the right)
	
	List<GameObject> platforms;// = new List<GameObject>();

	private GameObject inviLeft;
	private GameObject inviRight;
	
	private bool moveRight;
	private bool AtXPos; //global variable to determine if the character is at position
	private bool AtYPos;

	private bool atWaypoint;
	private bool isWhite;
	private bool createInvi;

	private bool isMoving;

	private bool onGround; // global variable, bool variable that says if player is on ground or not
	
	private int CurrPlat;//begin at -1 because will be stored in index 0 when created
	private int nextPlat; //begin at 0
	
	private bool actionJump;
	
	Vector3 currLoc; //current location
	Vector3 nextLoc; //next location
	Vector3 speed;
	
	private float xPos; //character x-pos
	
	void Start ()
	{
		init ();
	}

	void Update()
	{

	}

	// FixedUpdate function
	void FixedUpdate()
	{
		//if a platform has been created get coordinates of location to move to
		//determine whether you are moving left or right
		if(CurrPlat == nextPlat)
		{
			//reset = false;
			//actionJump = true;
			isMoving = true;
			atWaypoint = false;
			isWhite = false;
			
			//store position of characterController before any movement 
			x = transform.position.x;
			y = transform.position.y;

			//increment nextPlat since a platform was drawn
			nextPlat++;
			
			//get Current Location as well as next Location (go to location)
			currLoc = rigidbody.transform.position;
			nextLoc = platforms[CurrPlat].transform.GetChild(0).position;
			//Debug.Log ("before: " + currLoc + "after: " + nextLoc + " " + CurrPlat + " " + nextPlat);
			
			//atPost set to false because (for now) you have to move
			AtXPos = false;
			AtYPos = false;
			
			//moving right
			if(x < nextLoc.x)
			{
				if(createInvi)
				{
					inviLeft.transform.position = new Vector3 (rigidbody.transform.position.x + 6,
					                                           rigidbody.transform.position.y,
					                                           rigidbody.transform.position.z);
				}

				//Debug.Log ("RIGHT");
				moveDirection = 1;
				moveRight = true;
				//moveLeft = false;
				speed = new Vector3 (10,0,0);
			}
			
			//moving left
			else if(x > nextLoc.x)
			{
				if(createInvi)
				{
					inviLeft.transform.position = new Vector3 (rigidbody.transform.position.x - 6,
					                                           rigidbody.transform.position.y,
					                                           rigidbody.transform.position.z);
				}
				//Debug.Log ("LEFT");
				moveDirection = -1;
				//moveLeft = true;
				moveRight = false;
				speed = new Vector3 (-10,0,0);
			}
			//Vector2 currTemp = currLoc;
			//Vector2 newloc = nextLoc;

			//RaycastHit2D hit = Physics2D.Linecast(currTemp,newloc);
			//Debug.Log("hit this: " + hit.collider.gameObject.name);
			//Debug.DrawLine(currTemp,newloc);
			//Debug.DrawRay(currLoc,newloc);
			//if(hit.collider != null)
			//{
			//	Debug.Log ("Hit Something **************");
			//	Debug.DrawLine(currTemp,newloc);
			//	Debug.DrawRay(currLoc,newloc);
			//}
		}

		if(nextLoc.y <= y && !actionJump)
		{
			actionJump = false;
		}
//		Debug.Log(rigidbody.position.x + " jump to: " + nextLoc.x);
		
		//positionBeforeMove = transform.position.x;
		
		//store position before movement
		xPos = transform.position.x;
		//Debug.Log (xPos + " == " + nextLoc.x);
		//Debug.Log (onGround);
		//if(rigidbody.velocity.y == 0.0f)
		//	onGround = true;
		
		//get the difference of the rigidbody and the next Location
		//will be used to determine when to jump
		Vector3 difference = new Vector3 (
			transform.position.x - nextLoc.x,
			transform.position.y - nextLoc.y,
			transform.position.z - nextLoc.z);
		
		//if you have created a platform and ARE on the ground then check if you need to move
		if(onGround && platforms.Count > 0)
		{
			Debug.Log("ON GROUND AND PLATFORM");
			if(isMoving)//&& !AtXPos)
			{
				Debug.Log("MOVING TRUE");
				//in order to keep addForce at a constant velocity
				if(rigidbody.velocity.magnitude <= 10)
				{
					rigidbody.AddRelativeForce(speed * 10);
					//inviRight.rigidbody.AddRelativeForce(speed * 10);
					//inviLeft.rigidbody.AddRelativeForce(speed * 10);
				}
			}
			Debug.Log("MOVING FALSE");
		}
		else
			Debug.Log ("NOT MOVING AT ALL");
		//Debug.Log (rigidbody.velocity);

		//once you reach your X-Axis destination, STOP, even if unreachable
		if(moveRight){
			//inviPlayer = GameObject.Find("playerController/playerInvi");

			if(actionJump)
			{
				Jump ();
			}

			if(xPos + 4.5 > nextLoc.x)
			{
				//when you are at POSITION
				AtXPos = true;
				isMoving = false;
			}
		}
		else{
			if(actionJump)
			{
				Jump ();
			}
			if(xPos - 4.5 < nextLoc.x)
			{
				//when you are at POSITION
				AtXPos = true;
				isMoving = false;
			}
		}

		//WHAT TILL CALL THE JUMPING FUNCTION IF NEEDED
		//COMMENT OUT FOR NOW
		//if there exists a platform then you will move left/right/ or jump in place (FIX THE JUMP FOR THIS CASE)
//		if(platforms.Count > 0)
//		{
//			//if you are moving RIGHT
//			if(moveRight)
//			{
//				Debug.Log(difference.x);
//				//if(difference.x > -10.0 && you are onGround
//				if(difference.x > -10.0 && onGround)// && !actionJump)
//				{
//					//if you have have NOT jumped
//					if(actionJump)
//						Jump();
//				}
//				
//				//RIGHT
//				if(xPos > nextLoc.x)
//				{
//					//when you are at POSITION
//					AtXPos = true;
//				}
//			}
//			
//			//you are moving LEFT
//			else
//			{
//				Debug.Log(difference.x);
//				//if(difference.x > -10.0 && you are onGround
//				if(difference.x < 10.0 && onGround)// && !actionJump)
//				{
//					//if you have NOT jumped
//					if(actionJump)
//						Jump();
//				}
//				
//				//LEFT
//				if(xPos < nextLoc.x)
//				{
//					//when you are at POSITION
//					AtXPos = true;
//				}
//			}
//		}
		//}
	}

	void OnCollisionStay (Collision other)
	{
//		if(other.gameObject.name.IndexOf("Floor") != -1)
//		{
//			inviLeft.transform.position = new Vector3 (rigidbody.transform.position.x - 5,
//			                                           rigidbody.transform.position.y,
//			                                           rigidbody.transform.position.z);
//			
//			inviRight.transform.position = new Vector3 (rigidbody.transform.position.x + 5,
//			                                            rigidbody.transform.position.y,
//			                                            rigidbody.transform.position.z);
//			Debug.Log ("ON FLOOR");
//			onGround = true;
//		}
	}
	
	// collision detection between player and various objects
	void OnCollisionEnter (Collision collider)
	{
		if(collider.gameObject.name == "spikes")
		{
			Debug.Log("in spikes");
			Application.LoadLevel(Application.loadedLevel);
		}

		if(collider.gameObject.name.IndexOf("Floor") != -1)
		{
			createInvi = true;
//			inviLeft.transform.position = new Vector3 (rigidbody.transform.position.x - 6,
//			                                           rigidbody.transform.position.y,
//			                                           rigidbody.transform.position.z);
//			
//			inviRight.transform.position = new Vector3 (rigidbody.transform.position.x + 6,
//			                                            rigidbody.transform.position.y,
//			                                            rigidbody.transform.position.z);
			Debug.Log ("ON FLOOR");
			onGround = true;
		}
		// removes forces from acting on player if the player touches the ground or WHITE platform
		//if(collider.gameObject.name.IndexOf ("Floor") != -1 || collider.gameObject.name.IndexOf ("Platform") != -1)
		//{
		//	Debug.Log ("ON FLOOR");
		//	onGround = true;
		//}
		//else you are not on the Ground
		//else
		//{
		//	onGround = false;
		//}
		//if spikes, reset = true in order to reset the game/level/scene

		else if(collider.gameObject.name.Equals("Platform") && collider.gameObject.GetComponent<MeshRenderer>().material.color.Equals(Color.white))
		{
			//WORKS
			//Debug.Log("ENTER PLATFORM");
			isWhite = true;
			onGround = true;
			Debug.Log("in here");
			Debug.Log(rigidbody.position.x + " waypoint at: " + nextLoc.x);
			actionJump = false;
			//if(!isMoving)
			//{	
			//	Debug.Log("moving towards waypoint!");
			//	rigidbody.MovePosition(nextLoc + speed * Time.deltaTime);
			//}
		}

		//if platform landed on is BLUE, perform certain action
		//TO DO: enable "hop" only and disable horizontal movement
		else if(collider.gameObject.name.Equals("Platform") && collider.gameObject.GetComponent<MeshRenderer>().material.color.Equals(Color.blue))
		{
			onGround = true;

			float angle = Mathf.Deg2Rad * collider.gameObject.transform.rotation.eulerAngles.z + Mathf.PI / 2f;
			Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
			
			foreach(ContactPoint contact in collider.contacts)
			{
				if(contact.normal == direction)
				{
					//rigidbody.velocity = direction * 100f;
					rigidbody.AddForce(direction * 2500f);
					break;
				}
			}
		}

		else if(collider.gameObject.name.Equals("Platform") && collider.gameObject.GetComponent<MeshRenderer>().material.color.Equals(Color.red))
		{
			onGround = true;

			float angle = Mathf.Deg2Rad * collider.gameObject.transform.rotation.eulerAngles.z;
			Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
			Vector3 position = collider.contacts[0].point - collider.gameObject.GetComponent<Transform>().position;
			
			//rigidbody.velocity = direction * 100f;
			rigidbody.AddForce((Vector3.Dot(position, direction) < 0f ? 1f : -1f) * direction * 5000f);
		}
	}

	void OnCollisionExit (Collision other)
	{
		//FREE FALL CASES
		if(other.gameObject.name == "Platform")
		{
			//WORKS
			Debug.Log("LEAVING PLATFORMmmmmmmmmmmmmmmmmmmmm");
			onGround = false;
			actionJump = true;
		}

		if(other.gameObject.name.IndexOf("Floor") != -1)
		{
			//WORKS
			Debug.Log("LEAVING PLATFORM or LEAVING FLOOOOOOOOOOOOOOOR");
			onGround = false;
			actionJump = true;
		}
	}
	
	//OntriggerEnter function to check when the player has reached the edge
	//OnTriggerStay is called for almost every frame that the COLLIDER is TOUCHING the TRIGGER
	void OnTriggerEnter(Collider other)
	{
		//might be needed for waypoint
		if (other.gameObject.name == "Waypoint")
			atWaypoint = true;
	}
	
	
	public void addPlatform(GameObject platform)
	{
		platforms.Add(platform);
		CurrPlat++;
	}
	
	public void removePlatform(GameObject platform)
	{
		platforms.Remove(platform);
		CurrPlat--;
		nextPlat--;
	}
	
	// character jump function executed ONCE per platform
	public void Jump()
	{
		//FORCES applied to jump
		float JumpSpeed = 1500.0f;
		float forceX = 5.0f;
		
		//FORCES in ACTION (applied to rigid body) 
		rigidbody.AddForce (Vector3.up * JumpSpeed);
		rigidbody.AddForce (moveDirection * forceX, 0.0f, 0.0f);
		
		//once you jump, set onGround to FALSE, until you hit ground again
		onGround = false;
		//set actionJump to FALSE, since you already performed your ONE jump
		actionJump = false;
	}

	public void init()
	{
		Debug.Log ("Calling INIT fn");

		inviLeft = GameObject.Find("playerController/inviLeft");
		inviRight = GameObject.Find ("playerController/inviRight");
		createInvi = false;
//
//		inviLeft.transform.position = new Vector3 (rigidbody.transform.position.x - 4,
//		                                           rigidbody.transform.position.y,
//		                                           rigidbody.transform.position.z);
//
//		inviRight.transform.position = new Vector3 (rigidbody.transform.position.x + 4,
//		                                           rigidbody.transform.position.y,
//		                                           rigidbody.transform.position.z);

		//inviPlayer.transform.position.x = rigidbody.transform.position.x + 2;
		//inviPlayer.transform.position.y = rigidbody.transform.position.y;
		//inviPlayer.transform.position.z = rigidbody.transform.position.z;

		//nextLoc = platforms[CurrPlat].transform.GetChild(0).position;
		//xPos = transform.position.x; //get current character position

		//declare a list for platforms
		//set the counts for currPlat and nextPlat
		platforms = new List<GameObject>();
		CurrPlat = platforms.Count - 1;
		nextPlat = 0;
		
		//changing the gravity in order to allow the player a quicker jump
		Physics.gravity = new Vector3 (0, -25.0f, 0);
		
		//setting boolean values when script loads
		actionJump = false;

		atWaypoint = true;
		
		AtXPos = true;
		AtYPos = true;
		onGround = true;
		
		isMoving = false;
	}

}
