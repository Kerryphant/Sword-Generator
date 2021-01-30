using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCube : MonoBehaviour
{

	Mesh mesh;
	List<Vector3> vertices;
	List<int> triangles;

	public float scale = 1f;
	float adjustedScale;

	public int posX, posY, posZ;

	private float prevScale;
	private int prevX, prevY, prevZ;

	void Awake()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		adjustedScale = scale * 0.5f;
	}

	// Start is called before the first frame update
	void Start()
	{
		MakeCube(adjustedScale, new Vector3((float)posX * scale, (float)posY * scale, (float)posZ * scale));
		UpdateMesh();

		prevScale = scale;
		prevX = posX;
		prevY = posY;
		prevZ = posZ;

	}

	private void Update()
	{
		//if a value was changed
		if ((scale != prevScale) || (posX != prevX) || (posY != prevY) || (posZ != prevZ))
		{
			//update adjusted scale
			adjustedScale = scale * 0.5f;

			//recreate mesh
			MakeCube(adjustedScale, new Vector3((float)posX * scale, (float)posY * scale, (float)posZ * scale));
			UpdateMesh();

			//update changes to prev variables
			prevScale = scale;
			prevX = posX;
			prevY = posY;
			prevZ = posZ;
		}
	}

	void MakeCube(float cubeScale, Vector3 cubePos)
	{
		vertices = new List<Vector3>();
		triangles = new List<int>();

		for (int i = 0; i < 6; i++)
		{
			MakeFace(i, cubeScale, cubePos);
		}
	}

	void MakeFace(int dir, float faceScale, Vector3 facePos)
	{
		vertices.AddRange(CubeMeshData.faceVertices(dir, faceScale, facePos));

		int vertCount = vertices.Count;

		triangles.Add(vertCount - 4);
		triangles.Add(vertCount - 4 + 1);
		triangles.Add(vertCount - 4 + 2);

		triangles.Add(vertCount - 4);
		triangles.Add(vertCount - 4 + 2);
		triangles.Add(vertCount - 4 + 3);
	}

	private void UpdateMesh()
	{
		mesh.Clear();

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}
}
