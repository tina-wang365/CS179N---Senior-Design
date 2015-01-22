using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawReader : MonoBehaviour
{
	private LineRenderer line;
	private bool isMousePressed;
	private List<Vector3> pointsList;
	private Vector3 mousePos;
	private Vector3 minPos;
	private Vector3 maxPos;
	private GameObject platform;
	//public float maxDistance;
	//private float distance;

	struct myLine
	{
		public Vector3 StartPoint;
		public Vector3 EndPoint;
	};
	//    -----------------------------------    
	void Awake()
	{
		line = gameObject.AddComponent<LineRenderer>();
		//line.material = new Material(Shader.Find("Particles/Additive"));
		line.SetVertexCount(0);
		line.SetWidth(0.1f,0.1f);
		//line.SetColors(Color.green, Color.green);
		line.useWorldSpace = true;    
		isMousePressed = false;
		pointsList = new List<Vector3>();
		platform = null;
	}

	void Update() 
	{
		if(Input.GetMouseButtonDown(0))
		{
			minPos = new Vector3(float.PositiveInfinity, 0f, 0f);
			maxPos = new Vector3(float.NegativeInfinity, 0f, 0f);
			isMousePressed = true;
			line.SetColors(Color.green, Color.green);
			//distance = 0;
		}
		else if(Input.GetMouseButtonUp(0))
		{
			if(platform != null)
			{
				Destroy(platform);
			}

			isMousePressed = false;
			platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
			platform.name = "Platform";
			platform.transform.position = new Vector3((maxPos.x + minPos.x) / 2f, (maxPos.y + minPos.y) / 2f, 0f);
			platform.transform.localScale = new Vector3(maxPos.x - minPos.x, 1f, 1f);

			line.SetVertexCount(0);
			pointsList.RemoveRange(0,pointsList.Count);
		}

		if(isMousePressed)
		{
			mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.z = 0f;

			if(!pointsList.Contains (mousePos)) 
			{
				//Vector3 last = (Vector3)pointsList [pointsList.Count - 1];
				//distance += Mathf.Sqrt(Mathf.Pow(mousePos.x - last.x, 2f) + Mathf.Pow(mousePos.y - last.y, 2f));

				pointsList.Add (mousePos);
				line.SetVertexCount (pointsList.Count);
				line.SetPosition (pointsList.Count - 1, (Vector3)pointsList [pointsList.Count - 1]);

				minPos = mousePos.x < minPos.x ? mousePos : minPos;
				maxPos = mousePos.x > maxPos.x ? mousePos : maxPos;
			}
		}
	}
}
