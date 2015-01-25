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
	private bool[][] platformTemplate;
	private bool[][] trampolineTemplate;
	private float platformScoreThreshold = 5f;
	private float trampolineScoreThreshold = 6f;
	private float minX;
	private float maxX;
	private float minY;
	private float maxY;
	private float distance;
	public float maxDistance;
	public int maxPlatforms;
	public int maxTrampolines;

	void Awake()
	{
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
			isMousePressed = false;

			interpretDrawing();
			drawing.SetVertexCount(0);
			points.RemoveRange(0, points.Count);
		}

		if(isMousePressed && distance < maxDistance)
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			mousePos.z = 0f;

			if(!points.Contains(mousePos)) 
			{
				if(points.Count > 0)
				{
					Vector3 lastPoint = points[points.Count - 1];

					distance += Mathf.Sqrt(Mathf.Pow(mousePos.x - lastPoint.x, 2f) + Mathf.Pow(mousePos.y - lastPoint.y, 2f));
				}

				points.Add(mousePos);
				drawing.SetVertexCount(points.Count);
				drawing.SetPosition(points.Count - 1, points[points.Count - 1]);

				minX = mousePos.x < minX ? mousePos.x : minX;
				maxX = mousePos.x > maxX ? mousePos.x : maxX;
				minY = mousePos.y < minY ? mousePos.y : minY;
				maxY = mousePos.y > maxY ? mousePos.y : maxY;
			}
		}
	}

	private void interpretDrawing()
	{
		float xLength = maxX - minX;
		float yLength = maxY - minY;
		bool horizontal = xLength > yLength;
		float distanceFromMidpoint = (horizontal ? xLength : yLength) / 2f;
		Vector2 midpoint = new Vector2((maxX + minX) / 2f, (maxY + minY) / 2f);
		bool[][] grid = setUpGrid();

		foreach(Vector3 point in points)
		{
			int x = 2 + (int) (2f * (point.x - midpoint.x) / distanceFromMidpoint);
			int y = 2 + (int) (2f * (point.y - midpoint.y) / distanceFromMidpoint);

			if(x < 0 || y < 0 || x > 4 || y > 4)
			{
				return;
			}

			grid[x][y] = true;
		}

		float[] scores = {compareGrids(grid, platformTemplate) / platformScoreThreshold,
						  compareGrids(grid, trampolineTemplate) / trampolineScoreThreshold};
		int max = -1;
		float maxScore = 0.8f;

		for(int i = 0; i < scores.Length; i++)
		{
			if(scores[i] >= maxScore)
			{
				maxScore = scores[i];
				max = i;
			}
		}

		print("Platform: " + scores[0] + " Trampoline: " + scores[1]);

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

	private int compareGrids(bool[][] a, bool[][] b)
	{
		int score = 0;

		for(int i = 0; i < 5; i++)
		{
			for(int j = 0; j < 5; j++)
			{
				score += a[i][j] && b[i][j] ? 1 : 0;
			}
		}

		return score;
	}

	private void createPlatform(Vector2 position)
	{
		GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);

		platform.name = "Platform";
		platform.transform.position = new Vector3(position.x, position.y, 0f);
		platform.transform.localScale = new Vector3(maxX - minX, 1f, 1f);
		
		platforms.Add(platform);
		
		if(platforms.Count > maxPlatforms)
		{
			GameObject oldPlatform = platforms[0];
			
			platforms.Remove(oldPlatform);
			Destroy(oldPlatform);
		}
	}

	private void createTrampoline(Vector2 position)
	{
		GameObject trampoline = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

		trampoline.name = "Trampoline";
		trampoline.transform.position = new Vector3(position.x, position.y, 0f);
		trampoline.transform.localScale = new Vector3(maxX - minX, 0.1f, 1f);

		trampolines.Add(trampoline);

		if(trampolines.Count > maxTrampolines)
		{
			GameObject oldTrampoline = trampolines[0];

			trampolines.Remove(oldTrampoline);
			Destroy(oldTrampoline);
		}
	}

	private void setUpPlatformTemplate()
	{
		platformTemplate = setUpGrid();

		for(int i = 0; i < 5; i++)
		{
			platformTemplate[i][2] = true;
		}
	}

	private void setUpTrampolineTemplate()
	{
		trampolineTemplate = setUpGrid();

		for(float i = 0f; i < 25f; i++)
		{
			trampolineTemplate[2 + (int) (2f * Mathf.Cos(i * 2f * Mathf.PI / 25f))][2 + (int) (2f * Mathf.Sin(i * 2f * Mathf.PI / 25f))] = true;
		}
	}

	private bool[][] setUpGrid()
	{
		bool[][] grid = {new bool[5], new bool[5], new bool[5], new bool[5], new bool[5]};

		foreach(bool[] row in grid)
		{
			for(int i = 0; i < row.Length; i++)
			{
				row[i] = false;
			}
		}

		return grid;
	}
}
