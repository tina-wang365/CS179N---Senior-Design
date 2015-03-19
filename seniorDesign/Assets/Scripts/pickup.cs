using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
	public bool isActive = true;

	void Start()
	{
		gameObject.SetActive(true);

		isActive = true;

	}
	void OnTriggerEnter(Collider other)
	{
		//Plays key pickup sound.
		gameObject.GetComponent<AudioSource>().Play();

		//Disables the key's renderer and collider.
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<SphereCollider>().enabled = false;
		isActive = false;
	}
	                                 
	void Update()
	{
		//Stops playing key pickup sound and deactivates the key game object.
		if(gameObject.GetComponent<AudioSource>().time > 2f && !isActive)
		{
			gameObject.GetComponent<MeshRenderer>().enabled = true;
			gameObject.GetComponent<SphereCollider>().enabled = true;

			gameObject.SetActive(false);
		}
	}

	void setActive(bool active)
	{
		isActive = active;
	}
}
