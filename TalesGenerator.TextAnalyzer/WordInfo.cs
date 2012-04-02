using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalesGenerator.TextAnalyzer
{
	public enum SpeechPart
	{
		Adverb,
		Noun,
		Pronoun,
		Preposition,
		Undefined,
		Verb
	}

	public class WordInfo
	{
		#region Properties

		public string Word { get; private set; }

		public string NormalForm { get; private set; }

		public SpeechPart SpeechPart { get; private set; }

		public bool Fuzzy { get; private set; }
		#endregion

		#region Constructors

		public WordInfo(string word, string normalForm, SpeechPart speechPart)
		{
			Word = word;
			NormalForm = normalForm;
			SpeechPart = speechPart;
		}

		public WordInfo(string word, string normalForm, SpeechPart speechPart, bool fuzzy)
			: this(word, normalForm, speechPart)
		{
			Fuzzy = fuzzy;
		}
		#endregion

		#region Methods

		public override string ToString()
		{
			return string.Format("Word = {0}, NormalForm = {1}, SpeechPart = {2}, Fuzzy = {3}", Word, NormalForm, SpeechPart, Fuzzy);
		}
		#endregion
	}
}
