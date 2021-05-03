using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralHilt : MonoBehaviour
{
	public bool generated = false;
	public bool bladeSmoothed = false;
	public bool hiltSmoothed = false;

	private bool adjustedForBladeSmooth = false;
	private bool adjustedForHiltSmooth = false;

	public GameObject grip;
	public GameObject guard;
	public GameObject pommel;

	public Material matGrip;
	public Material matGuard;
	public Material matPommel;

	// Start is called before the first frame update
	void Start()
	{
		grip.transform.position = new Vector3(-0.5f, -0.51f, -3);
		guard.transform.RotateAround(guard.transform.position, new Vector3(0, 1, 0), 90);
	}

	// Update is called once per frame
	void Update()
	{
		if (!bladeSmoothed && adjustedForBladeSmooth)
		{
			adjustedForBladeSmooth = false;
		}
		else if (bladeSmoothed && !adjustedForBladeSmooth)
		{

			grip.transform.position = new Vector3(grip.transform.position.x, grip.transform.position.y, grip.transform.position.z + 0.5f);
			guard.transform.position = new Vector3(guard.transform.position.x, guard.transform.position.y, guard.transform.position.z + 0.5f);
			pommel.transform.position = new Vector3(pommel.transform.position.x, pommel.transform.position.y, pommel.transform.position.z + 0.5f);

			adjustedForBladeSmooth = true;
		}

		if (!hiltSmoothed && adjustedForHiltSmooth)
		{
			adjustedForBladeSmooth = false;
		}
		else if (hiltSmoothed && !adjustedForHiltSmooth)
		{

			grip.transform.position = new Vector3(grip.transform.position.x, grip.transform.position.y, grip.transform.position.z + 0.5f);
			guard.transform.position = new Vector3(guard.transform.position.x, guard.transform.position.y, guard.transform.position.z + 0.5f);
			pommel.transform.position = new Vector3(pommel.transform.position.x, pommel.transform.position.y, pommel.transform.position.z + 0.5f);

			adjustedForHiltSmooth = true;
		}


		if (generated)
		{
			float pommelZOffset = -(grip.GetComponent<ProceduralCuboid>().depth + (pommel.GetComponent<ProceduralSphere>().radius / 2));

			float guardXOffset = -(guard.GetComponent<ProceduralCuboid>().depth / 2);

			float gripZOffset = -(grip.GetComponent<ProceduralCuboid>().depth);

			pommel.transform.position = new Vector3(0, 0, pommelZOffset);
			
			guard.transform.position = new Vector3(guardXOffset, -0.5f, 0);

			grip.transform.position = new Vector3(-0.5f, -0.51f, gripZOffset - 0.1f);

			
		}
		else
		{
			Destroy(guard.GetComponent<ProceduralCuboid>());
			Destroy(grip.GetComponent<ProceduralCuboid>());
			Destroy(pommel.GetComponent<ProceduralSphere>());
		}
		
	}

	public void GenerateHilt()
	{
		GenerateGuard();
		GenerateGrip();
		GeneratePommel();
	}

	private void GenerateGuard()
	{
		guard.AddComponent<ProceduralCuboid>();
		guard.GetComponent<Renderer>().material = matGuard;
		guard.GetComponent<ProceduralCuboid>().depth = UnityEngine.Random.Range(3, 7);
		guard.AddComponent<CuboidData>();
		guard.GetComponent<CuboidData>().depth = guard.GetComponent<ProceduralCuboid>().depth;
		guard.GetComponent<ProceduralCuboid>().MakeCuboid();

		switch (UnityEngine.Random.Range(1,3))
		{
			case 1:
				{
					Console.WriteLine("Case 1");
					break;
				}		
			case 2:
				{
					Console.WriteLine("Case 2");
					break;
				}
			case 3:
				{
					Console.WriteLine("Case 3");
					break;
				}
			default:
				{
					Console.WriteLine("Unexpected guard selection");
					break;
				}
		}
	}

	private void GenerateGrip()
	{
		grip.AddComponent<ProceduralCuboid>();
		grip.GetComponent<Renderer>().material = matGrip;
		grip.GetComponent<ProceduralCuboid>().depth = UnityEngine.Random.Range(3, 10);
		grip.AddComponent<CuboidData>();
		grip.GetComponent<CuboidData>().depth = grip.GetComponent<ProceduralCuboid>().depth;
		grip.GetComponent<ProceduralCuboid>().MakeCuboid();

		switch (UnityEngine.Random.Range(1, 3))
		{
			case 1:
				{
					Console.WriteLine("Case 1");
					break;
				}
			case 2:
				{
					Console.WriteLine("Case 2");
					break;
				}
			case 3:
				{
					Console.WriteLine("Case 3");
					break;
				}
			default:
				{
					Console.WriteLine("Unexpected grip selection");
					break;
				}
		}
	}

	private void GeneratePommel()
	{
		pommel.AddComponent<ProceduralSphere>();
		pommel.GetComponent<Renderer>().material = matPommel;
		pommel.GetComponent<ProceduralSphere>().MakeSphere();

		switch (UnityEngine.Random.Range(1, 3))
		{
			case 1:
				{
					Console.WriteLine("Case 1");
					//hilt.AddComponent<ProceduralSphere>();
					//hilt.GetComponent<ProceduralSphere>().MakeSphere();
					break;
				}
			case 2:
				{
					Console.WriteLine("Case 2");
					break;
				}
			case 3:
				{
					Console.WriteLine("Case 3");
					break;
				}
			default:
				{
					Console.WriteLine("Unexpected pommel selection");
					break;
				}
		};
	}

	public void SmoothHilt()
	{
		Vector3 guardPos = guard.transform.position;
		Vector3 gripPos = grip.transform.position;

		grip.AddComponent<ProceduralCuboid>();
		grip.GetComponent<ProceduralCuboid>().smooth = true;
		grip.GetComponent<Renderer>().material = matGrip;
		grip.GetComponent<ProceduralCuboid>().depth = grip.GetComponent<CuboidData>().depth;
		grip.GetComponent<ProceduralCuboid>().MakeCuboid();
		grip.GetComponent<ProceduralCuboid>().SmoothMesh();
		grip.GetComponent<ProceduralCuboid>().UpdateMesh();

		guard.AddComponent<ProceduralCuboid>();
		guard.GetComponent<ProceduralCuboid>().smooth = true;
		guard.GetComponent<Renderer>().material = matGuard;
		guard.GetComponent<ProceduralCuboid>().depth = guard.GetComponent<CuboidData>().depth;
		guard.GetComponent<ProceduralCuboid>().MakeCuboid();
		guard.GetComponent<ProceduralCuboid>().SmoothMesh();
		guard.GetComponent<ProceduralCuboid>().UpdateMesh();

		hiltSmoothed = true;
	}				

}
