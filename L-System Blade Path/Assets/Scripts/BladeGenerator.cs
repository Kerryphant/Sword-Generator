using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeGenerator : MonoBehaviour
{
	public bool nodesFound = false;
	bool generated = false;
	List<Vector3> points = new List<Vector3>();

	// Start is called before the first frame update
	void Start()
    {
        
    }

	// Update is called once per frame
	void Update()
	{
		if (!nodesFound)
		{
			generated = false;

			points.Clear();
			var spheres = GameObject.FindGameObjectsWithTag("point");

			foreach (var item in spheres)
			{
				points.Add(item.transform.position);
			}

			if (points.Count > 0)
			{
				nodesFound = true;
			}
		}
		
		if(nodesFound && !generated)
		{
			ProceduralBlade mesh = transform.GetComponent<ProceduralBlade>();
			mesh.valuesPassed = true;
			//mesh.GeneratePathedCuboid(points.Count, points);
			mesh.GeneratePathedBlade(points.Count, points);

			generated = true;
		}

	}
}
