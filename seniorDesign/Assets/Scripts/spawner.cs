using UnityEngine;
using System.Collections;
using System;

public class spawner : MonoBehaviour {
	//setup position of where to spawn
	public float x, y, z;
	public bool levelEnd = false;
	public GameObject otherGameObject;
	//setup public variable to represent which trigger activated
	void Start()
	{

	}
	
	void OnTriggerStay(Collider other) {
		if (other.gameObject.name == "playerController" && this.gameObject.name == "spikes") {
			Debug.Log ("Spikes triggered by player!\n");
			//other.gameObject.transform.position = new Vector3 (x, y, z);
			//other.gameObject.rigidbody.velocity = new Vector3 (0, 0, 0);
			Application.LoadLevel ("tutorial1");

		} else if (other.gameObject.name == "playerController" && this.gameObject.name == "door") {
			Debug.Log ("Door triggered by player!\n");
			Application.LoadLevel ("level1");
		}
	}
}