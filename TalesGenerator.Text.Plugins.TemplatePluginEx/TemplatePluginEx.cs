using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using TalesGenerator.Net;

namespace TalesGenerator.Text.Plugins.TemplatePluginEx
{
	public class TemplatePluginEx : TemplateParserPlugin
	{
		#region Fields

		private const string PluginName = "Advanced Template Parser Plugin";

		private readonly Regex _wordRegex = new Regex(@"^~(\w+):([a-z]+)(\d+)?$");
		#endregion

		#region Properties

		public override string Name
		{
			get { return PluginName; }
		}
		#endregion

		#region Methods

		protected override IEnumerable<Regex> GetAvailableRegexes()
		{
			return new List<Regex>
			{
				_wordRegex
			};
		}

		public override TemplateParserPluginResult Parse(ITemplateParser templateParser, string template)
		{
			Contract.Requires<ArgumentNullException>(templateParser != null);
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(template));
			Contract.Ensures(Contract.Result<TemplateParserPluginResult>() != null);

			List<TemplateToken> templateTokens = new List<TemplateToken>();
			List<NetworkEdgeType> unresolvedContext = new List<NetworkEdgeType>();
			string resolvedText = null;
			Match match = _wordRegex.Match(template);

			if (match.Success)
			{
				string templateItemType = match.Groups[2].Value.ToLower();
				string wordSample = null;

				switch (templateItemType)
				{
					case "agent":
						var agentNodes = templateParser.NetworkContext[NetworkEdgeType.Agent];

						if (!agentNodes.Any())
						{
							unresolvedContext.Add(NetworkEdgeType.Agent);
							break;
						}

						wordSample = GetContextNodeByIndex(match, 3, agentNodes).Name;
						break;

					case "recipient":
						var recipientNodes = templateParser.NetworkContext[NetworkEdgeType.Recipient];

						if (!recipientNodes.Any())
						{
							unresolvedContext.Add(NetworkEdgeType.Recipient);
							break;
						}

						wordSample = GetContextNodeByIndex(match, 3, recipientNodes).Name;
						break;

					case "action":
						var actionNodes = templateParser.NetworkContext[NetworkEdgeType.Action];

						if (!actionNodes.Any())
						{
							unresolvedContext.Add(NetworkEdgeType.Action);
							break;
						}

						wordSample = GetContextNodeByIndex(match, 3, actionNodes).Name;
						break;

					default:
						throw new NotSupportedException();
				}

				if (!string.IsNullOrEmpty(wordSample))
				{
					resolvedText = templateParser.ReconcileWord(match.Groups[1].Value.Trim('~', ':'), wordSample);

					templateTokens.Add(new TemplateToken(resolvedText, Lemmatize(templateParser.TextAnalyzer, wordSample), match.ToString()));
				}
			}

			return new TemplateParserPluginResult(resolvedText, templateTokens, unresolvedContext);
		}
		#endregion
	}
}
