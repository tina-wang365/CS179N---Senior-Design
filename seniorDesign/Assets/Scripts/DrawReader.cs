using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawReader : MonoBehaviour
{
	private LineRenderer line;
	private bool isMousePressed;
	private List<Vector3> points;
	private List<GameObject> platforms;
	private Color platformColor;
	public float maxDistance;
	public int maxPlatforms;
	public GameObject player;

	void Awake()
	{
		points = new List<Vector3>();
		platforms = new List<GameObject>();
		isMousePressed = false;
		line = gameObject.AddComponent<LineRenderer>();
		platformColor = Color.white;
		line.material = new Material(Shader.Find("Particles/Additive"));
		line.useWorldSpace = true;

		line.SetVertexCount(0);
		line.SetWidth(0.5f, 0.5f);
	}
	
	void Update() 
	{
		if(Input.GetMouseButtonDown(0))
		{
			isMousePressed = true;
			
			line.SetColors(platformColor, platformColor);
			line.SetVertexCount(2);
		}
		else if(Input.GetMouseButtonUp(0))
		{
			//Create an object if it was drawn correctly and remove the drawing.
			createPlatform();
			line.SetVertexCount(0);
			points.RemoveRange(0, points.Count);

			isMousePressed = false;
		}
		else if(Input.GetMouseButtonUp(1))
		{
			//Cast a ray from the mouse position to determine which object was right-clicked.
			Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			//If the ray hit something, delete it.
			if(Physics.Raycast(camRay, out hit) && hit.collider != null)
			{
				if(removeClicked(hit, platforms))
				{
					//Display error message?
				}
			}
		}

		//Add points to the drawing while the left mouse button is down.
		if(isMousePressed)
		{
			//Get a point from the current mouse position. The camera should be at position (0, 1, -10) and rotation (0, 0, 0).
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			mousePos.z = 0f;
			
			if(!points.Contains(mousePos)) 
			{
				float distance = points.Count > 0 ? Mathf.Sqrt(Mathf.Pow(mousePos.x - points[0].x, 2f) + Mathf.Pow(mousePos.y - points[0].y, 2f)) : 0f;

				if(points.Count == 2)
				{
					points.RemoveAt(1);
				}

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

				line.SetPosition(0, points[0]);
				line.SetPosition(1, points[points.Count - 1]);
			}
		}
	}

	//Removes a drawn object when right-clicked.
	private bool removeClicked(RaycastHit hit, List<GameObject> list)
	{
		//Checks each game object in the list for the object that was right-clicked.
		foreach(GameObject gameObject in list)
		{
			if(hit.collider.Equals(gameObject.collider))
			{
				//Removes and destroys the right-clicked object.
				list.Remove(gameObject);
				Destroy(gameObject);
				return true;
			}
		}

		return false;
	}

	//Creates a platform at the specified position.
	private void createPlatform()
	{
		if(points.Count < 2)
		{
			return;
		}

		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
		float opp = points [points.Count - 1].y - points [0].y;
		float adj = points [points.Count - 1].x - points [0].x;
		float length = Mathf.Sqrt(Mathf.Pow(adj, 2f) + Mathf.Pow(opp, 2f));
		float angle = Mathf.Atan(opp / adj) * 180f / Mathf.PI;

		//Sets platform properties.
		platform.name = "Platform";
		platform.GetComponent<MeshRenderer>().material.color = platformColor;
		platform.transform.position = new Vector3((points[points.Count - 1].x + points[0].x) / 2f, (points[points.Count - 1].y + points[0].y) / 2f, 0f);
		platform.transform.localScale = new Vector3(length, 3f, 30f);
		addWaypoint(platform); 
		platform.transform.Rotate(0f, 0f, Mathf.Abs(angle) > 5.0 ? angle : 0f);
		addObject(platform, platforms, maxPlatforms);
	}

	private void addWaypoint(GameObject platform)
	{
		GameObject waypoint = GameObject.CreatePrimitive(PrimitiveType.Cube);

		Destroy(waypoint.rigidbody);
		Destroy(waypoint.renderer);

		waypoint.name = "Waypoint";
		waypoint.transform.position = platform.transform.position + new Vector3(0f, platform.transform.localScale.y / 1.5f, 0f);
		waypoint.transform.localScale = new Vector3(1f, 1f, platform.transform.localScale.z);
		waypoint.transform.parent = platform.transform;
		waypoint.collider.isTrigger = true;
	}

	//Adds a drawn object to the specified list.
	private void addObject(GameObject gameObject, List<GameObject> list, int max)
	{
		//The object isn't added if it intersects the player.
		if(!intersectsPlayer(gameObject))
		{
			//Adds the object to the list.
			list.Add(gameObject);

			//Deletes the oldest object in the list if the list contains too many objects.
			if(list.Count > max)
			{
				GameObject oldGameObject = list[0];
				
				list.Remove(oldGameObject);
				Destroy(oldGameObject);
			}
		}
	}

	//Determines whether a drawn object intersects the player.
	private bool intersectsPlayer(GameObject gameObject)
	{
		//Destroys the object if it intersects the player.
		if(gameObject.collider.bounds.Intersects(player.collider.bounds))
		{
			Destroy(gameObject);
			return true;
		}

		return false;
	}

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
