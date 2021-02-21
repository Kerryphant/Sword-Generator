using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullClark : MonoBehaviour
{
	public void SubdivideMesh(ref List<Vector3> origVertices_, ref List<int> triangles_, ref List<Edge> edges_, ref List<Face>faces_)
	{
		List<FacePoint> facePoints = new List<FacePoint>();
		//For each face
		for (int i = 0; i < faces_.Count; i++)
		{
			//Set each face point to be the average of all original points for the respective face
			Vector3 facePoint = (faces_[i].cornerVertices[0] + faces_[i].cornerVertices[1] + faces_[i].cornerVertices[2] + faces_[i].cornerVertices[3]) * 0.25f;
			FacePoint newFacePoint = new FacePoint(faces_[i].cornerVertices, facePoint);

			//add a face point
			facePoints.Add(newFacePoint);
		}

		List<Vector3> edgePoints = new List<Vector3>();
		//For each edge
		for (int i = 0; i < edges_.Count; i++)
		{
			//Set each edge point to be the average of the two neighbouring face points and its two original endpoints
			Vector3 edgePoint = (edges_[i].startVert + edges_[i].endVert + facePoints[edges_[i].face1].point + facePoints[edges_[i].face2].point) * 0.25f;
			//add an edge point
			edgePoints.Add(edgePoint);
		}

		
		List<Vector3> newVertexPoints = new List<Vector3>();

		//For each original point (P)
		for (int i = 0; i < origVertices_.Count; i++)
		{
			Vector3 averageFacePoints = new Vector3(0, 0, 0);
			Vector3 averageEdgePoints = new Vector3(0, 0, 0);

			//List<Vector3> nearbyNewPoints = new List<Vector3>();
			int foundFacePoints = 0;
			//take the average(F) of all n(recently created) face points for faces touching P
			for (int j = 0; j < facePoints.Count; j++)
			{
				if(facePoints[j].parentVertices.Contains(origVertices_[i]))
				{
					averageFacePoints += facePoints[j].point;
					foundFacePoints++;
				}
			}
			averageFacePoints = averageFacePoints / foundFacePoints;

			int foundEdgePoints = 0;
			//take the average(R) of all n edge midpoints for original edges touching P, where each edge midpoint is the average of its two endpoint vertices
			for (int j = 0; j < edges_.Count; j++)
			{
				if (edges_[j].startVert == origVertices_[i] || edges_[j].endVert == origVertices_[i])
				{
					averageEdgePoints += (edges_[j].startVert + edges_[j].endVert) * 0.5f;
					foundEdgePoints++;
				}
			}
			averageEdgePoints = averageEdgePoints / foundEdgePoints;

			//Move each original point to the new vertex point  -  the barycenter of P, R and F with respective weights (n-3), 2 and 1
			Vector3 newPos = (averageEdgePoints + (2 * averageEdgePoints) + ((foundEdgePoints - 3) * origVertices_[i])) / foundEdgePoints;
			//vertices_[i] = newPos;
			newVertexPoints.Add(newPos);
		}

		List<Edge> newFaceEdges = new List<Edge>();
		//connect each new face point to new edge points of all original edges defining the original face
		for (int i = 0; i < facePoints.Count; i++)
		{
			for (int j = 0; j < edges_.Count; j++)
			{
				//if both start and end points are within the face's parent array
				if(facePoints[i].parentVertices.Contains(edges_[j].startVert) && facePoints[i].parentVertices.Contains(edges_[j].endVert))
				{
					Edge tempEdge = new Edge(facePoints[i].point, edgePoints[j]);
					newFaceEdges.Add(tempEdge);
				}

			}
		}

		List<Edge> newVertEdges = new List<Edge>();
		//connect each new vertex point to the new edge points of all original edges incident on the original vertex
		for (int i = 0; i < newVertexPoints.Count; i++)
		{
			for (int j = 0; j < edges_.Count; j++)
			{
				if(origVertices_[i] == edges_[j].startVert || origVertices_[i] == edges_[j].endVert)
				{
					Edge tempEdge = new Edge(newVertexPoints[i], edgePoints[j]);
					newVertEdges.Add(tempEdge);
				}
			}
		}

		List<Vector3> finalVertList = new List<Vector3>();
		HashSet<Vector3> vertHashLookup = new HashSet<Vector3>();

		List<int> finalTriangles = new List<int>();

		//define new triangles as enclosed by edges
		for (int i = 0; i < newVertEdges.Count; i++)
		{
			for (int j = 0; j < newFaceEdges.Count; j++)
			{
				//if the start vert matches
				if(newVertEdges[i].startVert == newFaceEdges[j].startVert)
				{
					//create a triangle
					if(!vertHashLookup.Contains(newVertEdges[i].startVert))
					{
						vertHashLookup.Add(newVertEdges[i].startVert);
						finalVertList.Add(newVertEdges[i].startVert);
					}
					if(!vertHashLookup.Contains(newFaceEdges[j].endVert))
					{
						vertHashLookup.Add(newFaceEdges[j].endVert);
						finalVertList.Add(newFaceEdges[j].endVert);
					}
					if(!vertHashLookup.Contains(newVertEdges[i].endVert))
					{
						vertHashLookup.Add(newVertEdges[i].endVert);
						finalVertList.Add(newVertEdges[i].endVert);
					}

					finalTriangles.Add(finalVertList.IndexOf(newVertEdges[i].startVert));
					finalTriangles.Add(finalVertList.IndexOf(newFaceEdges[j].endVert));
					finalTriangles.Add(finalVertList.IndexOf(newVertEdges[i].endVert));

				}
				else if (newVertEdges[i].startVert == newFaceEdges[j].endVert)
				{
					if (!vertHashLookup.Contains(newVertEdges[i].startVert))
					{
						vertHashLookup.Add(newVertEdges[i].startVert);
						finalVertList.Add(newVertEdges[i].startVert);
					}
					if (!vertHashLookup.Contains(newFaceEdges[j].startVert))
					{
						vertHashLookup.Add(newFaceEdges[j].startVert);
						finalVertList.Add(newFaceEdges[j].startVert);
					}
					if (!vertHashLookup.Contains(newVertEdges[i].endVert))
					{
						vertHashLookup.Add(newVertEdges[i].endVert);
						finalVertList.Add(newVertEdges[i].endVert);
					}

					finalTriangles.Add(finalVertList.IndexOf(newVertEdges[i].startVert));
					finalTriangles.Add(finalVertList.IndexOf(newFaceEdges[j].startVert));
					finalTriangles.Add(finalVertList.IndexOf(newVertEdges[i].endVert));
				}
				//if the end vert matches
				else if (newVertEdges[i].endVert == newFaceEdges[j].startVert)
				{
					if (!vertHashLookup.Contains(newVertEdges[i].endVert))
					{
						vertHashLookup.Add(newVertEdges[i].endVert);
						finalVertList.Add(newVertEdges[i].endVert);
					}
					if (!vertHashLookup.Contains(newFaceEdges[j].endVert)) 
					{
						vertHashLookup.Add(newFaceEdges[j].endVert);
						finalVertList.Add(newFaceEdges[j].endVert);
					}
					if (!vertHashLookup.Contains(newVertEdges[i].startVert))
					{
						vertHashLookup.Add(newVertEdges[i].startVert);
						finalVertList.Add(newVertEdges[i].startVert);
					}

					finalTriangles.Add(finalVertList.IndexOf(newVertEdges[i].endVert));
					finalTriangles.Add(finalVertList.IndexOf(newFaceEdges[j].endVert));
					finalTriangles.Add(finalVertList.IndexOf(newVertEdges[i].startVert));
				}
				else if (newVertEdges[i].endVert == newFaceEdges[j].endVert)
				{
					if (!vertHashLookup.Contains(newVertEdges[i].endVert))
					{
						vertHashLookup.Add(newVertEdges[i].endVert);
						finalVertList.Add(newVertEdges[i].endVert);
					}
					if (!vertHashLookup.Contains(newFaceEdges[j].startVert))
					{
						vertHashLookup.Add(newFaceEdges[j].startVert);
						finalVertList.Add(newFaceEdges[j].startVert);
					}
					if (!vertHashLookup.Contains(newVertEdges[i].startVert))
					{
						vertHashLookup.Add(newVertEdges[i].startVert);
						finalVertList.Add(newVertEdges[i].startVert);
					}

					finalTriangles.Add(finalVertList.IndexOf(newVertEdges[i].endVert));
					finalTriangles.Add(finalVertList.IndexOf(newFaceEdges[j].startVert));
					finalTriangles.Add(finalVertList.IndexOf(newVertEdges[i].startVert));
				}
			}
		}

		origVertices_ = finalVertList;
		triangles_ = finalTriangles;
	}

	void ConvertMeshTriToQuad(ref List<Vector3> vertices_, ref List<int> triangles_)
	{

	}

	void ConvertMeshQuadToTri(ref List<Vector3> vertices_, ref List<int> quads)
	{

	}
}

/*
for (int i = 0; i < faces; i++)
{
	Vector3 facePoint = (vertices_[startVert] + vertices_[startVert+1] + vertices_[startVert+2] + vertices_[startVert+3]) * 0.25f;
	facePoints.Add(facePoint);

	facePoint = (vertices_[startVert] + vertices_[startVert + 1] + vertices_[startVert + 4] + vertices_[startVert + 5]) * 0.25f;
	facePoints.Add(facePoint);

	facePoint = (vertices_[startVert+1] + vertices_[startVert + 3] + vertices_[startVert + 5]  + vertices_[startVert + 7]) * 0.25f;
	facePoints.Add(facePoint);

	facePoint = (vertices_[startVert+3] + vertices_[startVert + 7] + vertices_[startVert + 6] + vertices_[startVert + 2]) * 0.25f;
	facePoints.Add(facePoint);

	facePoint = (vertices_[startVert] + vertices_[startVert + 2] + vertices_[startVert + 6] + vertices_[startVert + 4]) * 0.25f;
	facePoints.Add(facePoint);

	startVert += 4;

}
*/
