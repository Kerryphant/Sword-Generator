using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiltGenerator : MonoBehaviour
{
	public bool generated = false;

	void Start()
    {
	

	}


    void Update()
    {
		ProceduralHilt mesh = transform.GetComponent<ProceduralHilt>();
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
