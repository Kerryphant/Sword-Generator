using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCone : MonoBehaviour
{
	Mesh mesh;
	List<Vector3> vertices;
	List<Vector3> normals;
	List<int> triangles;

	public float radius = 2;
	public int segCount = 4;
	public float height = 4;

	private float prevSegCount;
	private float prevRadius;
	private float prevHeight = 4;

	double x = 0;
	double y = 0;
	double angle = 0;

	double interval = 0;

	void Awake()
	{
		mesh = GetComponent<MeshFilter>().mesh;
	}

	// Start is called before the first frame update
	void Start()
    {
		interval = (2 * System.Math.PI) / segCount;
		prevSegCount = segCount;
		prevRadius = radius;
		prevHeight = height;

		MakeCone();
		UpdateMesh();
	}

    // Update is called once per frame
    void Update()
    {
		if ((segCount != prevSegCount) || (radius != prevRadius) || (height != prevHeight))
		{
			interval = (2 * System.Math.PI) / segCount;

			MakeCone();
			UpdateMesh();

			prevSegCount = segCount;
			prevRadius = radius;
			prevHeight = height;
		}
	}

	private void MakeCone()
	{
		vertices = new List<Vector3>();
		normals = new List<Vector3>();
		triangles = new List<int>();

		for (int i = 0; i < segCount; i++)
		{
			x = radius * System.Math.Cos(angle);
			y = radius * System.Math.Sin(angle);

			//BASE
			vertices.Add(new Vector3(0, 0, 0));
			vertices.Add(new Vector3((float)x, 0, (float)y));
			vertices.Add(new Vector3((float)(radius * System.Math.Cos(angle + interval)), 0, (float)(radius * System.Math.Sin(angle + interval))));

			//SIDES
			vertices.Add(new Vector3((float)x, 0, (float)y));

			//TOP OF SHAPE
			vertices.Add(new Vector3(0, height , 0));

			vertices.Add(new Vector3((float)(radius * System.Math.Cos(angle + interval)), 0, (float)(radius * System.Math.Sin(angle + interval))));

			x = radius * System.Math.Cos(angle + interval);
			y = radius * System.Math.Sin(angle + interval);

			angle += interval;
		}

		for (int i = 0; i < vertices.Count; i++)
		{
			triangles.Add(i);
		}

	}

	private void UpdateMesh()
	{
		mesh.Clear();

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}
}
