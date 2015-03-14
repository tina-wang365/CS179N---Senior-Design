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
	public bool useAI;
	float length;

	GameObject skeleton;
	Animator anim;

	void Awake()
	{
		controller = gameObject.AddComponent<CharacterController>();
		controller.height = 2.5f;
		controller.center = new Vector3 (controller.center.x, controller.center.y + 1.5f, controller.center.z);
		audio = gameObject.GetComponents<AudioSource>();
	}

	void Start()
	{
		skeleton = GameObject.Find("playerController/Skeleton Legacy");
		anim = skeleton.GetComponent<Animator> ();
	}

	// collision detection between player and various objects
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
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
		int intervals = 25;

		origin.y += gameObject.transform.localScale.y * controller.radius / 2f;

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

	void FixedUpdate()
	{
		//skeleton = GameObject.Find("playerController/Skeleton Legacy");
		skeleton.transform.position = new Vector3(controller.transform.position.x,controller.transform.position.y + 1.0f,controller.transform.position.z);
		//Animator anim = skeleton.GetComponent<Animator> ();

		if(Mathf.Abs(length) > 0.0f)
			anim.SetFloat("moving", 1);
		else
			anim.SetFloat("moving",0);
		
	}

	void Update()
	{
		//skeleton = GameObject.Find("playerController/Skeleton Legacy");
		//skeleton.transform.position = new Vector3(controller.transform.position.x,controller.transform.position.y - 2.5f,controller.transform.position.z);
		float moveSpeed = 10f;
		float jumpSpeed = 20f;
		float gravity = 20f;

		//Animator anim = skeleton.GetComponent<Animator> ();
		
		if(controller.isGrounded)
		{
			bool jump = false;

			if(useAI)
			{
				//float length = 0f;
				length = 0f;
				float height = 0f;

				if(platforms.Count > 0)
				{
					Transform closestWaypoint = null;
					Vector3 closestPosition = Vector3.zero;
					Vector3 playerPosition = gameObject.transform.position;
					float distanceToClosest = Mathf.Infinity;
					float playerRadius = gameObject.transform.localScale.x * controller.radius;

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

					GameObject door = GameObject.Find("door");
					GameObject key = GameObject.Find("technique");
					Vector3 doorPosition = door.transform.position;
					Vector3 keyPosition = key != null ? key.transform.position : Vector3.zero;
					Vector3 targetPosition = Vector3.zero;
					float minJumpLength = 0f;
					float maxJumpLength = Mathf.Infinity;
					float doorAttractDistance = 40f;
					float keyAttractDistance = 25f;
					float distanceToDoor = Mathf.Sqrt(Mathf.Pow(doorPosition.x - playerPosition.x, 2f) + Mathf.Pow(doorPosition.y - playerPosition.y, 2f));
					float distanceToKey = key != null ? Mathf.Sqrt(Mathf.Pow(keyPosition.x - playerPosition.x, 2f)
						+ Mathf.Pow(keyPosition.y - playerPosition.y, 2f)) : Mathf.Infinity;

					if(key == null && distanceToDoor <= doorAttractDistance && lineOfSightExists(door.collider))
					{
						length = doorPosition.x - playerPosition.x;
						height = doorPosition.y - door.transform.localScale.y / 2f - playerPosition.y + 4f + playerRadius;
						minJumpLength = 20f;
						maxJumpLength = 35f;
						targetPosition = doorPosition;
					}
					else if(key != null && distanceToKey <= keyAttractDistance && lineOfSightExists(key.collider))
					{
						length = keyPosition.x - playerPosition.x;
						height = keyPosition.y - playerPosition.y + 4f;
						minJumpLength = 20f;
						maxJumpLength = 25f;
						targetPosition = keyPosition;
					}
					else if(closestWaypoint != null)
					{
						length = closestPosition.x - playerPosition.x;
						height = closestWaypoint.parent.position.y + closestWaypoint.parent.localScale.y / 2f - (playerPosition.y + 4f - playerRadius);
						minJumpLength = 15f;
						maxJumpLength = 25f;
						targetPosition = closestPosition;
						
						if(Mathf.Abs(length) < 0.1f)
						{
							Destroy(closestWaypoint.gameObject);
						}

						Debug.Log(height);
						
						if(height > 10f)
						{
							length = 0f;
							height = 0f;
						}
					}

					length = Mathf.Abs(length) < 0.1f ? 0f : length;

					//rotation.eulerAngles = transform.GetChild(0).rotation.eulerAngles;
					//transform.GetChild(0).rotation = Quaternion.Euler(0,-90,0);
					//Quaternion temp.eulerAngles = rotation.eulerAngles;
					//rotation.eulerAngles.y = temp.eulerAngles.y * -1;
					//Debug.Log (transform.GetChild(0).rotation.eulerAngles);
					//Debug.Log (transform.GetChild(0).rotation.eulerAngles);
					
					if(length < 0)
					{
						transform.GetChild(0).rotation = Quaternion.Euler(0,-90,0);
					}
					else
					{
						transform.GetChild(0).rotation = Quaternion.Euler(0,90,0);
					}

					if(Mathf.Abs(length) <= maxJumpLength && (height > 0f || Mathf.Abs(length) > minJumpLength))
					{
						Collider[] colliders = Physics.OverlapSphere(playerPosition + new Vector3(playerRadius * length / Mathf.Abs(length), -playerRadius, 0f), 1f);
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
					if(height > 0f && Mathf.Abs(length) < minJumpLength)
					{	
						jumpSpeed *= Mathf.Abs(length) / minJumpLength;
					}
				}
				else
				{
					length = 0f;
					height = 0f;
				}

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
