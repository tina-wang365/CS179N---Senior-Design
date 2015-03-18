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
		//controller.center = new Vector3 (controller.center.x, controller.center.y + 1.5f, controller.center.z);
		audio = gameObject.GetComponents<AudioSource>();
		halfPlayerHeight = gameObject.transform.localScale.y * controller.height / 2f;
	}

	void Start()
	{
		skeleton = GameObject.Find("playerController/Skeleton Legacy");
		//anim = skeleton.GetComponent<Animator> ();
		anim2 = skeleton.GetComponent<Animation> ();
	}

	// collision detection between player and various objects
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
//		if(hit.gameObject.name.Equals("spike"))
//		{
//			Debug.Log("Dead ANIMAITON");
//			anim2.Play("Dead");
//		}
		if(hit.gameObject.name.Equals("Platform"))
		{
			if(hit.gameObject.GetComponent<MeshRenderer>().material.color.Equals(Color.white))
			{
				return;
			}

			float surfaceAngle = Mathf.Deg2Rad * hit.gameObject.transform.rotation.eulerAngles.z + Mathf.PI / 2f;
			Vector3 surfaceDirection = new Vector3(Mathf.Cos(surfaceAngle), Mathf.Sin(surfaceAngle), 0f);

			if(hit.gameObject.GetComponent<MeshRenderer>().material.color.Equals(Color.blue))
			{
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
				float angle = surfaceAngle - Mathf.PI / 2f;
				Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

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

			if(hit.gameObject.transform.childCount > 0)
			{
				Destroy(hit.gameObject.transform.GetChild(0).gameObject);
			}
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

	private void moveToObject(Vector3 playerPosition, float height, float minJumpLength, float maxJumpLength)
	{
		float leadDistance = gameObject.transform.localScale.x * controller.radius * length / Mathf.Abs(length);
		Vector3 origin = playerPosition + new Vector3(leadDistance, -halfPlayerHeight, 0f);
		Collider[] colliders = Physics.OverlapSphere(origin, 1f);
		
		//Keep walking?
		if(colliders.Length > 0 && !spikeIsPresent(colliders))
		{
			leadDistance = (2f * gameObject.transform.localScale.x * controller.radius) * length / Mathf.Abs(length);
			origin = playerPosition + new Vector3(leadDistance, -halfPlayerHeight, 0f);
			colliders = Physics.OverlapSphere(origin, 1f);
			
			//Jump?
			if(colliders.Length == 0 || (colliders.Length > 0 && spikeIsPresent(colliders)))
			{
				leadDistance = (maxJumpLength + gameObject.transform.localScale.x * controller.radius) * length / Mathf.Abs(length);
				origin = playerPosition + new Vector3(leadDistance, -halfPlayerHeight, 0f);
				colliders = Physics.OverlapSphere(origin, 1f);
				
				if(colliders.Length > 0 && !spikeIsPresent(colliders))
				{
					length = maxJumpLength * length / Mathf.Abs(length);
				}
				else if(colliders.Length == 0 && height < 0f)
				{
					RaycastHit hit;
					Vector3 back = new Vector3(-length, 0f, 0f).normalized;
					
					length = (Physics.Raycast(origin, Vector3.down, out hit) && !hit.collider.gameObject.name.Contains("spike")
					          && Physics.Raycast(origin, back, out hit) && !hit.collider.gameObject.name.Contains("spike")
					          && Physics.Raycast(origin + new Vector3(0f, -halfPlayerHeight, 0f), back, out hit) && !hit.collider.gameObject.name.Contains("spike"))
						|| Mathf.Abs(length) <= minJumpLength && height < 0f ? length : 0f;
				}
				else if(Mathf.Abs(length) > minJumpLength || height >= 0f)
				{
					length = 0f;
				}
			}
		}
	}

	void FixedUpdate()
	{
		//skeleton = GameObject.Find("playerController/Skeleton Legacy");
		//skeleton.transform.position = new Vector3(controller.transform.position.x,controller.transform.position.y - gameObject.transform.localScale.y,controller.transform.position.z);
		//Animator anim = skeleton.GetComponent<Animator> ();

		anim2.Play(Mathf.Abs(length) == 0.0f ? "Idle" : "Run");
		//if(Mathf.Abs(length) > 0.0f)
		//	anim.SetFloat("moving", 1);
		//else
		//	anim.SetFloat("moving",0);
	}

	void Update()
	{
		//skeleton = GameObject.Find("playerController/Skeleton Legacy");
		skeleton.transform.position = new Vector3(controller.transform.position.x,controller.transform.position.y - gameObject.transform.localScale.y,controller.transform.position.z);
		float moveSpeed = 10f;
		float jumpSpeed = 20f;
		float gravity = 20f;

		//Animator anim = skeleton.GetComponent<Animator> ();
		if (Input.GetKeyDown (KeyCode.R))
			Application.LoadLevel (Application.loadedLevel);

		if(controller.isGrounded)
		{
			bool jump = false;

			if(useAI)
			{
				//float length = 0f;
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
					length = closestPosition.x - playerPosition.x;
					height = closestWaypoint.parent.position.y + closestWaypoint.parent.localScale.y / 2f - (playerPosition.y - halfPlayerHeight);
					minJumpLength = 15f;
					maxJumpLength = 25f;
					targetPosition = closestPosition;
					
					if(Mathf.Abs(length) < 0.1f)
					{
						Destroy(closestWaypoint.gameObject);
					}
					
					//Debug.Log(height);
				}
				else if(key == null && lineOfSightExists(door.collider))
				{
					length = doorPosition.x - playerPosition.x;
					height = doorPosition.y - door.collider.bounds.extents.y - (playerPosition.y - halfPlayerHeight);
					minJumpLength = 20f;
					maxJumpLength = 35f;
					targetPosition = doorPosition;

					moveToObject(playerPosition, height, minJumpLength, maxJumpLength);
				}
				else if(key != null && lineOfSightExists(key.collider))
				{
					length = keyPosition.x - playerPosition.x;
					height = keyPosition.y - key.collider.bounds.extents.y - (playerPosition.y - halfPlayerHeight);
					minJumpLength = 15f;
					maxJumpLength = 20f;
					targetPosition = keyPosition;
					
					moveToObject(playerPosition, height, minJumpLength, maxJumpLength);
				}
				else
				{
					length = 0f;
					height = 0f;
				}

				length = Mathf.Abs(length) < 0.1f || height > 11f ? 0f : length;
				height = (height < 0.1f && height > 0f) || height > 11f ? 0f : height;

				//rotation.eulerAngles = transform.GetChild(0).rotation.eulerAngles;
				//transform.GetChild(0).rotation = Quaternion.Euler(0,-90,0);
				//Quaternion temp.eulerAngles = rotation.eulerAngles;
				//rotation.eulerAngles.y = temp.eulerAngles.y * -1;
				//Debug.Log (transform.GetChild(0).rotation.eulerAngles);
				//Debug.Log (transform.GetChild(0).rotation.eulerAngles);
				
				transform.GetChild(0).rotation = Quaternion.Euler(0f, length == 0f ? transform.GetChild(0).rotation.eulerAngles.y : length / Mathf.Abs(length) * 90f, 0f);

				if(Mathf.Abs(length) <= maxJumpLength && (height > 0f || Mathf.Abs(length) > minJumpLength))
				{
					float leadDistance = gameObject.transform.localScale.x * controller.radius * length / Mathf.Abs(length);
					Collider[] colliders = Physics.OverlapSphere(playerPosition + new Vector3(leadDistance, -halfPlayerHeight, 0f), 1f);
					bool onTarget = false;

					jump = true;

					foreach(Collider collider in colliders)
					{
						if(!collider.gameObject.name.Contains("spike"))
						{
							jump = false;
						}

						if(collider.gameObject.transform.childCount > 0 && collider.gameObject.transform.GetChild(0).position.Equals(targetPosition))
						{
							onTarget = true;
						}
					}

					jump = (jump || height > 0f) && !onTarget;
				}

				//Decreasing the jump speed to NOT over jump
				jumpSpeed *= height > 0f && Mathf.Abs(length) < minJumpLength ? Mathf.Abs(length) / minJumpLength : 1f;

				moveDirection = new Vector3(length == 0f ? 0f : length / Mathf.Abs(length), 0f, 0f);
//				if(Mathf.Abs(length) > 0.0f)
//					anim.SetFloat("moving", 1);
//				else
//					anim.SetFloat("moving",0);
			}
			else
			{
				moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
				jump = Input.GetButton("Jump");
			}

			moveDirection = transform.TransformDirection(moveDirection) * moveSpeed;
			moveDirection.y = jump ? jumpSpeed : moveDirection.y;
		}

		moveDirection += redDirection + blueDirection;
		redDirection = Vector3.zero;
		blueDirection = Vector3.zero;
		moveDirection.y -= gravity * Time.deltaTime;
		
		controller.Move(moveDirection * Time.deltaTime);
		//Debug.Log(controller.velocity);
		
		//skeleton = GameObject.Find("playerController/Skeleton Legacy");
		//skeleton.transform.position = new Vector3(controller.transform.position.x,controller.transform.position.y - 2.5f,controller.transform.position.z);
		//Animator anim = skeleton.GetComponent<Animator> ();
	
		//anim.SetFloat("speed", Mathf.Abs(controller.velocity.x));

//		foreach(AnimationState state in anim)
//		{
//			Debug.Log(state.name);
//		}
//		if(controller.velocity.x != 0)
//			anim.Play(8;
	}
}
