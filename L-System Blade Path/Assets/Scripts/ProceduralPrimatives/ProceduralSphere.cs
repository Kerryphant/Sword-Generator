using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralSphere : MonoBehaviour
{
	Mesh mesh;
	List<Vector3> vertices;
	List<Vector3> normals;
	List<int> triangles;

	int[][] grid;

	public int segCount = 20;
	public float radius = 1;

	//Track changes so mesh can be live updated
	private float prevSegCount;
	private float prevRadius;

	//Angle of Latitude
	private double theta = 0;
	//Angle of longitude
	private double delta = 0;

	private double x = 0;
	private double y = 0;
	private double z = 0;

	//Incremental values
	private double thetaInc;
	private double deltaInc;

	private double texInc;
	private double texU = 0;
	private double texV = 0;

	void Awake()
	{
		mesh = GetComponent<MeshFilter>().mesh;
	}

	// Start is called before the first frame update
	void Start()
	{
		prevSegCount = segCount;
		prevRadius = radius;

		thetaInc = -((2 * System.Math.PI) / segCount);
		deltaInc = System.Math.PI / segCount;
		texInc = 1.0 / segCount;	

		MakeSphere();
		UpdateMesh();
	}

	private void Update()
	{
		if (segCount > 255)
		{
			segCount = 255;
		}

		if (radius < 1)
		{
			radius = 1;
		}

		//if a value was changed
		if ((segCount != prevSegCount) || (radius != prevRadius))
		{
			//reset values
			thetaInc = -((2 * System.Math.PI) / segCount);
			deltaInc = System.Math.PI / segCount;
			texInc = 1.0 / segCount;

			theta = 0;
			delta = 0;

			//recreate mesh
			MakeSphere();
			UpdateMesh();

			//update changes to prev variables
			prevSegCount = segCount;
			prevRadius = radius;
		}
	}

	public void MakeSphere()
	{
		vertices = new List<Vector3>();
		normals = new List<Vector3>();
		triangles = new List<int>();
		grid = new int[segCount + 2][];
		for (int i = 0; i < grid.Length; i++)
		{
			grid[i] = new int[segCount+1];
		}

		int gridCount = 0;
		
		//Calculate vertex coords
		//longitude
		for (int i = 0; i < segCount + 1; i+=2)
		{
			//latitude
			for (int j = 0; j < segCount + 1; j++)
			{
				//lats, longs
				x = radius * System.Math.Cos(theta) * System.Math.Sin(delta);
				y = radius * System.Math.Cos(delta);
				z = radius * System.Math.Sin(theta) * System.Math.Sin(delta);
				
				vertices.Add(new Vector3((float)x, (float)y, (float)z));
				normals.Add(new Vector3((float)(x / radius), (float)(y / radius), (float)(z / radius)));

				grid[i][j] = gridCount++;

				//texCoords.push_back(texU);
				//texCoords.push_back(texV);


				//lats, longs+1
				x = radius * System.Math.Cos(theta) * System.Math.Sin(delta + deltaInc);
				y = radius * System.Math.Cos(delta + deltaInc);
				z = radius * System.Math.Sin(theta) * System.Math.Sin(delta + deltaInc);

				vertices.Add(new Vector3((float)x, (float)y, (float)z));
				normals.Add(new Vector3((float)(x / radius), (float)(y / radius), (float)(z / radius)));

				grid[i+1][j] = gridCount++;

				//texCoords.push_back(texU);
				//texCoords.push_back(texV + texInc);

				theta += thetaInc;
				texU += texInc;
			}

			texU = 0;
			texV += texInc;
			delta += 2* deltaInc;
		}

		//Assign triangles
		for (int i = 0; i < grid.Length-2; i++)
		{
			for (int j = 0; j < grid[i].Length; j++)
			{
				if(j == (grid[i].Length - 1))
				{
					triangles.Add(grid[i][j]);
					triangles.Add(grid[i + 1][j]);
					triangles.Add(grid[i][0]);

					triangles.Add(grid[i][0]);
					triangles.Add(grid[i + 1][j]);
					triangles.Add(grid[i + 1][0]);
					continue;
				}

				triangles.Add(grid[i][j]);
				triangles.Add(grid[i+1][j]);
				triangles.Add(grid[i][j + 1]);
								   
				triangles.Add(grid[i][j + 1]);
				triangles.Add(grid[i+1][j]);
				triangles.Add(grid[i + 1][j + 1]);
			}
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
