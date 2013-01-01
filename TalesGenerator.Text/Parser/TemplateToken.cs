using System;
using System.Diagnostics.Contracts;

namespace TalesGenerator.Text
{
	public class TemplateToken : SentenceToken
	{
		#region Properties

		public Grammem Grammem { get; set; }

		public string TemplateItem { get; private set; }
		#endregion

		#region Constructors

		public TemplateToken(string text, string lemma, string templateItem)
			: base(text, lemma)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(templateItem));

			Grammem = Grammem.None;
			TemplateItem = templateItem;
		}
		#endregion

		#region Methods

		public override string ToString()
		{
			return string.Format("Text = \"{0}\". Lemma = \"{1}\". PartOfSpeech = {2}. PartOfSentence = {3}. TemplateItem = \"{4}\". Grammem = {5}.",
				Text,
				Lemma,
				PartOfSpeech,
				PartOfSentence,
				TemplateItem,
				Grammem);
		}
		#endregion
	}
}
