using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TalesGenerator.Core;
using TalesGenerator.Core.Collections;

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

		public override string Parse(ITemplateParser templateParser, string template)
		{
			if (templateParser == null)
			{
				throw new ArgumentNullException("templateParser");
			}
			if (string.IsNullOrEmpty(template))
			{
				throw new ArgumentException("template");
			}

			string result = string.Empty;
			Match match = _wordRegex.Match(template);

			if (match.Success)
			{
				string wordSample = match.Groups[2].Value.ToLower();

				switch (wordSample)
				{
					case "agent":
						wordSample = templateParser.CurrentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Agent).EndNode.Name;
						break;

					case "recipient":
						wordSample = templateParser.CurrentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Recipient).EndNode.Name;
						break;

					case "action":
						wordSample = templateParser.CurrentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Action).EndNode.Name;
						break;

					default:
						throw new InvalidOperationException();
				}

				result = templateParser.ReconcileWord(match.Groups[1].Value.Trim('~', ':'), wordSample);
			}

			return result;
		}
		#endregion
	}
}
