using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	public bool levelRestart = false;

	// Use this for initialization
	void Start () {
		this.gameObject.SetActive (true);

	}
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.name == "playerController") {
			this.gameObject.SetActive (false); //active must be set to false
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
