using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCurvedCuboid : MonoBehaviour
{
	Mesh mesh;
	List<Vector3> vertices;
	List<Vector3> normals;
	List<int> triangles;

	public float width = 1;
	public float height = 1;
	public float depth = 3;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void GenerateVertices()
	{

		int cuboidCount = 3;

		vertices = new List<Vector3>();
		float offset = 0;
		for (int i = 0; i < 4; i++)
		{
			if (i == 0)
			{
				//bottom left
				vertices.Add(new Vector3(0, 0, 0));
				//top left
				vertices.Add(new Vector3(0, height, 0));
				//bottom right
				vertices.Add(new Vector3(width, 0, 0));
				//top right
				vertices.Add(new Vector3(width, height, 0));
			}
			else
			{
				//bottom left
				vertices.Add(new Vector3(0, 0, depth + offset));
				//top left
				vertices.Add(new Vector3(0, height, depth + offset));
				//bottom right
				vertices.Add(new Vector3(width, 0, depth + offset));
				//top right
				vertices.Add(new Vector3(width, height, depth + offset));
				offset += depth;
			}

		}

		int startVert = 0;
		triangles = new List<int>();
		for (int i = 0; i < cuboidCount; i++)
		{
			//front 0, 1, 2, 2, 1, 3,
			triangles.Add(startVert + 0);
			triangles.Add(startVert + 1);
			triangles.Add(startVert + 2);
			triangles.Add(startVert + 2);
			triangles.Add(startVert + 1);
			triangles.Add(startVert + 3);


			//right 2, 3, 4, 4, 3, 5,
			triangles.Add(startVert + 2);
			triangles.Add(startVert + 3);
			triangles.Add(startVert + 6);
			triangles.Add(startVert + 6);
			triangles.Add(startVert + 3);
			triangles.Add(startVert + 7);

			//back 4, 5, 6, 6, 5, 7,
			triangles.Add(startVert + 6);
			triangles.Add(startVert + 7);
			triangles.Add(startVert + 4);
			triangles.Add(startVert + 4);
			triangles.Add(startVert + 7);
			triangles.Add(startVert + 5);


			//left 6, 7, 0, 0, 7, 1,
			triangles.Add(startVert + 4);
			triangles.Add(startVert + 5);
			triangles.Add(startVert + 0);
			triangles.Add(startVert + 0);
			triangles.Add(startVert + 5);
			triangles.Add(startVert + 1);

			//top 1, 7, 3, 3, 7, 5
			triangles.Add(startVert + 1);
			triangles.Add(startVert + 5);
			triangles.Add(startVert + 3);
			triangles.Add(startVert + 3);
			triangles.Add(startVert + 5);
			triangles.Add(startVert + 7);

			//bottom 6, 0, 4, 4, 0, 2
			triangles.Add(startVert + 4);
			triangles.Add(startVert + 0);
			triangles.Add(startVert + 6);
			triangles.Add(startVert + 6);
			triangles.Add(startVert + 0);
			triangles.Add(startVert + 2);

			startVert += 4;
		}
	}
}
