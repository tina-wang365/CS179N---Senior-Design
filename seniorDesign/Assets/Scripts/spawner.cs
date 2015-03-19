using UnityEngine;
using System.Collections;
using System;

public class spawner : MonoBehaviour
{
	private string[] levels;
	public static int level = 0;
	public GameObject pickup;
	private int numberOfLevels = 11;

	void Start()
	{
		levels = new string[] {"level1", "level2", "level3", "level4", "level5", "level6", "level7", "level8", "level9", "level10", "endScene"};

		//Finds the marker to determine whether or not the player just lost.
		GameObject failed = GameObject.Find("You Failed");
		AudioSource[] audio = gameObject.GetComponents<AudioSource>();

		if(audio.Length > 0)
		{
			if(failed == null)
			{
				//Plays the level start sound if the player just started or reset the level.
				audio[0].Play();
			}
			else
			{
				//Plays the player lost sound if the player just lost.
				audio[1].Play();
				Destroy(failed);
			}

			for(int i = 0; i < levels.Length; i++)
			{
				if(Application.loadedLevelName.Equals(levels[i]))
				{
					level = i;
					break;
				}
			}
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.name == "playerController" && (gameObject.name == "spike" || gameObject.name == "mediumSpikes3x1" || gameObject.name == "enemy"))
		{
			//Creates an object that will survive reloading the level and notify the script to play the losing sound.
			if(GameObject.Find("You Failed") == null)
			{
				GameObject failed = new GameObject();

				failed.name = "You Failed";

				GameObject.DontDestroyOnLoad(failed);
			}

			//Reloads the level if the player touches spikes or an enemy.
			Application.LoadLevel(levels[level]);
		}
		else if(other.gameObject.name == "playerController" && this.gameObject.name == "door")
		{
			if(pickup == null)
			{
				//This probably shouldn't be here.
				Destroy(other.gameObject);
			}
			else if(!pickup.GetComponent<Pickup>().isActive)
			{
				if(level < numberOfLevels - 1)
				{
					level++;
				}
				else
				{
					level = 0;
				}

				//Advances to the next level if the player has collected the key and reaches the door.
				Application.LoadLevel(levels[level]);
				pickup.SetActive(true);
			}
		}
	}
}