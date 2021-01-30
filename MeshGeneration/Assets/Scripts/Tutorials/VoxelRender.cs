using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelRender : MonoBehaviour
{
	Mesh mesh;
	List<Vector3> vertices;
	List<int> triangles;

	public float scale = 1f;
	float adjustedScale;

	// Start is called before the first frame update
	void Awake()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		adjustedScale = scale * 0.5f;
	}

	// Update is called once per frame
	void Start()
	{
		GenerateVoxelMesh(new VoxelData());
		UpdateMesh();
	}

	void GenerateVoxelMesh(VoxelData data)
	{
		vertices = new List<Vector3>();
		triangles = new List<int>();

		for (int z = 0; z < data.Depth; z++)
		{
			for (int x = 0; x < data.Width; x++)
			{
				if(data.GetCell(x, z) == 0)
				{
					continue;
				}

				MakeCube(adjustedScale, new Vector3((float)x * scale, 0, (float)z * scale), x, z, data);

			}
		}
	}

	void MakeCube(float cubeScale, Vector3 cubePos, int x, int z, VoxelData data)
	{
		for (int i = 0; i < 6; i++)
		{
			if(data.GetNeighbour(x, 0, z, (Direction)i) == 0)
			{
				MakeFace((Direction)i, cubeScale, cubePos);
			}		
		}
	}

	void MakeFace(Direction dir, float faceScale, Vector3 facePos)
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
