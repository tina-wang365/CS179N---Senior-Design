using UnityEngine;
using System.Collections;

public class inviScript : MonoBehaviour {

	public GameObject playerAI;
	public AIScript ai;
	public bool inviJump;
	public bool inviGround;

	// Use this for initialization
	void Start () {
		playerAI = GameObject.Find("playerController");
		ai = playerAI.GetComponent<AIScript> ();
		Debug.Log ("WWWTTTFFFF");
		inviJump = false;
		inviGround = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		Debug.Log ("INVI FU EXECUTION!!!!!!!!!!!!!!!");
		if(ai.x < ai.nextLoc.x)
		{
			Debug.Log("moving right!!!!!!!!!!!");
			if(ai.createInvi)
			{
				rigidbody.transform.position = new Vector3 (playerAI.transform.position.x + 6,
				                                            playerAI.transform.position.y,
				                                            playerAI.transform.position.z);
			}
		}

		else if(ai.x > ai.nextLoc.x)
		{
			Debug.Log("moving left!!!!!!!!!!!");
			if(ai.createInvi)
			{
				rigidbody.transform.position = new Vector3 (playerAI.transform.position.x - 6,
				                                            playerAI.transform.position.y,
				                                            playerAI.transform.position.z);
			}
		}
	}

	void OnCollisionExit (Collision other)
	{
//		//FREE FALL CASES
		if(other.gameObject.name == "Platform")
		{
//			//WORKS
			Debug.Log("LEAVING PLATFORMmmmmmmmmmmmmmmmmmmmm");
//			//onGround = false;
//			//ai.actionJump = true;
			inviJump = true;
			inviGround = false;
		}
		
		else if(other.gameObject.name.IndexOf("Floor") != -1)
		{
//			//WORKS
			Debug.Log("LEAVING PLATFORM or LEAVING FLOOOOOOOOOOOOOOOR");
//			//onGround = false;
//			//ai.actionJump = true;
			inviJump = true;
			inviGround = false;
		}
	}

	void OnCollisionEnter (Collision other)
	{
		if (other.gameObject.name == "Platform")
		{
			Debug.Log("INVI PLAYEEEER ENTERINGGGG PLATFOOORRMMM");
			ContactPoint contact = other.contacts[0];
			Vector3 pos = contact.point;
			Debug.Log("contact PLAT here: " + pos);
			inviGround = true;
			inviJump = false;
		}

		else if (other.gameObject.name.IndexOf ("Floor") != -1)
		{
			Debug.Log("INVI PLAYEEEER ENTERINGGGG FLLLLOOOORRRR");
			ContactPoint contact = other.contacts[0];
			Vector3 pos = contact.point;
			Debug.Log("contact FLOOR here: " + pos);
			inviGround = true;
			inviJump = false;
		}

		//no collision
		else{
			Debug.Log ("NO COLLLISSSSIIIIOONNNNNNN");
			inviJump = true;
			inviGround = false;
		}
	}
}
