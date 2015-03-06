using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	private List<GameObject> platforms = new List<GameObject>();
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 blueDirection = Vector3.zero;
	private Vector3 redDirection = Vector3.zero;

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

	void Update()
	{
		CharacterController controller = gameObject.GetComponent<CharacterController>();
		float moveSpeed = 10f;
		float jumpSpeed = 20f;
		float gravity = 20f;
		
		if(controller.isGrounded)
		{
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
			moveDirection = transform.TransformDirection(moveDirection) * moveSpeed;
			moveDirection.y = Input.GetButton("Jump") ? jumpSpeed : moveDirection.y;
		}

		moveDirection += redDirection + blueDirection;
		redDirection = Vector3.zero;
		blueDirection = Vector3.zero;
		moveDirection.y -= gravity * Time.deltaTime;
		
		controller.Move(moveDirection * Time.deltaTime);
	}
}
