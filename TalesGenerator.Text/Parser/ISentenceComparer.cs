using System;

namespace TalesGenerator.Text
{
	internal interface ISentenceComparer
	{
		bool Compare(string firstSentence, string secondSentence);
	}
}
