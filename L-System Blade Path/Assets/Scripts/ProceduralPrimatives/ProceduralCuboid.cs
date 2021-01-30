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

	public bool valuesPassed;


	public Material mat;
	public Material lineMaterial;

	GameObject line;
	LineRenderer lineRenderer;

	// Start is called before the first frame update
	void Awake()
	{
		mesh = GetComponent<MeshFilter>().mesh;

		print(RotatePointAroundPivot(new Vector3(1, 0, 0), new Vector3(2, 0, 0), new Vector3(0, -90, 0)));
		print(SignedAngleBetween(new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0)));
	}

	private void Start()
	{
		//MakeCuboid();
		//UpdateMesh();

		GetComponent<Renderer>().material = mat;
	}

	void Update()
	{
		/*if ((width != prevWidth) || (height != prevHeight) || (depth != prevDepth))
		{
			prevWidth = width;
			prevHeight = height;
			prevDepth = depth;
		}
		
		 MakeCuboid();*/

		if (valuesPassed)
		{
			UpdateMesh();
		}


	}

	public void GeneratePathedBlade(int numPoints_, List<Vector3> points_)
	{
		GenerateCuboidLine(numPoints_);
		PositionVerticesToPoints(numPoints_, points_);

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
			//get the angle between the x-axis and the line made from the current point to the next point
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

	private void MakeCuboid()
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

	/*public void GeneratePathedCuboid(int numPoints_, List<Vector3> points_)
	{
		int cuboidCount = numPoints_ - 1;
		List<bool> verticalFace = new List<bool>();


		if (numPoints_ > 0)
		{
			vertices = new List<Vector3>();
			for (int i = 0; i < numPoints_; i++)
			{

				if (i != 0 && points_[i] == points_[i - 1])
				{
					//there is a duplicate in this set
				}
				else
				{
					//(not first iteration, and the previous x is not the same as current x) and (next item exists and next z is less than current z and next x is not the same as current x) or (next item exists and its z is the same as the current z and the next x is not the same as the current x)
					if (((i != 0 && (points_[i - 1].x != points_[i].x)) && ((i + 1 < numPoints_) && (points_[i + 1].z < points_[i].z) && (points_[i + 1].x != points_[i].x)))
					|| ((i + 1 < numPoints_) && (points_[i + 1].z == points_[i].z) && (points_[i + 1].x != points_[i].x))
					|| ((i + 1 >= numPoints_) && points_[i].z == points_[i - 1].z))
					{
						if (points_[i].x < 0)
						{
							//bottom left
							vertices.Add(new Vector3(points_[i].x, points_[i].y - (0.5f * height), points_[i].z - (0.5f * width)));
							//top left
							vertices.Add(new Vector3(points_[i].x, points_[i].y + (0.5f * height), points_[i].z - (0.5f * width)));
							//bottom right
							vertices.Add(new Vector3(points_[i].x, points_[i].y - (0.5f * height), points_[i].z + (0.5f * width)));
							//top right
							vertices.Add(new Vector3(points_[i].x, points_[i].y + (0.5f * height), points_[i].z + (0.5f * width)));
						}
						else
						{
							//bottom left
							vertices.Add(new Vector3(points_[i].x, points_[i].y - (0.5f * height), points_[i].z + (0.5f * width)));
							//top left
							vertices.Add(new Vector3(points_[i].x, points_[i].y + (0.5f * height), points_[i].z + (0.5f * width)));
							//bottom right
							vertices.Add(new Vector3(points_[i].x, points_[i].y - (0.5f * height), points_[i].z - (0.5f * width)));
							//top right
							vertices.Add(new Vector3(points_[i].x, points_[i].y + (0.5f * height), points_[i].z - (0.5f * width)));

						}


						verticalFace.Add(true);
					}
					else
					{
						//bottom left
						vertices.Add(new Vector3(points_[i].x - (0.5f * width), points_[i].y - (0.5f * height), points_[i].z));
						//top left
						vertices.Add(new Vector3(points_[i].x - (0.5f * width), points_[i].y + (0.5f * height), points_[i].z));
						//bottom right
						vertices.Add(new Vector3(points_[i].x + (0.5f * width), points_[i].y - (0.5f * height), points_[i].z));
						//top right
						vertices.Add(new Vector3(points_[i].x + (0.5f * width), points_[i].y + (0.5f * height), points_[i].z));

						verticalFace.Add(false);
					}
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
				if (verticalFace[i])
				{
					triangles.Add(startVert + 4);
					triangles.Add(startVert + 5);
					triangles.Add(startVert + 6);
					triangles.Add(startVert + 6);
					triangles.Add(startVert + 5);
					triangles.Add(startVert + 7);
				}
				else
				{
					triangles.Add(startVert + 6);
					triangles.Add(startVert + 7);
					triangles.Add(startVert + 4);
					triangles.Add(startVert + 4);
					triangles.Add(startVert + 7);
					triangles.Add(startVert + 5);
				}


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
	}*/
}