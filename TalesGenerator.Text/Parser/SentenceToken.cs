using System;
using System.Diagnostics.Contracts;

namespace TalesGenerator.Text
{
	public enum PartOfSentence
	{
		/// <summary>
		/// Подлежащее.
		/// </summary>
		Subject,

		/// <summary>
		/// Сказуемое.
		/// </summary>
		Predicate,

		/// <summary>
		/// Дополнение.
		/// </summary>
		Object,

		/// <summary>
		/// Обращение.
		/// </summary>
		Appeal,

		Unknown
	}

	public class SentenceToken
	{
		#region Properties

		public PartOfSentence PartOfSentence { get; set; }

		public PartOfSpeech PartOfSpeech { get; set; }

		public string Lemma { get; private set; }

		public string Text { get; private set; }
		#endregion

		#region Constructors

		public SentenceToken(string text, string lemma)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(text));
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(lemma));

			PartOfSentence = PartOfSentence.Unknown;
			PartOfSpeech = PartOfSpeech.UNKNOWN;
			Text = text;
			Lemma = lemma;
		}
		#endregion

		#region Methods

		public override string ToString()
		{
			return string.Format("Text = \"{0}\". Lemma = \"{1}\". PartOfSpeech = {2}. PartOfSentence = {3}.", Text, Lemma, PartOfSpeech, PartOfSentence);
		}
		#endregion
	}
}
