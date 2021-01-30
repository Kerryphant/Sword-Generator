using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BladeGen : MonoBehaviour
{
	public GameObject CuboidPrefab;
	public GameObject ConePrefab;

	List<GameObject> parts;

	HiltGen hiltScript;

	// Start is called before the first frame update
	void Start()
    {
		hiltScript = GetComponent<HiltGen>();

		parts = new List<GameObject>();

		parts.Add((GameObject)Instantiate(CuboidPrefab));
		parts.Add((GameObject)Instantiate(ConePrefab));

		for (int i = 0; i < parts.Count; i++)
		{
			parts[i].transform.parent = this.transform;
		}

		//blade
		ProceduralCuboid tempCuboid = parts[0].GetComponent<ProceduralCuboid>();
		parts[0].transform.Rotate(-90.0f, 45.0f, 0.0f);
		parts[0].transform.position = new Vector3(0, hiltScript.hiltHeight, (tempCuboid.height * 0.5f));

		tempCuboid.width = 2.828f;
		tempCuboid.height = 2.828f;
		tempCuboid.depth = 27;

		//tip
		ProceduralCone tempCone = parts[1].GetComponent<ProceduralCone>();
	}

    // Update is called once per frame
    void Update()
    {
		UpdateBlade();
	}

	void UpdateBlade()
	{
		ProceduralCuboid tempCuboid = parts[0].GetComponent<ProceduralCuboid>();
		float diagonalLength = Mathf.Sqrt((tempCuboid.width * tempCuboid.width) + (tempCuboid.height * tempCuboid.height));
		parts[0].transform.position = new Vector3(0, hiltScript.hiltHeight, (diagonalLength * 0.5f));

		if (tempCuboid.width != tempCuboid.height)
		{
			if (tempCuboid.width < tempCuboid.height)
			{
				tempCuboid.width = tempCuboid.height;
			}
			else
			{
				tempCuboid.height = tempCuboid.width;
			}
		}

		if ((hiltScript.guardDepth > 0) && (diagonalLength > hiltScript.guardDepth))
		{
			float shortSides = Mathf.Sqrt((hiltScript.guardDepth * hiltScript.guardDepth) * 0.5f);

			tempCuboid.width = shortSides;
			tempCuboid.height = shortSides;
		}

		//tip
		ProceduralCone tempCone = parts[1].GetComponent<ProceduralCone>();
		parts[1].transform.position = new Vector3(0, hiltScript.hiltHeight + tempCuboid.depth, 0);
		tempCone.radius = diagonalLength * 0.5f;
	}
}