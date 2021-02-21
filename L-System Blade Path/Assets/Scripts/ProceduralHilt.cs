using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralHilt : MonoBehaviour
{
	public bool generated = false;

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
		if(generated)
		{
			pommel.transform.position = new Vector3(0, 0, -(grip.GetComponent<ProceduralCuboid>().depth + (pommel.GetComponent<ProceduralSphere>().radius / 2)));
			
			float offset = -(guard.GetComponent<ProceduralCuboid>().depth / 2);
			guard.transform.position = new Vector3(offset, -0.5f, 0);

			grip.transform.position = new Vector3(-0.5f, -0.51f, -(grip.GetComponent<ProceduralCuboid>().depth));
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
}
