using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

using UnityEngine.UI;

namespace SVS
{
	public class LSystemGenerator : MonoBehaviour
	{
		public Rule[] rules;
		public string rootSentence;

		[Range(0, 10)]
		public int iterationLimit = 1;

		public bool randomIgnoreRuleModifier = true;

		[Range(0, 1)]
		public float chanceToIgnoreRule = 0.3f;

		public InputField rootSentenceInput;
		public InputField iterationInput;
		public InputField ignoreRuleInput;

		private void Start()
		{
			Debug.Log(GenerateSentence());

			rootSentenceInput.onEndEdit.AddListener(delegate { setRootSentence(rootSentenceInput); });
			iterationInput.onEndEdit.AddListener(delegate { setIterationLimit(iterationInput); });
			ignoreRuleInput.onEndEdit.AddListener(delegate { setIgnoreRule(ignoreRuleInput); });		
		}

		void setIterationLimit(InputField userInput)
		{
			int value = int.Parse(userInput.text);
			if (value >= 0 && value <= 20)
			{
				iterationLimit = int.Parse(userInput.text);
			}
		}

		void setIgnoreRule(InputField userInput)
		{
			int value = int.Parse(userInput.text);
			if (value >= 0 && value <= 1)
			{
				chanceToIgnoreRule = float.Parse(userInput.text);
			}
		}

		void setRootSentence(InputField userInput)
		{
			rootSentence = userInput.text;
		}

		public string GenerateSentence(string word = null)
		{
			if(word == null)
			{
				word = rootSentence;
			}
			return GrowRecursive(word);
		}

		private string GrowRecursive(string word, int currentIndex = 0)
		{
			if (currentIndex >= iterationLimit)
			{
				return word;
			}

			StringBuilder newWord = new StringBuilder();

			foreach(var c in word)
			{
				newWord.Append(c);
				ProcessRulesRecursively(newWord, c, currentIndex);
			}

			return newWord.ToString();
		}

		private void ProcessRulesRecursively(StringBuilder newWord, char c, int currentIndex)
		{
			foreach (var rule in rules)
			{
				if(rule.letter == c.ToString())
				{
					if (randomIgnoreRuleModifier  && currentIndex > 1)
					{
						//randomly ignore branches
						if (Random.value < chanceToIgnoreRule)
						{
							return;
						}
					}
					newWord.Append(GrowRecursive(rule.GetResult(), ++currentIndex));
				}
			}
		}
	}
}