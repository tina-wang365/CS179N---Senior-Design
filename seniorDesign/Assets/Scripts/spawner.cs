using UnityEngine;
using System.Collections;
using System;

public class spawner : MonoBehaviour
{
	//setup position of where to spawn
	private string[] levels;
	public static int level = 0;
	public GameObject pickup;
	private int numberOfLevels = 11;

	//setup public variable to represent which trigger activated
	void Start()
	{
		levels = new string[] {"level1", "level2", "level3", "level4", "level5", "level6", "level7", "level8", "level9", "level10", "endScene"};

		GameObject failed = GameObject.Find("You Failed");
		AudioSource[] audio = gameObject.GetComponents<AudioSource>();

		if(audio.Length > 0)
		{
			if(failed == null)
			{
				audio[0].Play();
			}
			else
			{
				audio[1].Play();
				Destroy(failed);
			}
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.name == "playerController" && (this.gameObject.name == "spike" || this.gameObject.name == "mediumSpikes3x1"))
		{
			if(GameObject.Find("You Failed") == null)
			{
				GameObject failed = new GameObject();

				failed.name = "You Failed";

				GameObject.DontDestroyOnLoad(failed);
			}

			Debug.Log("Spikes triggered by player!\n");
			Debug.Log("Level = " + level + "\n");
			Application.LoadLevel(levels[level]);
		}
		else if(other.gameObject.name == "playerController" && this.gameObject.name == "door")
		{
			Debug.Log ("Door triggered by player!\n");
			Debug.Log ("Level = " + level + "\n");

			if(!pickup.GetComponent<Pickup>().isActive)
			{
				if(level < numberOfLevels - 1)
				{
					level++;
				}
				else
				{
					level = 0;
				}

				Application.LoadLevel(levels[level]);
				pickup.SetActive(true);
			}
			else
			{
				Debug.Log ("The door is locked. You need a key.\n");
			}
		}
	}
}