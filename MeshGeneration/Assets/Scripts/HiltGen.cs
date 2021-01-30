using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HiltGen : MonoBehaviour
{
	public GameObject SpherePrefab;
	public GameObject CuboidPrefab;

	List<GameObject> parts;

	float gripHeight;
	[HideInInspector]
	public float guardDepth;
	[HideInInspector]
	public float hiltHeight;

	// Start is called before the first frame update
	void Start()
    {
		parts = new List<GameObject>();

		parts.Add((GameObject)Instantiate(SpherePrefab));
		parts.Add((GameObject)Instantiate(CuboidPrefab));
		parts.Add((GameObject)Instantiate(CuboidPrefab));

		for (int i = 0; i < parts.Count; i++)
		{
			parts[i].transform.parent = this.transform;
		}

		//pommel sphere
		ProceduralSphere tempSphere = parts[0].GetComponent<ProceduralSphere>();
		tempSphere.radius = 2;

		//grip cuboid
		ProceduralCuboid tempCuboid = parts[1].GetComponent<ProceduralCuboid>();
		parts[1].transform.Rotate(90.0f, 0.0f, 0.0f);

		tempCuboid.width = 1;
		tempCuboid.height = 2;
		tempCuboid.depth = 10;

		//guard cuboid
		tempCuboid = parts[2].GetComponent<ProceduralCuboid>();
		tempCuboid.width = 4;
		tempCuboid.height = 1;
		tempCuboid.depth = 7;

	}

	// Update is called once per frame
	void Update()
    {
		//TODO make these relative to sphere

		//grip
		ProceduralCuboid tempCuboid = parts[1].GetComponent<ProceduralCuboid>();
		gripHeight = tempCuboid.depth;
		parts[1].transform.position = new Vector3(-(tempCuboid.width * 0.5f), gripHeight, -(tempCuboid.height * 0.5f));

		//guard
		tempCuboid = parts[2].GetComponent<ProceduralCuboid>();
		parts[2].transform.position = new Vector3(-(tempCuboid.width * 0.5f), gripHeight, -(tempCuboid.depth * 0.5f));

		guardDepth = tempCuboid.width;

		//sphere
		ProceduralSphere tempSphere = parts[0].GetComponent<ProceduralSphere>();

		hiltHeight = gripHeight + tempCuboid.height;
	}
}
