using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawReader : MonoBehaviour
{
	private LineRenderer drawing;
	private bool isMousePressed;
	private List<Vector3> points;
	private List<GameObject> platforms;
	private List<GameObject> trampolines;
	private bool[,] platformTemplate;
	private bool[,] trampolineTemplate;
	private float platformScoreThreshold = 5f;
	private float trampolineScoreThreshold = 6f;
	private float minX;
	private float maxX;
	private float minY;
	private float maxY;
	private float distance;
	private int gridSize = 5;
	private int halfGridSize;
	public float maxDistance;
	public int maxPlatforms;
	public int maxTrampolines;
	public GameObject player;

	void Awake()
	{
		halfGridSize = gridSize / 2;
		points = new List<Vector3>();
		platforms = new List<GameObject>();
		trampolines = new List<GameObject>();
		isMousePressed = false;
		drawing = gameObject.AddComponent<LineRenderer>();
		//line.material = new Material(Shader.Find("Particles/Additive"));
		drawing.useWorldSpace = true;
		
		setUpPlatformTemplate();
		setUpTrampolineTemplate();
		drawing.SetVertexCount(0);
		drawing.SetWidth(0.1f, 0.1f);
		//line.SetColors(Color.green, Color.green);
	}
	
	void Update() 
	{
		if(Input.GetMouseButtonDown(0))
		{
			//Set initial min/max drawing point values.
			minX = float.PositiveInfinity;
			maxX = float.NegativeInfinity;
			minY = float.PositiveInfinity;
			maxY = float.NegativeInfinity;
			isMousePressed = true;
			distance = 0f;
			
			//line.SetColors(Color.green, Color.green);
		}
		else if(Input.GetMouseButtonUp(0))
		{

			//Create an object if it was drawn correctly and remove the drawing.
			isMousePressed = false;

			interpretDrawing();
			drawing.SetVertexCount(0);
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
				if(removeClicked(hit, platforms) || removeClicked(hit, trampolines))
				{
					//Display error message?
				}
			}
		}

		//Add points to the drawing while the left mouse button is down.
		if(isMousePressed && distance < maxDistance)
		{
			//Get a point from the current mouse position. The camera should be at position (0, 1, -10) and rotation (0, 0, 0).
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			mousePos.z = 0f;
			
			if(!points.Contains(mousePos)) 
			{
				//Calculate the distance drawn.
				if(points.Count > 0)
				{
					Vector3 lastPoint = points[points.Count - 1];
					
					distance += Mathf.Sqrt(Mathf.Pow(mousePos.x - lastPoint.x, 2f) + Mathf.Pow(mousePos.y - lastPoint.y, 2f));
				}

				//Add point to the drawing.
				points.Add(mousePos);
				drawing.SetVertexCount(points.Count);
				drawing.SetPosition(points.Count - 1, points[points.Count - 1]);

				//Recalculate minimum and maximum point values.

				minX = mousePos.x < minX ? mousePos.x : minX;
				maxX = mousePos.x > maxX ? mousePos.x : maxX;
				minY = mousePos.y < minY ? mousePos.y : minY;
				maxY = mousePos.y > maxY ? mousePos.y : maxY;
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

	//Compares a drawing to object templates and creates an object if a match is found.

	private void interpretDrawing()
	{
		float xLength = maxX - minX;
		float yLength = maxY - minY;
		bool horizontal = xLength > yLength;
		float distanceFromMidpoint = (horizontal ? xLength : yLength) / 2f;
		Vector2 midpoint = new Vector2((maxX + minX) / 2f, (maxY + minY) / 2f);

		bool[,] grid = setUpGrid();

		//Converts the drawing to grid format for comparison.
		foreach(Vector3 point in points)
		{
			int x = halfGridSize + (int) (((float) halfGridSize) * (point.x - midpoint.x) / distanceFromMidpoint);
			int y = halfGridSize + (int) (((float) halfGridSize) * (point.y - midpoint.y) / distanceFromMidpoint);

			if(x < 0 || y < 0 || x > gridSize - 1 || y > gridSize - 1)
			{
				return;
			}

			grid[x, y] = true;
		}

		//Calculates matching scores for each template.

		float[] scores = {compareGrids(grid, platformTemplate) / platformScoreThreshold,
			compareGrids(grid, trampolineTemplate) / trampolineScoreThreshold};
		int max = -1;
		float maxScore = 0.8f;

		//Finds the object template with the highest score (initially set high to avoid false positives).

		for(int i = 0; i < scores.Length; i++)
		{
			if(scores[i] >= maxScore)
			{
				maxScore = scores[i];
				max = i;
			}
		}
		
		print("Platform: " + scores[0] + " Trampoline: " + scores[1]);

		//Create the object with the highest score.

		switch(max)
		{
		case 0:
			createPlatform(midpoint);
			break;
		case 1:
			createTrampoline(midpoint);
			break;
		default:
			break;
		}
	}

	//Compares two grids and returns a score based on their similarity.
	private int compareGrids(bool[,] a, bool[,] b)
	{
		int score = 0;

		for(int i = 0; i < gridSize; i++)

		{
			for(int j = 0; j < gridSize; j++)
			{
				score += a[i, j] && b[i, j] ? 1 : 0;
			}
		}
		
		return score;
	}

	//Creates a platform at the specified position.
	private void createPlatform(Vector2 position)
	{
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);

		//Sets platform properties.

		platform.name = "Platform";
		platform.transform.position = new Vector3(position.x, position.y, 0f);
		platform.transform.localScale = new Vector3(maxX - minX, 1f, 1f);

		addObject(platform, platforms, maxPlatforms);
	}

	//Creates a trampoline at the speficied position.
	private void createTrampoline(Vector2 position)
	{
		GameObject trampoline = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

		//Sets trampoline properties.
		trampoline.name = "Trampoline";
		trampoline.transform.position = new Vector3(position.x, position.y, 0f);
		trampoline.transform.localScale = new Vector3(maxX - minX, 0.1f, maxX - minX);

		addObject(trampoline, trampolines, maxTrampolines);
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

	//Sets up the grid template for a platform.
	private void setUpPlatformTemplate()
	{
		platformTemplate = setUpGrid();

		//Creates a horizontal line in the grid.
		for(int i = 0; i < gridSize; i++)
		{
			platformTemplate[i, halfGridSize] = true;
		}
	}

	//Sets up the grid template for a trampoline.
	private void setUpTrampolineTemplate()
	{
		trampolineTemplate = setUpGrid();

		float intervals = (float) (gridSize * gridSize);

		//Creates a circle in the grid.
		for(float i = 0f; i < intervals; i++)
		{
			trampolineTemplate[halfGridSize + (int) (((float) halfGridSize) * Mathf.Cos(i * 2f * Mathf.PI / intervals)),
				halfGridSize + (int) (((float) halfGridSize) * Mathf.Sin(i * 2f * Mathf.PI / intervals))] = true;
		}
	}

	//Creates a "blank" grid template.
	private bool[,] setUpGrid()
	{
		bool[,] grid = new bool[gridSize, gridSize];

		for(int i = 0; i < gridSize; i++)
		{
			for(int j = 0; j < gridSize; j++)
			{
				grid[i, j] = false;
			}
		}
		
		return grid;
	}
}
