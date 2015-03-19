using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	private List<GameObject> platforms = new List<GameObject>();
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 blueDirection = Vector3.zero;
	private Vector3 redDirection = Vector3.zero;
	private AudioSource[] audio;
	private CharacterController controller;
	private GameObject skeleton;
	private Animation anim2;
	private float length;
	private float halfPlayerHeight;
	public bool useAI;

	void Awake()
	{
		controller = gameObject.AddComponent<CharacterController>();
		controller.height = 2.0f;
		audio = gameObject.GetComponents<AudioSource>();
		halfPlayerHeight = gameObject.transform.localScale.y * controller.height / 2f;
	}

	void Start()
	{
		skeleton = GameObject.Find("playerController/Skeleton Legacy");
		anim2 = skeleton.GetComponent<Animation>();
	}

	//Collision detection between player and platforms
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(hit.gameObject.name.Equals("Platform"))
		{
			//Ignores white platforms.
			if(hit.gameObject.GetComponent<MeshRenderer>().material.color.Equals(Color.white))
			{
				return;
			}

			//Gets the direction to bounce the player.
			float surfaceAngle = Mathf.Deg2Rad * hit.gameObject.transform.rotation.eulerAngles.z + Mathf.PI / 2f;
			Vector3 surfaceDirection = new Vector3(Mathf.Cos(surfaceAngle), Mathf.Sin(surfaceAngle), 0f);

			if(hit.gameObject.GetComponent<MeshRenderer>().material.color.Equals(Color.blue))
			{
				//Bounces the player and plays the blue platform sound if the player hit either the top or bottom of the platform.
				if(Vector3.Angle(hit.normal, surfaceDirection) <= 45f)
				{
					blueDirection = surfaceDirection * 50f;

					audio[0].Play();
				}
				else if(Vector3.Angle(hit.normal, -surfaceDirection) <= 45f)
				{
					blueDirection = -surfaceDirection * 50f;

					audio[0].Play();
				}
			}
			else if(hit.gameObject.GetComponent<MeshRenderer>().material.color.Equals(Color.red))
			{
				//Calculates direction of acceleration.
				float angle = surfaceAngle - Mathf.PI / 2f;
				Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

				//Accelerates the player and plays the red platform sound if the player hit either the top or bottom of the platform.
				if(Vector3.Angle(hit.normal, surfaceDirection) <= 45f)
				{
					float dot = Vector3.Dot(moveDirection, direction);

					if(dot > 0f)
					{
						redDirection = direction * 50f;

						audio[1].Play();
					}
					else if(dot < 0f)
					{
						redDirection = -direction * 50f;

						audio[1].Play();
					}
				}
			}

			//Marks the platform as visited.
			if(hit.gameObject.transform.childCount > 0)
			{
				Destroy(hit.gameObject.transform.GetChild(0).gameObject);
			}
		}
	}

	//Adds a platform to the list of platforms (called by the DrawReader script).
	public void addPlatform(GameObject platform)
	{
		platforms.Add(platform);
	}

	//Removes a platform from the list of platforms (called by the DrawReader script).
	public void removePlatform(GameObject platform)
	{
		platforms.Remove(platform);
	}

	//Determines whether a line of sight exists between the player and the given collider.
	private bool lineOfSightExists(Collider collider)
	{
		RaycastHit hit;
		Vector3 origin = gameObject.transform.position;
		int intervals = 360;

		for(int i = 0; i < intervals; i++)
		{
			float angle = (float) i * 2f * Mathf.PI / (float) intervals;
			Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

			if(Physics.Raycast(origin, direction, out hit) && hit.collider.Equals(collider))
			{
				return true;
			}
		}

		return false;
	}

	//Determines whether or not one of the given colliders belongs to a spike.
	private bool spikeIsPresent(Collider[] colliders)
	{
		foreach(Collider collider in colliders)
		{
			if(collider.gameObject.name.Contains("spike"))
			{
				return true;
			}
		}

		return false;
	}

	//Determines whether or not to move and/or jump to a door or key.
	private void moveToObject(Vector3 playerPosition, float height, float minJumpLength, float maxJumpLength)
	{
		//Checks colliders at the player's feet.
		float leadDistance = gameObject.transform.localScale.x * controller.radius * length / Mathf.Abs(length);
		Vector3 origin = playerPosition + new Vector3(leadDistance, -halfPlayerHeight, 0f);
		Collider[] colliders = Physics.OverlapSphere(origin, 1f);

		if(colliders.Length > 0 && !spikeIsPresent(colliders))
		{
			//Checks colliders a little further away from the player if a non-spike collider is present.
			leadDistance = (2f * gameObject.transform.localScale.x * controller.radius) * length / Mathf.Abs(length);
			origin = playerPosition + new Vector3(leadDistance, -halfPlayerHeight, 0f);
			colliders = Physics.OverlapSphere(origin, 1f);
			
			if(colliders.Length == 0 || (colliders.Length > 0 && spikeIsPresent(colliders)))
			{
				//Checks colliders at the maximum length the player will try to jump if no non-spike colliders are present.
				leadDistance = (maxJumpLength + gameObject.transform.localScale.x * controller.radius) * length / Mathf.Abs(length);
				origin = playerPosition + new Vector3(leadDistance, -halfPlayerHeight, 0f);
				colliders = Physics.OverlapSphere(origin, 1f);
				
				if(colliders.Length > 0 && !spikeIsPresent(colliders))
				{
					//The player will jump toward the object if a non-spike collider is present.
					length = maxJumpLength * length / Mathf.Abs(length);
				}
				else if(colliders.Length == 0 && height < 0f)
				{
					RaycastHit hit;
					Vector3 back = new Vector3(-length, 0f, 0f).normalized;

					//The player will move toward the object if safe ground is ahead.
					length = (Physics.Raycast(origin, Vector3.down, out hit) && !hit.collider.gameObject.name.Contains("spike")
					          && Physics.Raycast(origin, back, out hit) && !hit.collider.gameObject.name.Contains("spike")
					          && Physics.Raycast(origin + new Vector3(0f, -halfPlayerHeight, 0f), back, out hit) && !hit.collider.gameObject.name.Contains("spike"))
						|| Mathf.Abs(length) <= minJumpLength && height < 0f ? length : 0f;
				}
				else if(Mathf.Abs(length) > minJumpLength || height >= 0f)
				{
					//The player won't move if the object is too far away or too high.
					length = 0f;
				}
			}
		}
	}

	void FixedUpdate()
	{
		anim2.Play(Mathf.Abs(length) == 0.0f ? "Idle" : "Run");
	}

	void Update()
	{
		//Sets player model position and player movement properties.
		skeleton.transform.position = new Vector3(controller.transform.position.x,controller.transform.position.y - gameObject.transform.localScale.y,controller.transform.position.z);
		float moveSpeed = 10f;
		float jumpSpeed = 20f;
		float gravity = 20f;

		//Resets the level if the R key is pressed.
		if(Input.GetKeyDown (KeyCode.R))
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		if(controller.isGrounded)
		{
			bool jump = false;

			if(useAI)
			{
				length = 0f;

				Transform closestWaypoint = null;
				GameObject door = GameObject.Find("door");
				GameObject key = GameObject.Find("technique");
				Vector3 closestPosition = Vector3.zero;
				Vector3 playerPosition = gameObject.transform.position;
				Vector3 doorPosition = door.collider.bounds.center;
				Vector3 keyPosition = key != null ? key.collider.bounds.center : Vector3.zero;
				Vector3 targetPosition = Vector3.zero;
				float height = 0f;
				float minJumpLength = 0f;
				float maxJumpLength = Mathf.Infinity;

				//Finds the closest platform, if any.
				if(platforms.Count > 0)
				{
					float distanceToClosest = Mathf.Infinity;

					foreach(GameObject platform in platforms)
					{
						if(platform.transform.childCount > 0)
						{
							Transform waypoint = platform.transform.GetChild(0);
							float distance = Mathf.Sqrt(Mathf.Pow(waypoint.position.x - playerPosition.x, 2f) + Mathf.Pow(waypoint.position.y - playerPosition.y, 2f));
							
							if(closestWaypoint == null || distance < distanceToClosest)
							{
								closestWaypoint = waypoint;
								closestPosition = waypoint.position;
								distanceToClosest = distance;
							}
						}
					}
				}

				if(closestWaypoint != null)
				{
					//Sets movement attributes for moving to a platform.
					length = closestPosition.x - playerPosition.x;
					height = closestWaypoint.parent.position.y + closestWaypoint.parent.localScale.y / 2f - (playerPosition.y - halfPlayerHeight);
					minJumpLength = 15f;
					maxJumpLength = 25f;
					targetPosition = closestPosition;

					//Marks the platform as visited if the player is sufficiently close to its waypoint.
					if(Mathf.Abs(length) < 0.1f)
					{
						Destroy(closestWaypoint.gameObject);
					}
				}
				else if(key == null && lineOfSightExists(door.collider))
				{
					//Sets movement attributes for moving to a door.
					length = doorPosition.x - playerPosition.x;
					height = doorPosition.y - door.collider.bounds.extents.y - (playerPosition.y - halfPlayerHeight);
					minJumpLength = 20f;
					maxJumpLength = 35f;
					targetPosition = doorPosition;

					//Determines whether and how to move to the door.
					moveToObject(playerPosition, height, minJumpLength, maxJumpLength);
				}
				else if(key != null && lineOfSightExists(key.collider))
				{
					//Sets movement attributes for moving to a key.
					length = keyPosition.x - playerPosition.x;
					height = keyPosition.y - key.collider.bounds.extents.y - (playerPosition.y - halfPlayerHeight);
					minJumpLength = 15f;
					maxJumpLength = 20f;
					targetPosition = keyPosition;

					//Determines whether and how to move to the key.
					moveToObject(playerPosition, height, minJumpLength, maxJumpLength);
				}
				else
				{
					//Doesn't move if no unvisited waypoints are found and no key or door is in the player's line of sight.
					length = 0f;
					height = 0f;
				}

				//Doesn't move if the target is too high.
				length = Mathf.Abs(length) < 0.1f || height > 11f ? 0f : length;
				height = (height < 0.1f && height > 0f) || height > 11f ? 0f : height;
				transform.GetChild(0).rotation = Quaternion.Euler(0f, length == 0f ? transform.GetChild(0).rotation.eulerAngles.y : length / Mathf.Abs(length) * 90f, 0f);

				//Decides whether or not to jump if within the maximum jump length and either the target is higher or outside the minimum jump length.
				if(Mathf.Abs(length) <= maxJumpLength && (height > 0f || Mathf.Abs(length) > minJumpLength))
				{
					//Checks colliders at the player's feet.
					float leadDistance = gameObject.transform.localScale.x * controller.radius * length / Mathf.Abs(length);
					Collider[] colliders = Physics.OverlapSphere(playerPosition + new Vector3(leadDistance, -halfPlayerHeight, 0f), 1f);
					bool onTarget = false;

					jump = true;

					foreach(Collider collider in colliders)
					{
						//Doesn't jump if a collider that isn't a spike is found.
						if(!collider.gameObject.name.Contains("spike"))
						{
							jump = false;
						}

						//Determines whether the player is standing on the platform it's moving towards.
						if(collider.gameObject.transform.childCount > 0 && collider.gameObject.transform.GetChild(0).position.Equals(targetPosition))
						{
							onTarget = true;
						}
					}

					//Jumps if the player isn't on the platform and either the target is higher or a jump was already determined.
					jump = (jump || height > 0f) && !onTarget;
				}

				//Decreases the jump speed to not over jump and sets movement direction.
				jumpSpeed *= height > 0f && Mathf.Abs(length) < minJumpLength ? Mathf.Abs(length) / minJumpLength : 1f;
				moveDirection = new Vector3(length == 0f ? 0f : length / Mathf.Abs(length), 0f, 0f);
			}
			else
			{
				//Moves according to player input if AI is disabled.
				moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
				jump = Input.GetButton("Jump");
			}

			//Determines movement direction and speed by previously determined values.
			moveDirection = transform.TransformDirection(moveDirection) * moveSpeed;
			moveDirection.y = jump ? jumpSpeed : moveDirection.y;
		}

		//Accounts for movement due to red/blue platforms and gravity.
		moveDirection += redDirection + blueDirection;
		redDirection = Vector3.zero;
		blueDirection = Vector3.zero;
		moveDirection.y -= gravity * Time.deltaTime;

		//Moves the player.
		controller.Move(moveDirection * Time.deltaTime);
	}
}
