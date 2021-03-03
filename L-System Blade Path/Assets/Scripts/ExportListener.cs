using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ExportListener : MonoBehaviour
{
	public Button button;
	int exportCount = 0;

	// Start is called before the first frame update
	void Start()
    {
		button.onClick.AddListener(Export);
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			Export();	
		}
	}

	void Export()
	{
		exportCount++;

		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];

		int i = 0;
		while (i < meshFilters.Length)
		{
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

			i++;
		}

		Mesh outputMesh = new Mesh();
		outputMesh.CombineMeshes(combine);

		List<Vector3> vert = new List<Vector3>(outputMesh.vertices);
		List<Vector3> norm = new List<Vector3>(outputMesh.normals);
		List<int> tri = new List<int>(outputMesh.triangles);

		Exporter.ExportMesh("mesh" + exportCount, vert, norm, tri);
	}
}
