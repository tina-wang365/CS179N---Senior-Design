using UnityEngine;
using System.Collections;
using System;

public class spawner : MonoBehaviour {
	//setup position of where to spawn
	//public float x, y, z;
	//public bool levelEnd = false;
	private string[] levels;
	public static int level = 0;
	//setup public variable to represent which trigger activated
	void Start()
	{
		levels = new string[] {"tutorial1", "level1", "level2", "level3", "level6", "level7", "endScene"};
	}
	
	void OnTriggerStay(Collider other) {
		if (other.gameObject.name == "playerController" && 
		    (this.gameObject.name == "spikes" || this.gameObject.name == "mediumSpikes3x1"))
		{
			Debug.Log ("Spikes triggered by player!\n");
			//other.gameObject.transform.position = new Vector3 (x, y, z);
			//other.gameObject.rigidbody.velocity = new Vector3 (0, 0, 0);
			Debug.Log ("Level = " + level + "\n");
			Application.LoadLevel (levels[level]);
		} else if (other.gameObject.name == "playerController" && this.gameObject.name == "door") {
			Debug.Log ("Door triggered by player!\n");
			Debug.Log ("Level = " + level + "\n");
			level++;
			Application.LoadLevel (levels[level]);
			
		}
	}
}