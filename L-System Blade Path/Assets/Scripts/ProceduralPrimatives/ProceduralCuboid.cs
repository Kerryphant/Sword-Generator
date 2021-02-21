using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCuboid : MonoBehaviour
{
	Mesh mesh;
	List<Vector3> vertices;
	List<Vector3> normals;
	List<int> triangles;

	public float width = 1;
	public float height = 1;
	public float depth = 3;

	private float prevWidth = 0;
	private float prevHeight = 0;
	private float prevDepth = 0;

	// Start is called before the first frame update
	void Awake()
    {
		mesh = GetComponent<MeshFilter>().mesh;
	}

	private void Start()
	{
		MakeCuboid();
		UpdateMesh();
	}

	// Update is called once per frame
	void Update()
	{
		if ((width != prevWidth) || (height != prevHeight) || (depth != prevDepth))
		{
			prevWidth = width;
			prevHeight = height;
			prevDepth = depth;
		}

		MakeCuboid();
		UpdateMesh();
	}

	public void MakeCuboid()
	{
		vertices = new List<Vector3>
		{
			new Vector3(0, 0, 0),
			new Vector3(0, height, 0),
			new Vector3(width, 0, 0),
			new Vector3(width, height, 0),

			new Vector3(width, 0, depth),
			new Vector3(width, height, depth),
			new Vector3(0, 0, depth),
			new Vector3(0, height, depth),
		};

		triangles = new List<int>
		{
			//front
			0, 1, 2, 2, 1, 3,
			//right
			2, 3, 4, 4, 3, 5,
			//back
			4, 5, 6, 6, 5, 7,
			//left
			6, 7, 0, 0, 7, 1,
			//top
			1, 7, 3, 3, 7, 5,
			//bottom
			6, 0, 4, 4, 0, 2
		};

	}

	private void UpdateMesh()
	{
		mesh.Clear();

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}
}