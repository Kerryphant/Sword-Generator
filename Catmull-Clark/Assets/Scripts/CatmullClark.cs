using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullClark : MonoBehaviour
{
	public void SubdivideMesh(ref List<Vector3> origVertices_, ref List<int> triangles_, ref List<int> quads_, ref List<Edge> edges_, ref List<FacePoint> faces_)
	{
		List<FacePoint> facePoints = new List<FacePoint>();
		//For each face
		for (int i = 0; i < faces_.Count; i++)
		{
			//Set each face point to be the average of all original points for the respective face
			Vector3 facePoint = (faces_[i].parentVertValue[0] + faces_[i].parentVertValue[1] + faces_[i].parentVertValue[2] + faces_[i].parentVertValue[3]) * 0.25f;
			FacePoint newFacePoint = new FacePoint(faces_[i].parentVertValue, facePoint);

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
				if (facePoints[j].parentVertValue.Contains(origVertices_[i]))
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
				if (facePoints[i].parentVertValue.Contains(edges_[j].startVert) && facePoints[i].parentVertValue.Contains(edges_[j].endVert))
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
				if (origVertices_[i] == edges_[j].startVert || origVertices_[i] == edges_[j].endVert)
				{
					Edge tempEdge = new Edge(newVertexPoints[i], edgePoints[j]);
					newVertEdges.Add(tempEdge);
				}
			}
		}

		List<Vector3> finalVertList = new List<Vector3>();
		HashSet<Vector3> vertHashLookup = new HashSet<Vector3>();

		List<int> finalTriangles = new List<int>();

		//for each of the faces
		for (int i = 0; i < facePoints.Count; i++)
		{
			//List<List<Edge>> localFaceEdge = new List<List<Edge>>();

			AddFinalVert(facePoints[i].point, ref vertHashLookup, ref finalVertList);

			List<Edge> tempLeftSide = new List<Edge>();
			List<Edge> tempTopSide = new List<Edge>();
			List<Edge> tempRightSide = new List<Edge>();
			List<Edge> tempBotSide = new List<Edge>();

			//for each of the new vert edges
			for (int j = 0; j < newVertEdges.Count; j++)
			{
				//if the edge contains element 0 or 1 of this face's parents	
				if (newVertEdges[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[0])] || newVertEdges[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[1])])
				{
					//store it
					tempLeftSide.Add(newVertEdges[j]);
				}

				//if the edge contains element 1 or 3 of this face's parents
				if (newVertEdges[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[1])] || newVertEdges[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[2])])
				{
					//store it
					tempTopSide.Add(newVertEdges[j]);
				}

				//if the edge contains element 3 or 2 of this face's parents
				if (newVertEdges[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[2])] || newVertEdges[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[3])])
				{
					//store it
					tempRightSide.Add(newVertEdges[j]);
				}

				//if the edge contains element 2 or 0 of this face's parents
				if (newVertEdges[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[3])] || newVertEdges[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[0])])
				{
					//store it
					tempBotSide.Add(newVertEdges[j]);
				}
			}

			int[] matchingEdgesIndexes = new int[8];

			for (int j = 0; j < tempLeftSide.Count; j++)
			{
				Edge leftTarget = tempLeftSide[j];
				Edge topTarget = tempTopSide[j];
				Edge rightTarget = tempRightSide[j];
				Edge botTarget = tempBotSide[j];

				for (int k = 0; k < tempLeftSide.Count; k++)
				{
					if (tempLeftSide[k].startVert != leftTarget.startVert && tempLeftSide[k].endVert == leftTarget.endVert)
					{
						//if this edge starts at the bottom left of the face
						if (tempLeftSide[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[0])])
						{
							matchingEdgesIndexes[0] = j;
							matchingEdgesIndexes[1] = k;

							AddFinalVert(tempLeftSide[j], ref vertHashLookup, ref finalVertList);
							AddFinalVert(tempLeftSide[k], ref vertHashLookup, ref finalVertList);
						}
						else
						{
							matchingEdgesIndexes[0] = k;
							matchingEdgesIndexes[1] = j;

							AddFinalVert(tempLeftSide[j], ref vertHashLookup, ref finalVertList);
							AddFinalVert(tempLeftSide[k], ref vertHashLookup, ref finalVertList);
						}
					}

					if (tempTopSide[k].startVert != topTarget.startVert && tempTopSide[k].endVert == topTarget.endVert)
					{

						if (tempTopSide[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[1])])
						{
							matchingEdgesIndexes[2] = j;
							matchingEdgesIndexes[3] = k;

							AddFinalVert(tempTopSide[j], ref vertHashLookup, ref finalVertList);
							AddFinalVert(tempTopSide[k], ref vertHashLookup, ref finalVertList);
						}
						else
						{
							matchingEdgesIndexes[2] = k;
							matchingEdgesIndexes[3] = j;

							AddFinalVert(tempTopSide[j], ref vertHashLookup, ref finalVertList);
							AddFinalVert(tempTopSide[k], ref vertHashLookup, ref finalVertList);
						}
					}

					if (tempRightSide[k].startVert != rightTarget.startVert && tempRightSide[k].endVert == rightTarget.endVert)
					{
						if (tempRightSide[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[2])])
						{
							matchingEdgesIndexes[4] = j;
							matchingEdgesIndexes[5] = k;

							AddFinalVert(tempRightSide[j], ref vertHashLookup, ref finalVertList);
							AddFinalVert(tempRightSide[k], ref vertHashLookup, ref finalVertList);
						}
						else
						{
							matchingEdgesIndexes[4] = k;
							matchingEdgesIndexes[5] = j;

							AddFinalVert(tempRightSide[j], ref vertHashLookup, ref finalVertList);
							AddFinalVert(tempRightSide[k], ref vertHashLookup, ref finalVertList);
						}
					}

					if (tempBotSide[k].startVert != botTarget.startVert && tempBotSide[k].endVert == botTarget.endVert)
					{
						if (tempBotSide[j].startVert == newVertexPoints[origVertices_.IndexOf(facePoints[i].parentVertValue[3])])
						{
							matchingEdgesIndexes[6] = j;
							matchingEdgesIndexes[7] = k;

							AddFinalVert(tempBotSide[j], ref vertHashLookup, ref finalVertList);
							AddFinalVert(tempBotSide[k], ref vertHashLookup, ref finalVertList);
						}
						else
						{
							matchingEdgesIndexes[6] = k;
							matchingEdgesIndexes[7] = j;

							AddFinalVert(tempBotSide[j], ref vertHashLookup, ref finalVertList);
							AddFinalVert(tempBotSide[k], ref vertHashLookup, ref finalVertList);
						}
					}
				}
			}

			//bottom left
			finalTriangles.Add(finalVertList.IndexOf(tempLeftSide[matchingEdgesIndexes[0]].startVert));
			finalTriangles.Add(finalVertList.IndexOf(tempLeftSide[matchingEdgesIndexes[0]].endVert));
			finalTriangles.Add(finalVertList.IndexOf(tempBotSide[matchingEdgesIndexes[7]].endVert));

			finalTriangles.Add(finalVertList.IndexOf(tempBotSide[matchingEdgesIndexes[7]].endVert));
			finalTriangles.Add(finalVertList.IndexOf(tempLeftSide[matchingEdgesIndexes[0]].endVert));
			finalTriangles.Add(finalVertList.IndexOf(facePoints[i].point));


			//top left
			finalTriangles.Add(finalVertList.IndexOf(tempLeftSide[matchingEdgesIndexes[1]].endVert));
			finalTriangles.Add(finalVertList.IndexOf(tempLeftSide[matchingEdgesIndexes[1]].startVert));
			finalTriangles.Add(finalVertList.IndexOf(facePoints[i].point));

			finalTriangles.Add(finalVertList.IndexOf(facePoints[i].point));
			finalTriangles.Add(finalVertList.IndexOf(tempLeftSide[matchingEdgesIndexes[1]].startVert));
			finalTriangles.Add(finalVertList.IndexOf(tempTopSide[matchingEdgesIndexes[2]].endVert));

			//top right
			finalTriangles.Add(finalVertList.IndexOf(facePoints[i].point));
			finalTriangles.Add(finalVertList.IndexOf(tempTopSide[matchingEdgesIndexes[2]].endVert));
			finalTriangles.Add(finalVertList.IndexOf(tempRightSide[matchingEdgesIndexes[4]].endVert));

			finalTriangles.Add(finalVertList.IndexOf(tempRightSide[matchingEdgesIndexes[4]].endVert));
			finalTriangles.Add(finalVertList.IndexOf(tempTopSide[matchingEdgesIndexes[2]].endVert));
			finalTriangles.Add(finalVertList.IndexOf(tempTopSide[matchingEdgesIndexes[3]].startVert));

			//bottom right
			finalTriangles.Add(finalVertList.IndexOf(tempBotSide[matchingEdgesIndexes[6]].endVert));
			finalTriangles.Add(finalVertList.IndexOf(facePoints[i].point));
			finalTriangles.Add(finalVertList.IndexOf(tempBotSide[matchingEdgesIndexes[6]].startVert));

			finalTriangles.Add(finalVertList.IndexOf(tempBotSide[matchingEdgesIndexes[6]].startVert));
			finalTriangles.Add(finalVertList.IndexOf(facePoints[i].point));
			finalTriangles.Add(finalVertList.IndexOf(tempRightSide[matchingEdgesIndexes[5]].endVert));

		}

		origVertices_ = finalVertList;
		triangles_ = finalTriangles;
	}

	void AddFinalVert(Edge value, ref HashSet<Vector3> hashLookUp, ref List<Vector3> finalVerts)
	{					  
		if (!hashLookUp.Contains(value.endVert))
		{
			hashLookUp.Add(value.endVert);
			finalVerts.Add(value.endVert);
		}
		if (!hashLookUp.Contains(value.startVert))
		{
			hashLookUp.Add(value.startVert);
			finalVerts.Add(value.startVert);
		}
		if (!hashLookUp.Contains(value.startVert))
		{
			hashLookUp.Add(value.startVert);
			finalVerts.Add(value.startVert);
		}
	}

	void AddFinalVert(Vector3 value, ref HashSet<Vector3> hashLookUp, ref List<Vector3> finalVerts)
	{
		if (!hashLookUp.Contains(value))
		{
			hashLookUp.Add(value);
			finalVerts.Add(value);
		}
		if (!hashLookUp.Contains(value))
		{
			hashLookUp.Add(value);
			finalVerts.Add(value);
		}
		if (!hashLookUp.Contains(value))
		{
			hashLookUp.Add(value);
			finalVerts.Add(value);
		}
	}

	void ConvertMeshTriToQuad(ref List<Vector3> vertices_, ref List<int> triangles_)
	{

	}

	void ConvertMeshQuadToTri(ref List<Vector3> vertices_, ref List<int> quads)
	{

	}
}

/*if(newVertEdges[i].startVert == newFaceEdges[j].startVert)
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
else*/
