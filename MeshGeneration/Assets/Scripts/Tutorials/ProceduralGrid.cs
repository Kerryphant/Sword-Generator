using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralGrid : MonoBehaviour
{
	Mesh mesh;
	Vector3[] vertices;
	int[] triangles;

	//grid settings
	public float cellSize = 1;
	//defines position of entire grid
	public Vector3 gridOffset;
	public int gridSize;

    // Start is called before the first frame update
    void Awake()
    {
		mesh = GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Start()
    {
		MakeContiguousProceduralGrid();
		UpdateMesh();
    }

	private void UpdateMesh()
	{
		mesh.Clear();

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}

	private void MakeDiscreteProceduralGrid()
	{
		//set array size
		vertices = new Vector3[gridSize*gridSize*4];
		triangles = new int[gridSize * gridSize * 6];

		//tracking variables
		int verCount = 0;
		int t = 0;

		//set vertex offset defines shape of given cell
		float vertexOffset = cellSize * 0.5f;//multiply cause less computationally expensive.

		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				//position of single cell
				Vector3 cellOffset = new Vector3(x * cellSize, 0, y * cellSize);

				//populate vertices and triangles arrays
				vertices[verCount] = new Vector3(-vertexOffset, 0, -vertexOffset) + gridOffset + cellOffset;
				vertices[verCount + 1] = new Vector3(-vertexOffset, 0, vertexOffset) + gridOffset + cellOffset;
				vertices[verCount + 2] = new Vector3(vertexOffset, 0, -vertexOffset) + gridOffset + cellOffset;
				vertices[verCount + 3] = new Vector3(vertexOffset, 0, vertexOffset) + gridOffset + cellOffset;

				triangles[t] = verCount;
				triangles[t + 1] = verCount + 1;
				triangles[t + 2] = verCount + 2;
				triangles[t + 3] = verCount + 2;
				triangles[t + 4] = verCount + 1;
				triangles[t + 5] = verCount + 3;

				verCount += 4;
				t += 6;
			}
		}

	}

	private void MakeContiguousProceduralGrid()
	{
		//set array size
		vertices = new Vector3[(gridSize + 1) * (gridSize + 1)];
		triangles = new int[gridSize * gridSize * 6];

		//tracking variables
		int verCount = 0;
		int t = 0;

		//set vertex offset defines shape of given cell
		float vertexOffset = cellSize * 0.5f;//multiply cause less computationally expensive.

		//create vertex grid
		for (int x = 0; x <= gridSize; x++)
		{
			for (int y = 0; y <= gridSize; y++)
			{
				vertices[verCount] = new Vector3((x * cellSize) - vertexOffset, 0, (y * cellSize) - vertexOffset);
				verCount++;
			}
		}

		//reset vertex tracker
		verCount = 0;

		//set each cell's triangles
		for (int x = 0; x < gridSize; x++)
		{
			for (int y = 0; y < gridSize; y++)
			{
				triangles[t] = verCount;
				triangles[t + 1] = verCount + 1;
				triangles[t + 2] = verCount + (gridSize + 1);
				triangles[t + 3] = verCount + (gridSize + 1);
				triangles[t + 4] = verCount + 1;
				triangles[t + 5] = verCount + (gridSize + 1) + 1;

				verCount++;
				t += 6;
			}

			verCount++;

		}

	}
}
