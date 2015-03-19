using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DrawReader : MonoBehaviour
{
	private LineRenderer line;
	private bool isMousePressed;
	private List<Vector3> points;
	private List<GameObject> platforms;
	private Color platformColor;
	private AudioSource[] audio;
	public float maxDistance;
	public int maxPlatforms;
	public bool enableBlue;
	public bool enableRed;
	public GameObject player;
	public GameObject canvas;

	void Awake()
	{
		points = new List<Vector3>();
		platforms = new List<GameObject>();
		isMousePressed = false;
		platformColor = Color.white;
		line = gameObject.AddComponent<LineRenderer>();
		line.material = new Material(Shader.Find("Particles/Additive"));
		line.useWorldSpace = true;
		audio = gameObject.GetComponents<AudioSource>();

		line.SetVertexCount(0);
		line.SetWidth(0.5f, 0.5f);

		//Disables the red button if specified by its corresponding public variable.
		if(!enableRed)
		{
			disableButton("RedButton");
		}

		//Disables the blue button if specified by its corresponding public variable.
		if(!enableBlue)
		{
			disableButton("BlueButton");
		}

		//Disables the white button if the other two buttons have both been disabled.
		if(!enableRed && !enableBlue)
		{
			disableButton("WhiteButton");
		}
	}

	void Update() 
	{
		//Disables the platform's light if its initial particle animation has stopped.
		foreach(GameObject platform in platforms)
		{
			platform.light.range = platform.particleSystem.isStopped ? 0f : platform.light.range;
		}

		if(Input.GetMouseButtonDown(0))
		{
			Color transparentPlatformColor = new Color(platformColor.r, platformColor.g, platformColor.b, 0.75f);

			isMousePressed = true;

			//Sets line properties and begins playing electricity sound effect.
			line.SetColors(transparentPlatformColor, transparentPlatformColor);
			line.SetVertexCount(2);
			audio[2].Play();
		}
		else if(Input.GetMouseButtonUp(0))
		{
			//Creates a platform, resets line (and associated point) properties and stops playing the electricity sound effect.
			createPlatform();
			line.SetVertexCount(0);
			points.RemoveRange(0, points.Count);
			audio[2].Stop();

			isMousePressed = false;
		}
		else if(Input.GetMouseButtonUp(1))
		{
			//Casts a ray from the mouse position to determine which object was right-clicked.
			Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			//If the ray hit a platform, delete it.
			if(Physics.Raycast(camRay, out hit) && hit.collider != null)
			{
				removeClicked(hit);
			}
		}

		//Adds points to the drawing while the left mouse button is down.
		if(isMousePressed)
		{
			//Gets a point from the current mouse position.
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			mousePos.z = 0f;
			
			if(!points.Contains(mousePos)) 
			{
				//Calculates the distance between the mouse position and the first point.
				float distance = points.Count > 0 ? Mathf.Sqrt(Mathf.Pow(mousePos.x - points[0].x, 2f) + Mathf.Pow(mousePos.y - points[0].y, 2f)) : 0f;

				//Removes the second point so it can be replaced.
				if(points.Count == 2)
				{
					points.RemoveAt(1);
				}

				//Adds the furthest valid point to the list of points.
				if(distance < maxDistance)
				{
					points.Add(mousePos);
				}
				else
				{
					float x = mousePos.x - points[0].x;
					float angle = Mathf.Atan((mousePos.y - points[0].y) / x) + (x < 0f ? Mathf.PI : 0f);

					points.Add(new Vector3(Mathf.Cos(angle) * maxDistance + points[0].x, Mathf.Sin(angle) * maxDistance + points[0].y, 0f));
				}

				//Sets the line's position based on the current points.
				line.SetPosition(0, points[0]);

				//Creates lightning effect.
				if(points.Count > 1)
				{
					line.SetVertexCount((int) maxDistance);
					lightningEffect();
					line.SetPosition((int) maxDistance - 1, points[points.Count - 1]);
				}
				else
				{
					line.SetPosition(1, points[points.Count - 1]);
				}
			}
		}
	}

	void FixedUpdate()
	{
		//Modifies and activates the particle system of the next platform to be deleted.
		if(platforms.Count == maxPlatforms && maxPlatforms > 0 && platforms[0].particleSystem.isStopped)
		{
			platforms[0].particleSystem.emissionRate = 10f;
			platforms[0].particleSystem.playbackSpeed = 1f;
			platforms[0].particleSystem.startSize = 1f;
			platforms[0].particleSystem.Play();
		}

		//Produces lightning effect for line renderer between Update() calls.
		if(points.Count > 1)
		{
			lightningEffect();
		}

		//Plays background music.
		if(Time.timeSinceLevelLoad > 1f && !audio[1].isPlaying)
		{
			audio[1].Play();
		}
	}

	//Produces lightning effect while a platform is being drawn.
	private void lightningEffect()
	{
		//Creates random points inside a series of circles around points at regular intervals between the beginning and end of the drawn line.
		for(int i = 1; i < (int) maxDistance - 1; i++)
		{
			Vector2 pos =  points[0] + (points[1] - points[0]) * (float) i / maxDistance;
			Vector2 rand = Random.insideUnitCircle + pos;
			
			line.SetPosition(i, new Vector3(rand.x, rand.y, 0f));
		}
	}

	//Disables a UI button.
	private void disableButton(string name)
	{
		for(int i = 0; i < canvas.GetComponent<RectTransform>().childCount; i++)
		{
			Transform button = canvas.GetComponent<RectTransform>().GetChild(i);

			//Disables the button with the specified name.
			if(button.name.Equals(name))
			{
				button.GetComponent<Button>().interactable = false;
				button.GetComponent<Image>().enabled = false;
				break;
			}
		}
	}

	//Removes a platform when right-clicked.
	private bool removeClicked(RaycastHit hit)
	{
		//Searches the list of platforms for the one that was right-clicked.
		foreach(GameObject platform in platforms)
		{
			if(hit.collider.Equals(platform.collider))
			{
				//Removes and destroys the right-clicked platform.
				platforms.Remove(platform);
				player.GetComponent<PlayerController>().removePlatform(platform);
				Destroy(platform);
				return true;
			}
		}

		return false;
	}

	//Creates a platform at the specified position.
	private void createPlatform()
	{
		//If fewer that two points are in the list of points, no platform is created.
		if(points.Count < 2)
		{
			return;
		}

		//If the left mouse button was let go over a button, no platform is created.
		for(int i = 0; i < canvas.GetComponent<RectTransform>().childCount; i++)
		{
			Transform button = canvas.GetComponent<RectTransform>().GetChild(i);
			Vector3 position = Camera.main.ScreenToWorldPoint(button.position);

			if(Mathf.Sqrt(Mathf.Pow(points[1].x - position.x, 2f) + Mathf.Pow(points[1].y - position.y, 2f)) < 3f)
			{
				return;
			}
		}

		//Creates a platform and determines its size and orientation.
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		float opp = points [points.Count - 1].y - points [0].y;
		float adj = points [points.Count - 1].x - points [0].x;
		float length = Mathf.Sqrt(Mathf.Pow(adj, 2f) + Mathf.Pow(opp, 2f));
		float angle = Mathf.Atan(opp / adj) * 180f / Mathf.PI;

		//Adds a particle system and light to the platform.
		platform.AddComponent<ParticleSystem>();
		platform.AddComponent<Light>();

		//Sets properties of the platform and its particle system and light.
		platform.name = "Platform";
		platform.particleSystem.loop = false;
		platform.particleSystem.startLifetime = 1f;
		platform.particleSystem.emissionRate = 10f * length;
		platform.particleSystem.playbackSpeed = 25f;
		platform.particleSystem.startSize = 10f;
		platform.particleSystem.startColor = platformColor;
		platform.light.color = platformColor;
		platform.light.type = LightType.Point;
		platform.light.intensity = 2f;
		platform.light.range = 100f;
		platform.GetComponent<MeshRenderer>().material.color = platformColor;
		platform.transform.position = new Vector3((points[points.Count - 1].x + points[0].x) / 2f, (points[points.Count - 1].y + points[0].y) / 2f, 0f);
		platform.transform.localScale = new Vector3(length < 3f ? 3f : length, 3f, 30f);

		//Adds a waypoint to the platform, rotates the platform and adds the platform to the list of platforms.
		addWaypoint(platform);
		platform.transform.Rotate(0f, 0f, Mathf.Abs(angle) > 5f ? angle : 0f);
		addPlatform(platform);
	}

	//Adds an AI waypoint as a child of the given platform.
	private void addWaypoint(GameObject platform)
	{
		GameObject waypoint = GameObject.CreatePrimitive(PrimitiveType.Cube);

		//Removes unnecessary components from the waypoint.
		Destroy(waypoint.rigidbody);
		Destroy(waypoint.renderer);

		//Sets waypoint properties.
		waypoint.name = "Waypoint";
		waypoint.transform.position = platform.transform.position + new Vector3(0f, platform.transform.localScale.y / 1.5f, 0f);
		waypoint.transform.localScale = new Vector3(1f, 1f, platform.transform.localScale.z);
		waypoint.transform.parent = platform.transform;
		waypoint.collider.isTrigger = true;
	}

	//Adds a platform to the specified list.
	private void addPlatform(GameObject platform)
	{
		//The platform is destroyed if it intersects the player.
		if(platform.collider.bounds.Intersects(player.collider.bounds))
		{
			Destroy(platform);
		}
		else
		{
			//Adds the platform to the list of platforms and plays an explosion sound.
			platforms.Add(platform);
			player.GetComponent<PlayerController>().addPlatform(platform);
			audio[0].Play();

			//Deletes the oldest platform if the list of platforms contains too many platforms.
			if(platforms.Count > maxPlatforms)
			{
				GameObject oldGameObject = platforms[0];
				
				platforms.Remove(oldGameObject);
				player.GetComponent<PlayerController>().removePlatform(oldGameObject);
				Destroy(oldGameObject);
			}
		}
	}

	//Sets the color of new platforms based on button input.
	public void setColor(string color)
	{
		switch(color)
		{
			case "White":
				platformColor = Color.white;
				break;
			case "Red":
				platformColor = Color.red;
				break;
			case "Blue":
				platformColor = Color.blue;
				break;
			default:
				break;
		}
	}
}
