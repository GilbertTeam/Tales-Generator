using System;
using System.Text.RegularExpressions;

namespace TalesGenerator.Text
{
	internal class ExactMatchSentenceComparer : ISentenceComparer
	{
		#region Methods

		private string[] Decompose(string text)
		{
			return LexerUtils.TrimText(text).Split(' ');
		}

		public bool Compare(string firstSentence, string secondSentence)
		{
			if (firstSentence == null ||
				secondSentence == null)
			{
				if (firstSentence == secondSentence)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			string[] firstSentenceWords = Decompose(firstSentence);
			string[] secondSentenceWords = Decompose(secondSentence);

			if (firstSentenceWords.Length == secondSentenceWords.Length)
			{
				for (int i = 0; i < firstSentenceWords.Length; i++)
				{
					if (!string.Equals(firstSentenceWords[i], secondSentenceWords[i], StringComparison.CurrentCultureIgnoreCase))
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
		#endregion
	}
}
