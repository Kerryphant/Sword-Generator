using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autodesk.Fbx;

public static class Exporter
{
	public static void ExportMesh (string fileName, List<Vector3> vertices_, List<Vector3> normals_,  List<int> triangles_)
	{
		using (FbxManager fbxManager = FbxManager.Create())
		{
		// configure IO settings.
			fbxManager.SetIOSettings(FbxIOSettings.Create(fbxManager, Globals.IOSROOT));

			// Export the scene
			using (FbxExporter exporter = FbxExporter.Create(fbxManager, "myExporter"))
			{

				// Initialize the exporter.
				bool status = exporter.Initialize(fileName, -1, fbxManager.GetIOSettings());

				// Create a new scene to export
				FbxScene scene = FbxScene.Create(fbxManager, "myScene");

				FbxMesh mesh = FbxMesh.Create(fbxManager, "myMesh");

				mesh.InitControlPoints(vertices_.Count);

				for (int i = 0; i < vertices_.Count; i++)
				{
					FbxVector4 pos = new FbxVector4(vertices_[i].x, vertices_[i].y, vertices_[i].z);
					mesh.SetControlPointAt(pos, i);
				}

				for (int i = 0; i < normals_.Count; i++)
				{
					FbxVector4 normal = new FbxVector4(normals_[i].x, normals_[i].y, normals_[i].z);
					mesh.SetControlPointAt(normal, i);
				}

				for (int i = 0; i < triangles_.Count; i+=3)
				{
					mesh.BeginPolygon();
					mesh.AddPolygon(triangles_[i]);
					mesh.AddPolygon(triangles_[i + 1]);
					mesh.AddPolygon(triangles_[i + 2]);
					mesh.EndPolygon();
				}

				scene.AddMember(mesh);

				// Export the scene to the file.
				exporter.Export(scene);
			}

		}
	}

	//FbxExporters.Editor.ConvertToModel.Convert(target.gameObject, null, savePath, null, null, null);
	//Whereever, you include this line of code make sure it is in a folder called Editor. Otherwise the class FbxExporters.Editor won’t be visible
}
