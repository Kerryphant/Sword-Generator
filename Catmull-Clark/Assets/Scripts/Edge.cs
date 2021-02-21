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

/*struct EdgePoint
{
	public EdgePoint()
	{

	}

	public Vector3 point;
}*/


public struct Face
{
	public Face(List<Vector3> corners_)
	{
		cornerVertices = corners_;
	}

	public List<Vector3> cornerVertices;
}

public struct FacePoint
{
	public FacePoint(List<Vector3> parentVertices_, Vector3 point_)
	{
		parentVertices = parentVertices_;
		point = point_;
	}

	public List<Vector3> parentVertices;
	public Vector3 point;
}