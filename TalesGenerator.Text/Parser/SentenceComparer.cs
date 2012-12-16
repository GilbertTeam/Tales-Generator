using System.Linq;
using System.Text.RegularExpressions;

namespace TalesGenerator.Text
{
	internal class SentenceComparer
	{
		private string[] Decompose(string text)
		{
			return Regex.Replace(text, @"[\.,]", string.Empty).ToLower().Split(' ');
		}

		public bool Compare(TemplateParserResult result, string sentence)
		{
			string[] resultWords = Decompose(result.Text);
			string[] sentenceWords = Decompose(sentence);

			if (resultWords.Length == sentenceWords.Length)
			{
				for (int i = 0; i < resultWords.Length; i++)
				{
					if (resultWords[i] != sentenceWords[i])
					{
						return false;
					}
				}

				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
