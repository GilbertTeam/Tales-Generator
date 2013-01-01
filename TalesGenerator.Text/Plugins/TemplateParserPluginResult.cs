using System.Collections.Generic;
using TalesGenerator.Net;
using System.Diagnostics.Contracts;
using System;

namespace TalesGenerator.Text.Plugins
{
	public class TemplateParserPluginResult : TemplateParserBaseResult
	{
		#region Properties

		public IEnumerable<TemplateToken> TemplateTokens { get; private set; }
		#endregion

		#region Constructors

		public TemplateParserPluginResult(string text, IEnumerable<TemplateToken> templateTokens)
			: base(text)
		{
			Contract.Requires<ArgumentNullException>(templateTokens != null);

			TemplateTokens = templateTokens;
		}

		public TemplateParserPluginResult(string text, IEnumerable<TemplateToken> templateTokens, IEnumerable<NetworkEdgeType> unresolvedContext)
			: base(text, unresolvedContext)
		{
			Contract.Requires<ArgumentNullException>(templateTokens != null);

			TemplateTokens = templateTokens;
		}
		#endregion
	}
}
