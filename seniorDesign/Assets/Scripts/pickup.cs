using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
	public bool isActive = true;

	// Use this for initialization
	void Start()
	{
		gameObject.SetActive(true); //active must be set to true

		isActive = true;

	}
	void OnTriggerEnter(Collider other)
	{
		gameObject.GetComponent<AudioSource>().Play();

		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<SphereCollider>().enabled = false;
		isActive = false;
	}
	                                 
	// Update is called once per frame
	void Update()
	{
		if(gameObject.GetComponent<AudioSource>().time > 2f && !isActive)
		{
			gameObject.GetComponent<MeshRenderer>().enabled = true;
			gameObject.GetComponent<SphereCollider>().enabled = true;

			gameObject.SetActive(false); //active must be set to false
		}
	}

	void setActive(bool active)
	{
		isActive = active;
	}
}
