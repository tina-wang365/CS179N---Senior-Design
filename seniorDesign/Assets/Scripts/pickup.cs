using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	public bool levelRestart = false;
	public bool isActive = true;
	// Use this for initialization
	void Start () {
		if (levelRestart) {
			this.gameObject.SetActive (true); //active must be set to true
			isActive = true;
			levelRestart = false;
		}

	}
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.name == "playerController") {
			this.gameObject.SetActive (false); //active must be set to false
			isActive = false;
		}
	}
	                                 
	// Update is called once per frame
	void Update () {
	
	}
}
