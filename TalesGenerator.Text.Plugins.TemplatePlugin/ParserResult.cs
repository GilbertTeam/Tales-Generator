using System.Collections.Generic;

namespace TalesGenerator.Text.Plugins
{
	internal class ParserResult
	{
		#region Properties

		public string Text { get; private set; }

		public IEnumerable<TemplateToken> TemplateTokens { get; private set; }
		#endregion

		#region Constructors

		public ParserResult(string text, IEnumerable<TemplateToken> templateTokens)
		{
			Text = text;
			TemplateTokens = templateTokens;
		}
		#endregion
	}
}
