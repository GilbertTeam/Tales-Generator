using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TalesGenerator.Net;

namespace TalesGenerator.Text
{
	public class TemplateParserResult : TemplateParserBaseResult
	{
		#region Properties

		/// <summary>
		/// Набор токенов, получившихся в результате разбора шаблона.
		/// </summary>
		public IEnumerable<IEnumerable<TemplateToken>> TemplateTokens { get; private set; }
		#endregion

		#region Constructors

		public TemplateParserResult(string text, IEnumerable<IEnumerable<TemplateToken>> templateTokens)
			: base(text)
		{
			Contract.Requires<ArgumentNullException>(templateTokens != null);

			TemplateTokens = templateTokens;
		}

		public TemplateParserResult(string text, IEnumerable<IEnumerable<TemplateToken>> templateTokens, IEnumerable<NetworkEdgeType> unresolvedContext)
			: base(text, unresolvedContext)
		{
			Contract.Requires<ArgumentNullException>(templateTokens != null);

			TemplateTokens = templateTokens;
		}
		#endregion
	}
}
