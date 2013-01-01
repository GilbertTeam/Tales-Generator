using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace TalesGenerator.Text
{
	internal class SentenceTokenDefaultEqualityComparer : IEqualityComparer<SentenceToken>
	{
		public bool Equals(SentenceToken x, SentenceToken y)
		{
			if (object.ReferenceEquals(x, y))
			{
				return true;
			}
			if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
			{
				return object.ReferenceEquals(x, y);
			}

			return
				string.Equals(x.Lemma, y.Lemma, StringComparison.CurrentCultureIgnoreCase)/* &&
				x.PartOfSpeech == y.PartOfSpeech*/;
		}

		public int GetHashCode(SentenceToken sentenceToken)
		{
			Contract.Requires<ArgumentNullException>(sentenceToken != null);

			return sentenceToken.Text.GetHashCode() ^ (int)sentenceToken.PartOfSpeech;
		}
	}
}
