using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public int speed;

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis("Horizontal");
		//float moveVertical = Input.GetAxis("Vertical");
		
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, 0.0f);
		
		rigidbody.AddForce(movement * speed * Time.deltaTime);
	}
}
