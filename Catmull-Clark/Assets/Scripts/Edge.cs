using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Edge
{
	public Edge(Vector3 start, Vector3 end)
	{
		startVert = start;
		endVert = end;

		face1 = -1;
		face2 = -1;
	}

	public Vector3 startVert;
	public Vector3 endVert;

	public int face1;
	public int face2;
}
public struct FacePoint
{
	public FacePoint(List<Vector3> parentVertices_, Vector3 point_)
	{
		parentVertValue = parentVertices_;
		point = point_;
		populated = true;
	}

	public FacePoint(List<Vector3> parentVertices_)
	{
		parentVertValue = parentVertices_;
		point = new Vector3();
		populated = false;
	}

	bool populated;
	public List<Vector3> parentVertValue;
	public Vector3 point;
}