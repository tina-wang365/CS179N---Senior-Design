﻿using UnityEngine;
using System.Collections;
using System;

public class spawner : MonoBehaviour
{
	//setup position of where to spawn
	private string[] levels;
	public static int level = 0;
	public GameObject pickup;
	private int numberOfLevels = 11;
	//public GameObject character;
	//private Animation animDead;

	//setup public variable to represent which trigger activated
	void Start()
	{
		levels = new string[] {"level1", "level2", "level3", "level4", "level5", "level6", "level7", "level8", "level9", "level10", "endScene"};

		GameObject failed = GameObject.Find("You Failed");
		AudioSource[] audio = gameObject.GetComponents<AudioSource>();
		//character = GameObject.Find("playerController/Skeleton Legacy");
		//animDead = character.GetComponent<Animation> ();

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
			//animDead.Play ("Dead");

			if(GameObject.Find("You Failed") == null)
			{
				GameObject failed = new GameObject();

				failed.name = "You Failed";

				GameObject.DontDestroyOnLoad(failed);
			}

			Debug.Log("Spikes triggered by player!\n");
			Debug.Log("Level = " + level + "\n");
			//yield WaitForSeconds(3.0f);
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