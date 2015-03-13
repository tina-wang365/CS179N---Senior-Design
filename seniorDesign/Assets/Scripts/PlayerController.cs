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
						redDirection = direction * 50f;
					}
					else if(dot < 0f)
					{
						redDirection = -direction * 50f;
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

		origin.y += gameObject.GetComponent<CharacterController>().radius / 2f;

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

	void Update()
	{
		CharacterController controller = gameObject.GetComponent<CharacterController>();
		float moveSpeed = 10f;
		float jumpSpeed = 20f;
		float gravity = 20f;
		float length = 0f;
		float height = 0f;
		bool jump = false;
		//bool jump;
		
		if(controller.isGrounded)
		{
			//bool jump = false;

			if(useAI)
			{
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
					float maxJumpLength = Mathf.Infinity;
					float doorAttractDistance = 40f;
					float keyAttractDistance = 15f;
					float distanceToDoor = Mathf.Sqrt(Mathf.Pow(doorPosition.x - playerPosition.x, 2f) + Mathf.Pow(doorPosition.y - playerPosition.y, 2f));
					float distanceToKey = key != null ? Mathf.Sqrt(Mathf.Pow(keyPosition.x - playerPosition.x, 2f)
						+ Mathf.Pow(keyPosition.y - playerPosition.y, 2f)) : Mathf.Infinity;

					if(key == null && distanceToDoor <= doorAttractDistance && lineOfSightExists(door.collider))
					{
						length = doorPosition.x - playerPosition.x;
						height = doorPosition.y - door.transform.localScale.y / 2f - playerPosition.y + controller.radius;
						minJumpLength = 20f;
						maxJumpLength = 35f;
						targetPosition = doorPosition;
					}
					else if(key != null && distanceToKey <= keyAttractDistance && lineOfSightExists(key.collider))
					{
						length = keyPosition.x - playerPosition.x;
						height = keyPosition.y - playerPosition.y;
						minJumpLength = 20f;
						maxJumpLength = 25f;
						targetPosition = keyPosition;
					}
					else if(closestWaypoint != null)
					{
						length = closestPosition.x - playerPosition.x;
						height = closestWaypoint.parent.position.y + closestWaypoint.parent.localScale.y - (playerPosition.y - controller.radius);
						minJumpLength = 15f;
						maxJumpLength = 25f;
						targetPosition = closestPosition;
						
						if(Mathf.Abs(length) < 0.1f)
						{
							Destroy(closestWaypoint.gameObject);
						}
						
						if(height > 10f)
						{
							length = 0f;
							height = 0f;
						}
					}

					length = Mathf.Abs(length) < 0.1f ? 0f : length;

					if(Mathf.Abs(length) <= maxJumpLength && (height > 0f || Mathf.Abs(length) > minJumpLength))
					{
						Collider[] hitColliders = Physics.OverlapSphere(playerPosition + new Vector3(minJumpLength*length/Mathf.Abs (length),-5,0), controller.radius + 5);
						int i = 0;
						while (i < hitColliders.Length) {
							Debug.Log ("hitCollier: " + hitColliders[i].name);
							if(hitColliders[i].name != "spike")
							//hitColliders[i].SendMessage("AddDamage");
							i++;
						}
						//jump = height > 0f || Physics.OverlapSphere(playerPosition + (playerPosition - targetPosition) / 2f, controller.radius + 5).Length == 0;
						jump = height > 0f || Physics.OverlapSphere(playerPosition + new Vector3(minJumpLength*length/Mathf.Abs (length),-5,0), controller.radius + 5).Length == 0;

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

			//Decreasing the jump speed to NOT over jump
			if(Mathf.Abs(length) < 10.0f && height > 0)
			{	
				jumpSpeed = 15;
			}
			moveDirection = transform.TransformDirection(moveDirection) * moveSpeed;
			moveDirection.y = jump ? jumpSpeed : moveDirection.y;
		}

		if(Mathf.Abs(length) < 10.0f && jump)
			moveSpeed = 10;

		moveDirection += redDirection + blueDirection;
		redDirection = Vector3.zero;
		blueDirection = Vector3.zero;
		moveDirection.y -= gravity * Time.deltaTime;
		
		controller.Move(moveDirection * Time.deltaTime);
	}
}
