using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	private List<GameObject> platforms = new List<GameObject>();
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 blueDirection = Vector3.zero;
	private Vector3 redDirection = Vector3.zero;
	public bool useAI;

	void Awake()
	{
		gameObject.AddComponent<CharacterController>();
	}

	// collision detection between player and various objects
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(hit.gameObject.name.Equals("Platform"))
		{
			float surfaceAngle = Mathf.Deg2Rad * hit.gameObject.transform.rotation.eulerAngles.z + Mathf.PI / 2f;
			Vector3 surfaceDirection = new Vector3(Mathf.Cos(surfaceAngle), Mathf.Sin(surfaceAngle), 0f);

			if(hit.gameObject.GetComponent<MeshRenderer>().material.color.Equals(Color.blue))
			{
				if(Vector3.Angle(hit.normal, surfaceDirection) <= 45f)
				{
					blueDirection = surfaceDirection * 50f;
				}
				else if(Vector3.Angle(hit.normal, -surfaceDirection) <= 45f)
				{
					blueDirection = -surfaceDirection * 50f;
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
						redDirection = direction * 100f;
					}
					else if(dot < 0f)
					{
						redDirection = -direction * 100f;
					}
				}
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
		int intervals = 25;

		for(int i = 0; i < intervals; i++)
		{
			float angle = (float) i * 2f * Mathf.PI / (float) intervals;
			Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

			if(Physics.Raycast(gameObject.transform.position, direction, out hit) && hit.collider.Equals(collider))
			{
				return true;
			}
		}

		return false;
	}

	void Update()
	{
		CharacterController controller = gameObject.GetComponent<CharacterController>();
		float moveSpeed = 10f;
		float jumpSpeed = 20f;
		float gravity = 20f;
		
		if(controller.isGrounded)
		{
			bool jump = false;

			if(useAI)
			{
				float length = 0f;
				float height = 0f;

				if(platforms.Count > 0)
				{
					Transform closestWaypoint = null;
					Vector3 closestPosition = Vector3.zero;
					Vector3 playerPosition = gameObject.transform.position;
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

					GameObject door = GameObject.Find("door");
					GameObject key = GameObject.Find("technique");
					Vector3 doorPosition = door.transform.position;
					Vector3 keyPosition = key != null ? key.transform.position : Vector3.zero;
					Vector3 targetPosition;
					float minJumpLength = 0f;
					float doorAttractDistance = 30f;
					float keyAttractDistance = 10f;
					float distanceToDoor = Mathf.Sqrt(Mathf.Pow(doorPosition.x - playerPosition.x, 2f) + Mathf.Pow(doorPosition.y - playerPosition.y, 2f)) - doorAttractDistance;
					float distanceToKey = key != null ? Mathf.Sqrt(Mathf.Pow(keyPosition.x - playerPosition.x, 2f)
						+ Mathf.Pow(keyPosition.y - playerPosition.y, 2f)) - keyAttractDistance : Mathf.Infinity;

					if((distanceToClosest < distanceToDoor || key != null) && distanceToClosest < distanceToKey)
					{
						length = closestPosition.x - playerPosition.x;
						height = closestPosition.y - playerPosition.y;
						minJumpLength = platforms[platforms.Count - 1].transform.localScale.x;
						targetPosition = closestPosition;

						if(Mathf.Abs(length) < 0.1f)
						{
							Destroy(closestWaypoint.gameObject);
						}
					}
					else if(key == null && (closestWaypoint != null || distanceToDoor <= doorAttractDistance) && lineOfSightExists(door.collider))
					{
						length = doorPosition.x - playerPosition.x;
						height = doorPosition.y - playerPosition.y;
						minJumpLength = 15f;
						targetPosition = doorPosition;
					}
					else if(key != null && (closestWaypoint != null || distanceToKey <= keyAttractDistance))
					{
						length = keyPosition.x - playerPosition.x;
						height = keyPosition.y - playerPosition.y;
						minJumpLength = 15f;
						targetPosition = keyPosition;
					}

					length = Mathf.Abs(length) < 0.1f ? 0f : length;

					if(Mathf.Abs(length) <= 25f && (height > 0f || Mathf.Abs(length) > minJumpLength))
					{
						jump = height > 0f || Physics.OverlapSphere(playerPosition + (playerPosition - targetPosition) / 2f, 1f).Length == 0;
					}
				}
				else
				{
					length = 0f;
					height = 0f;
				}

				moveDirection = new Vector3(length == 0f ? 0f : length / Mathf.Abs(length), 0f, 0f);
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
	}
}
