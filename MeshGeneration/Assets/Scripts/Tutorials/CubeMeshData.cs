using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CubeMeshData
{
	public static Vector3[] vertices =
	{
		//north face
		new Vector3(1, 1, 1),
		new Vector3(-1, 1, 1),
		new Vector3(-1, -1, 1),
		new Vector3(1, -1, 1),

		//south face
		new Vector3(-1, 1, -1),
		new Vector3(1, 1, -1),
		new Vector3(1, -1, -1),
		new Vector3(-1, -1, -1)
	};

	public static int[][] faceTriangles =
	{
		//north
		new int[] {0, 1, 2, 3 },

		//east
		new int[] {5, 0, 3, 6 },

		//south
		new int[] {4, 5, 6, 7 },

		//west
		new int[] {1, 4, 7, 2 },

		//up
		new int[] {1, 0, 5, 4 },

		//down
		new int[] {7, 6, 3, 2 }
	};

	public static Vector3[] faceVertices (int dir, float faceScale, Vector3 facePos)
	{
		Vector3[] fv = new Vector3[4];

		for(int i = 0; i < fv.Length; i++)
		{
			fv[i] = (vertices[faceTriangles[dir][i]] * faceScale) + facePos;
		}

		return fv;
	}

	public static Vector3[] faceVertices(Direction dir, float faceScale, Vector3 facePos)
	{
		return faceVertices((int) dir, faceScale, facePos);
	}
}
