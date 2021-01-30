using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVS
{
	[CreateAssetMenu(menuName ="ProceduralCity")]
	public class Rule : ScriptableObject
	{
		public string letter;

		[SerializeField]
		private string[] results = null;

		[SerializeField]
		private bool randomResults = false;

		public string GetResult()
		{
			if(randomResults)
			{
				int randomIndex = UnityEngine.Random.Range(0, results.Length);
				return results[randomIndex];
			}
			return results[0];
		}

	}
}