using System;

namespace Test
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

	public class Token
	{
		#region Properties

		public PartOfSentence PartOfSentence { get; private set; }

		public string TemplateItem { get; private set; }

		public string Text { get; private set; }
		#endregion

		#region Constructors

		public Token(string templateItem, string text, PartOfSentence partOfSentence)
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

		#region MyRegion

		public override string ToString()
		{
			return string.Format("TemplateItem = {0}. Text = {1}. PartOfSentence = {2}.", TemplateItem, Text, PartOfSentence);
		}
		#endregion
	}
}
