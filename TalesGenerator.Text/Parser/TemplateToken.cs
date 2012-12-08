using System;

namespace TalesGenerator.Text
{
	public enum PartOfSentence
	{
		/// <summary>
		/// Подлежащее
		/// </summary>
		Subject,

		/// <summary>
		/// Сказуемое
		/// </summary>
		Predicate,

		/// <summary>
		/// Дополнение
		/// </summary>
		Object
	}

	public class TemplateToken
	{
		#region Properties

		public PartOfSentence PartOfSentence { get; private set; }

		public string TemplateItem { get; private set; }

		public string Text { get; private set; }
		#endregion

		#region Constructors

		public TemplateToken(string templateItem, string text, PartOfSentence partOfSentence)
		{
			if (string.IsNullOrEmpty(templateItem))
			{
				throw new ArgumentNullException("templateItem");
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}

			TemplateItem = templateItem;
			Text = text;
			PartOfSentence = partOfSentence;
		}
		#endregion

		#region Methods

		public override string ToString()
		{
			return string.Format("TemplateItem = {0}. Text = \"{1}\". PartOfSentence = {2}.", TemplateItem, Text, PartOfSentence);
		}
		#endregion
	}
}
