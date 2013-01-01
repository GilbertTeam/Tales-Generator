using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace TalesGenerator.Text.Parser
{
	internal class SentenceTokenFullEqualityComparer : IEqualityComparer<SentenceToken>
	{
		public bool Equals(SentenceToken firstSentenceToken, SentenceToken secondSentenceToken)
		{
			if (object.ReferenceEquals(firstSentenceToken, secondSentenceToken))
			{
				return true;
			}
			if (object.ReferenceEquals(firstSentenceToken, null) || object.ReferenceEquals(secondSentenceToken, null))
			{
				return object.ReferenceEquals(firstSentenceToken, secondSentenceToken);
			}

			return string.Equals(firstSentenceToken.Text, secondSentenceToken.Text, StringComparison.CurrentCultureIgnoreCase)
				&& firstSentenceToken.PartOfSpeech == secondSentenceToken.PartOfSpeech
				&& firstSentenceToken.PartOfSentence == secondSentenceToken.PartOfSentence;
		}

		public int GetHashCode(SentenceToken sentenceToken)
		{
			Contract.Requires<ArgumentNullException>(sentenceToken != null);

			return sentenceToken.Text.GetHashCode() ^ (int)sentenceToken.PartOfSpeech ^ (int)sentenceToken.PartOfSentence;
		}
	}
}
