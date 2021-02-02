using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralBlade: MonoBehaviour
{
	Mesh mesh;
	List<Vector3> vertices;
	List<Vector3> normals;
	List<int> triangles;

	public float width = 1;
	public float height = 1;
	public float depth = 3;

	public bool valuesPassed;

	public Material mat;
	public GameObject placeHolder;

	// Start is called before the first frame update
	void Awake()
	{
		mesh = GetComponent<MeshFilter>().mesh;

		print(RotatePointAroundPivot(new Vector3(1, 0, 0), new Vector3(2, 0, 0), new Vector3(0, -90, 0)));
		print(SignedAngleBetween(new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0)));
	}

	private void Start()
	{
		GetComponent<Renderer>().material = mat;
		Instantiate(placeHolder, transform);
	}

	void Update()
	{
		if (valuesPassed)
		{
			UpdateMesh();
		}
	}

	public void GeneratePathedBlade(int numPoints_, List<Vector3> points_)
	{
		GenerateCuboidLine(numPoints_);
		PositionVerticesToPoints(numPoints_, points_);
		ShapeIntoDiamondBlade(numPoints_, points_);
	}

	void GenerateCuboidLine(int numPoints_)
	{
		int cuboidCount = numPoints_ - 1;

		vertices = new List<Vector3>();
		float offset = 0;
		for (int i = 0; i < numPoints_; i++)
		{
			if (i == 0)
			{
				//bottom left
				vertices.Add(new Vector3(0, 0, 0 + offset));
				//top left
				vertices.Add(new Vector3(0, height, 0 + offset));
				//bottom right
				vertices.Add(new Vector3(width, 0, 0 + offset));
				//top right
				vertices.Add(new Vector3(width, height, 0 + offset));
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

	void PositionVerticesToPoints(int numPoints_, List<Vector3> points_)
	{
		int startVert = 0;
		for (int i = 0; i < numPoints_; i++)
		{
			//4 vertices to be moved
			vertices[startVert] = new Vector3(points_[i].x - (0.5f * width), points_[i].y - (0.5f * height), points_[i].z);

			vertices[startVert + 1] = new Vector3(points_[i].x - (0.5f * width), points_[i].y + (0.5f * height), points_[i].z);

			vertices[startVert + 2] = new Vector3(points_[i].x + (0.5f * width), points_[i].y - (0.5f * height), points_[i].z);

			vertices[startVert + 3] = new Vector3(points_[i].x + (0.5f * width), points_[i].y + (0.5f * height), points_[i].z);

			startVert += 4;
		}


		startVert = 4;
		for (int i = 1; i < numPoints_ - 1; i++)
		{
			//get the angle between the positive x-axis and the line made from the current point to the next point
			float rotationA = SignedAngleBetween(Vector3.right, points_[i + 1] - points_[i], new Vector3(0, 1, 0));
			
			//get HALF of angle between the line made from the current & next point and the current & previous point
			float rotationB = SignedAngleBetween(points_[i + 1] - points_[i], points_[i - 1] - points_[i], new Vector3(0, 1, 0)) / 2;
			
			//get the rotation in degrees by adding the calculated angles
			float rotation = rotationA + rotationB;
			
			//Check if the angle must be flipped
			if (rotationB < 0)
			{
				rotation += 180;
			}

			//assign the position for each vertex in this face
			vertices[startVert] = RotatePointAroundPivot(vertices[startVert], points_[i], new Vector3(0, rotation, 0));
			vertices[startVert + 1] = RotatePointAroundPivot(vertices[startVert + 1], points_[i], new Vector3(0, rotation, 0));
			vertices[startVert + 2] = RotatePointAroundPivot(vertices[startVert + 2], points_[i], new Vector3(0, rotation, 0));
			vertices[startVert + 3] = RotatePointAroundPivot(vertices[startVert + 3], points_[i], new Vector3(0, rotation, 0));

			startVert += 4;
		}
		{//local scope for end point
			
			startVert = (numPoints_ - 1) * 4;

			Vector3 rotatedPointForPerpendicular = RotatePointAroundPivot(points_[numPoints_ - 2], points_[numPoints_ - 1], new Vector3(0, 90, 0));
			float rotationX = SignedAngleBetween(Vector3.right, points_[numPoints_ - 1] - rotatedPointForPerpendicular, new Vector3(0, 1, 0));
			
			vertices[startVert] = RotatePointAroundPivot(vertices[startVert], points_[numPoints_ - 1], new Vector3(0, rotationX, 0));
			vertices[startVert + 1] = RotatePointAroundPivot(vertices[startVert + 1], points_[numPoints_ - 1], new Vector3(0, rotationX, 0));
			vertices[startVert + 2] = RotatePointAroundPivot(vertices[startVert + 2], points_[numPoints_ - 1], new Vector3(0, rotationX, 0));
			vertices[startVert + 3] = RotatePointAroundPivot(vertices[startVert + 3], points_[numPoints_ - 1], new Vector3(0, rotationX, 0));
		}
	}

	Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		Vector3 dir = point - pivot; // get point direction relative to pivot
		dir = Quaternion.Euler(angles) * dir; // rotate it
		point = dir + pivot; // calculate rotated point
		return point; // return it
	}

	float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
	{
		// angle in [0,180]
		float angle = Vector3.Angle(a, b);
		float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

		// angle in [-179,180]
		float signed_angle = angle * sign;

		// angle in [0,360] (not used but included here for completeness)
		//float angle360 =  (signed_angle + 180) % 360;

		return signed_angle;
	}

	void ShapeIntoDiamondBlade(int numPoints_, List<Vector3> points_)
	{
		Vector3 axis;
		int startVert = 0;
		
		//rotate the vertices 45 degress using the line between the current point and next point as an axis
		for (int i = 0; i < numPoints_ - 1; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				placeHolder.transform.position = vertices[startVert + j];
				axis = points_[i + 1] - points_[i];

				placeHolder.transform.RotateAround(points_[i], axis, 45);
				vertices[startVert + j] = placeHolder.transform.position;
			
			}
			startVert += 4;
		}

		//rotate the end vertices
		startVert = vertices.Count - 4;
		axis = points_[points_.Count - 1] - points_[points_.Count - 2];
		for (int j = 0; j < 4; j++)
		{
			placeHolder.transform.position = vertices[startVert + j];

			placeHolder.transform.RotateAround(points_[points_.Count - 1], axis, 45);
			vertices[startVert + j] = placeHolder.transform.position;

		}
		
		//displace points to give blade shape
		startVert = 0;
		for (int i = 0; i < numPoints_; i++)
		{
			vertices[startVert] = new Vector3(vertices[startVert].x, vertices[startVert].y + 0.25f, vertices[startVert].z);
			vertices[startVert + 1] = new Vector3(vertices[startVert + 1].x - 0.25f, vertices[startVert + 1].y, vertices[startVert + 1].z);
			vertices[startVert + 2] = new Vector3(vertices[startVert + 2].x + 0.25f, vertices[startVert + 2].y, vertices[startVert + 2].z);
			vertices[startVert + 3] = new Vector3(vertices[startVert + 3].x, vertices[startVert + 3].y - 0.25f, vertices[startVert + 3].z);

			startVert += 4;
		}
		

		//Create the point of the blade
		Vector3 midpoint = new Vector3((vertices[vertices.Count - 4].x + vertices[vertices.Count - 1].x) / 2.0f, (vertices[vertices.Count - 4].y + vertices[vertices.Count - 1].y) / 2.0f, (vertices[vertices.Count - 4].z + vertices[vertices.Count - 1].z) / 2.0f);
		Vector3 direction = points_[points_.Count - 2] - points_[points_.Count - 1];
		direction.Normalize();
		vertices.Add(midpoint - (2*direction));

		//startVert = 5;
		//for (int i = 0; i < 4; i++)
		//{
		triangles.Add(vertices.Count - 1);
		triangles.Add(vertices.Count - 4);
		triangles.Add(vertices.Count - 5);

		triangles.Add(vertices.Count - 1);
		triangles.Add(vertices.Count - 2);
		triangles.Add(vertices.Count - 4);

		triangles.Add(vertices.Count - 1);
		triangles.Add(vertices.Count - 3);
		triangles.Add(vertices.Count - 2);

		triangles.Add(vertices.Count - 1);
		triangles.Add(vertices.Count - 5);
		triangles.Add(vertices.Count - 3);
		//}

	}

	private void UpdateMesh()
	{
		mesh.Clear();

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

	}
}