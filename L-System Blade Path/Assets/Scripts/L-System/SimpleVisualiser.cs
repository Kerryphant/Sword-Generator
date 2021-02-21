using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVS
{
	public class SimpleVisualiser : MonoBehaviour
	{
		public LSystemGenerator lSystem;
		HashSet<Vector3> positions = new HashSet<Vector3>(); //hash set to avoid duplicate points
		public GameObject prefab;
		public Material lineMaterial;

		[SerializeField]
		private int length = 1;

		[SerializeField]
		private float angle = 90;

		[Range(0, 1)]
		public float chanceToChooseAngle = 0.3f;


		public int Length
		{
			get
			{
				if(length > 0)
				{
					return length;
				}
				else
				{
					return 1;
				}
			}
			set => length = value;
		}

		private void Start()
		{
			var sequence = lSystem.GenerateSentence();
			VisualiseSequence(sequence);
		}

		private void Update()
		{
			if(Input.GetKeyDown("space"))
			{
				var points = GameObject.FindGameObjectsWithTag("point");

				foreach (var item in points)
				{
					DestroyImmediate(item);
				}

				positions.Clear();

				var sequence = lSystem.GenerateSentence();
				Debug.Log(sequence);
				VisualiseSequence(sequence);
				

				transform.GetComponent<BladeGen>().nodesFound = false;
				transform.GetComponentInChildren<HiltGen>().generated = false;
				transform.GetComponent<ProceduralBlade>().valuesPassed = false;
			}	
		}

		private void VisualiseSequence(string sequence)
		{
			Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
			var currentPosition = Vector3.zero;

			Vector3 direction = Vector3.forward;
			Vector3 tempPos = Vector3.zero;

			positions.Add(currentPosition);

			foreach (var letter in sequence)
			{
				EncodingLetters encoding = (EncodingLetters)letter;

				switch (encoding)
				{
					case EncodingLetters.save:
						savePoints.Push(new AgentParameters
						{
							position = currentPosition,
							direction = direction,
							length = Length
						});
						break;
					case EncodingLetters.load:
						if(savePoints.Count > 0)
						{
							var agentParameter = savePoints.Pop();
							currentPosition = agentParameter.position;
							direction = agentParameter.direction;
							Length = agentParameter.length;
						}
						else
						{
							throw new System.Exception("Don't have saved point in our stack");
						}
						break;
					case EncodingLetters.draw:
						tempPos = currentPosition;
						currentPosition += direction * length;
						positions.Add(currentPosition);
						break;
					case EncodingLetters.turnRight:
						if (UnityEngine.Random.value < chanceToChooseAngle)
						{
							float tempAngle = (UnityEngine.Random.Range(10, 50));
							direction = Quaternion.AngleAxis(UnityEngine.Random.Range(10, 30), Vector3.up) * direction;
						}
						else
						{
							direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
						}				
						break;
					case EncodingLetters.turnLeft:
						if (UnityEngine.Random.value < chanceToChooseAngle)
						{
							float tempAngle = -(UnityEngine.Random.Range(10, 50));
							direction = Quaternion.AngleAxis(tempAngle, Vector3.up) * direction;
						}
						else
						{
							direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
						}					
						break;
					default:
						break;
				}
			}

			foreach (var position in positions)
			{
				Instantiate(prefab, position, Quaternion.identity, transform);
			}
		}

		private void DrawLine(Vector3 start, Vector3 end, Color colour)
		{
			GameObject line = new GameObject("line");
			line.transform.position = start;
			var lineRenderer = line.AddComponent<LineRenderer>();
			lineRenderer.material = lineMaterial;
			lineRenderer.startColor = colour;
			lineRenderer.endColor = colour;
			lineRenderer.startWidth = 0.1f;
			lineRenderer.endWidth = 0.1f;
			lineRenderer.SetPosition(0, start);
			lineRenderer.SetPosition(1, end);
			line.transform.SetParent(transform);
		}

		public enum EncodingLetters
		{
			unknown = '1',
			save = '[',
			load = ']',
			draw = 'F',
			turnRight = '+',
			turnLeft = '-'
		}

	}
}
