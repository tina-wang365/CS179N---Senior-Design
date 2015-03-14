using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OnClickButton : MonoBehaviour
{
	public void loadNextScene()
	{
		Application.LoadLevel("level1");
	}
}
