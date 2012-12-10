using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;

namespace TalesGenerator.Text.Plugins.TemplatePluginEx
{
	public class TemplatePluginEx : TemplateParserPlugin
	{
		#region Fields

		private const string PluginName = "Advanced Template Parser Plugin";

		private readonly Regex _wordRegex = new Regex(@"^~(\w+):(\w+)$");
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

		public override TemplateParserResult Parse(ITemplateParser templateParser, string template)
		{
			if (templateParser == null)
			{
				throw new ArgumentNullException("templateParser");
			}
			if (string.IsNullOrEmpty(template))
			{
				throw new ArgumentException("template");
			}

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
						var agentNodes = templateParser.ParserContext[NetworkEdgeType.Agent];

						if (!agentNodes.Any())
						{
							unresolvedContext.Add(NetworkEdgeType.Agent);
							break;
						}

						wordSample = agentNodes.First().Name;
						break;

					case "recipient":
						var recipientNodes = templateParser.ParserContext[NetworkEdgeType.Recipient];

						if (!recipientNodes.Any())
						{
							unresolvedContext.Add(NetworkEdgeType.Recipient);
							break;
						}

						wordSample = recipientNodes.First().Name;
						break;

					case "action":
						var actionNodes = templateParser.ParserContext[NetworkEdgeType.Action];

						if (!actionNodes.Any())
						{
							unresolvedContext.Add(NetworkEdgeType.Action);
							break;
						}

						wordSample = actionNodes.First().Name;
						break;

					default:
						throw new NotSupportedException();
				}

				if (!string.IsNullOrEmpty(wordSample))
				{
					resolvedText = templateParser.ReconcileWord(match.Groups[1].Value.Trim('~', ':'), wordSample);
				}
			}

			return new TemplateParserResult(resolvedText, unresolvedContext);
		}
		#endregion
	}
}
