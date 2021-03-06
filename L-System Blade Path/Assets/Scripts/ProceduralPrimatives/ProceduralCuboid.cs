﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCuboid : MonoBehaviour
{
	Mesh mesh;
	List<Vector3> vertices;
	List<Vector3> normals;
	List<int> triangles;
	List<int> quads;

	List<Edge> edges;
	List<FacePoint> faces;

	public float width = 1;
	public float height = 1;
	public float depth = 3;

	private float prevWidth = 0;
	private float prevHeight = 0;
	private float prevDepth = 0;

	public bool smooth = false;

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

	public void SmoothMesh()
	{
		CatmullClarkSubdivide.SubdivideMesh(ref vertices, ref triangles, ref quads, ref edges, ref faces);
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

		//MakeCuboid();
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

		//6 indices for each triangle face, 4 indices for quad faces
		int faceCount = (triangles.Count / 6);

		quads = new List<int>();
		for (int j = 0; j < triangles.Count; j += 6)
		{
			quads.Add(triangles[j]);
			quads.Add(triangles[j + 1]);
			quads.Add(triangles[j + 5]);
			quads.Add(triangles[j + 2]);
		}

		faces = new List<FacePoint>()
		{
			new FacePoint(new List<Vector3>//front
			{vertices[0], vertices[1], vertices[3], vertices[2]}),

			new FacePoint(new List<Vector3>//right
			{vertices[2], vertices[3], vertices[5], vertices[4]}),

			new FacePoint(new List<Vector3>//back
			{vertices[4], vertices[5], vertices[7], vertices[6]}),

			new FacePoint(new List<Vector3>//left
			{vertices[6], vertices[7], vertices[1], vertices[0]}),

			new FacePoint(new List<Vector3>//top
			{vertices[1], vertices[7], vertices[5], vertices[3]}),

			new FacePoint(new List<Vector3>//bottom
			{vertices[6], vertices[0], vertices[2], vertices[4]})
		};

		edges = new List<Edge>
		{
			new Edge(vertices[0], vertices[1]),
			new Edge(vertices[1], vertices[3]),
			new Edge(vertices[3], vertices[2]),
			new Edge(vertices[2], vertices[0]),

			new Edge(vertices[3], vertices[5]),
			new Edge(vertices[5], vertices[4]),
			new Edge(vertices[4], vertices[2]),

			new Edge(vertices[5], vertices[7]),
			new Edge(vertices[7], vertices[6]),
			new Edge(vertices[6], vertices[4]),

			new Edge(vertices[7], vertices[1]),
			new Edge(vertices[0], vertices[6]),

		};

		for (int i = 0; i < edges.Count; i++)
		{
			for (int j = 0; j < faces.Count; j++)
			{
				bool check1 = (faces[j].parentVertValue.Contains(edges[i].startVert));
				bool check2 = (faces[j].parentVertValue.Contains(edges[i].endVert));
				if (check1 && check2)
				{
					if (edges[i].face1 >= 0)
					{
						Edge newEdge = edges[i];

						newEdge.face2 = j;
						edges[i] = newEdge;
					}
					else
					{
						Edge newEdge = edges[i];
						newEdge.face1 = j;
						edges[i] = newEdge;
					}
				}
			}
		}
	}

	public void UpdateMesh()
	{
		mesh.Clear();

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}
}
