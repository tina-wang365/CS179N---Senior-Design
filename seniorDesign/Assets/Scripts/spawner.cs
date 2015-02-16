using UnityEngine;
using System.Collections;
using System;

public class spawner : MonoBehaviour {
	//setup position of where to spawn
	public float x, y, z;
	public bool levelEnd = false;
	//setup public variable to represent which trigger activated
	void Start()
	{
	}
	/*void OnTriggerEnter(Collider other) {
		if (other.gameObject.name == "playerController") {
			Debug.Log ("player ENTERS trigger!\n");
		}
	}*/

	void OnTriggerStay(Collider other) {
		if (other.gameObject.name == "playerController") {
			Debug.Log ("player is WITHIN trigger!\n");
			other.gameObject.transform.position = new Vector3(x, y, z);
			other.gameObject.rigidbody.velocity = new Vector3(0, 0, 0);
		}
	}
}