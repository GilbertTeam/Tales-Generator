using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;
using TalesGenerator.Text;
using TalesGenerator.Text.Plugins;

namespace TalesGenerator.Plugin
{
	public class TemplatePlugin : TemplateParserPlugin
	{
		#region Fields

		private const string PluginName = "Template Parser Plugin";

		private readonly Regex _agentRegex = new Regex(@"^[Aa]gent(:\d+)?$");

		private readonly Regex _agentsRegex = new Regex(@"^[Aa]gents(:\d+)?$");

		private readonly Regex _recipientRegex = new Regex(@"^[Rr]ecipient(:\d+)?$");

		private readonly Regex _recipientsRegex = new Regex(@"^[Rr]ecipients(:\d+)?$");

		private readonly Regex _actionRegex = new Regex(@"^[Aa]ction(:\d+)?$");
		#endregion

		#region Properties

		public override string Name
		{
			get { return PluginName; }
		}
		#endregion

		#region Methods

		private string ParseAgentTemplateItem(ITemplateParser templateParser, Match match, string value, bool ignoreCase = false)
		{
			//TODO Пока рассматриваются предложения только с одним предикативным центром.
			TemplateToken predicate = templateParser.Context.SingleOrDefault(context => context.PartOfSentence == PartOfSentence.Predicate);
			string text = templateParser.ParseGrammemMatch(match, value, predicate != null ? predicate.Text : null, ignoreCase);

			templateParser.Context.Add(new TemplateToken(match.ToString(), text, predicate != null ? PartOfSentence.Object : PartOfSentence.Subject));

			return text;
		}

		private string ParseRecipientTemplateItem(ITemplateParser templateParser, Match match, string value, bool ignoreCase = false)
		{
			string text = string.Empty;

			//TODO Пока рассматриваются предложения только с одним предикативным центром.
			TemplateToken predicate = templateParser.Context.SingleOrDefault(context => context.PartOfSentence == PartOfSentence.Predicate);

			if (predicate != null)
			{
				text = templateParser.ReconcileWord(value, Grammem.Accusativ);
			}
			else
			{
				text = templateParser.ParseGrammemMatch(match, value, predicate != null ? predicate.Text : null, ignoreCase);
			}

			templateParser.Context.Add(new TemplateToken(match.ToString(), text, predicate != null ? PartOfSentence.Object : PartOfSentence.Subject));

			return text;
		}

		protected override IEnumerable<Regex> GetAvailableRegexes()
		{
			return new List<Regex>
			{
				_agentRegex,
				_agentsRegex,
				_recipientRegex,
				_recipientsRegex,
				_actionRegex
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

			string resolvedText = null;
			List<NetworkEdgeType> unresolvedContext = new List<NetworkEdgeType>();

			for (; ; )
			{
				Match match = _agentsRegex.Match(template);
				if (match.Success)
				{
					var agentNodes = templateParser.ParserContext[NetworkEdgeType.Agent];

					if (!agentNodes.Any())
					{
						unresolvedContext.Add(NetworkEdgeType.Agent);
						break;
					}

					var agentNames = agentNodes.Select(node => node.Name);
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(ParseAgentTemplateItem(templateParser, match, agentNames.First()));

					foreach (var agentName in agentNames.Skip(1))
					{
						stringBuilder.AppendFormat(", {0}", ParseAgentTemplateItem(templateParser, match, agentName.ToLower(), true));
					}

					resolvedText = stringBuilder.ToString();
					break;
				}

				match = _agentRegex.Match(template);
				if (match.Success)
				{
					var agentNodes = templateParser.ParserContext[NetworkEdgeType.Agent];

					if (!agentNodes.Any())
					{
						unresolvedContext.Add(NetworkEdgeType.Agent);
						break;
					}

					string agentName = agentNodes.First().Name;

					resolvedText = ParseAgentTemplateItem(templateParser, match, agentName);
					break;
				}

				match = _recipientsRegex.Match(template);
				if (match.Success)
				{
					var recipientNodes = templateParser.ParserContext[NetworkEdgeType.Recipient];

					if (!recipientNodes.Any())
					{
						unresolvedContext.Add(NetworkEdgeType.Recipient);
						break;
					}

					var recipientNames = recipientNodes.Select(node => node.Name);
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(ParseRecipientTemplateItem(templateParser, match, recipientNames.First()));

					foreach (var recipient in recipientNames.Skip(1))
					{
						stringBuilder.AppendFormat(", {0}", ParseRecipientTemplateItem(templateParser, match, recipient.ToLower(), true));
					}

					resolvedText = stringBuilder.ToString();
					break;
				}

				match = _recipientRegex.Match(template);
				if (match.Success)
				{
					var recipientNodes = templateParser.ParserContext[NetworkEdgeType.Recipient];

					if (!recipientNodes.Any())
					{
						unresolvedContext.Add(NetworkEdgeType.Recipient);
						break;
					}

					string recipientName = recipientNodes.First().Name;

					resolvedText = ParseRecipientTemplateItem(templateParser, match, recipientName);
					break;
				}

				match = _actionRegex.Match(template);
				if (match.Success)
				{
					var actionNodes = templateParser.ParserContext[NetworkEdgeType.Action];

					if (!actionNodes.Any())
					{
						unresolvedContext.Add(NetworkEdgeType.Action);
						break;
					}

					string action = actionNodes.First().Name;
					//TODO В случае наличия нескольких подлежащих сказуемое должно быть во множественном числе.
					TemplateToken subject = templateParser.Context.LastOrDefault(context => context.PartOfSentence == PartOfSentence.Subject);
					string[] words = action.Split(' ');
					resolvedText = templateParser.ParseGrammemMatch(match, words[0], subject != null ? subject.Text : null);

					templateParser.Context.Add(new TemplateToken(match.ToString(), resolvedText, PartOfSentence.Predicate));

					foreach (string word in words.Skip(1))
					{
						resolvedText += " " + word;
					}
					break;
				}

				break;
			}

			return new TemplateParserResult(resolvedText, unresolvedContext);
		}
		#endregion

		
	}
}
