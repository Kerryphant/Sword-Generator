using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiltGenerator : MonoBehaviour
{
	public bool generated = false;
	public bool bladeSmoothed = false;

	void Start()
    {
	

	}


    void Update()
    {


		ProceduralHilt mesh = transform.GetComponent<ProceduralHilt>();

		if (mesh.bladeSmoothed != bladeSmoothed)
		{
			mesh.bladeSmoothed = bladeSmoothed;
		}
		
		if (!generated)
		{	
			mesh.generated = true;
			mesh.GenerateHilt();

			generated = true;
		}
		else
		{
			mesh.generated = false;
		}
	}
}
