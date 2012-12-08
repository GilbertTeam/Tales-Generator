using System;
using System.Linq;
using System.Text;
using TalesGenerator.Core.Collections;
using System.Text.RegularExpressions;
using TalesGenerator.Core;
using TalesGenerator.Text;
using TalesGenerator.Text.Plugins;
using System.Collections.Generic;

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

			string result = null;

			for (; ; )
			{
				Match match = _agentsRegex.Match(template);
				if (match.Success)
				{
					var agents = templateParser.CurrentNode.OutgoingEdges.GetEdges(NetworkEdgeType.Agent).Select(edge => edge.EndNode.Name);
					StringBuilder stringBuilder = new StringBuilder();

					if (agents.Count() == 0)
					{
						throw new InvalidOperationException();
					}

					stringBuilder.Append(ParseAgentTemplateItem(templateParser, match, agents.First()));

					foreach (var agent in agents.Skip(1))
					{
						stringBuilder.AppendFormat(", {0}", ParseAgentTemplateItem(templateParser, match, agent.ToLower(), true));
					}

					result = stringBuilder.ToString();
					break;
				}

				match = _agentRegex.Match(template);
				if (match.Success)
				{
					result = ParseAgentTemplateItem(templateParser, match, templateParser.CurrentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Agent).EndNode.Name);
					break;
				}

				match = _recipientsRegex.Match(template);
				if (match.Success)
				{
					var recipients = templateParser.CurrentNode.OutgoingEdges.GetEdges(NetworkEdgeType.Recipient).Select(edge => edge.EndNode.Name);
					StringBuilder stringBuilder = new StringBuilder();

					if (recipients.Count() == 0)
					{
						throw new InvalidOperationException();
					}

					stringBuilder.Append(ParseRecipientTemplateItem(templateParser, match, recipients.First()));

					foreach (var recipient in recipients.Skip(1))
					{
						stringBuilder.AppendFormat(", {0}", ParseRecipientTemplateItem(templateParser, match, recipient.ToLower(), true));
					}

					result = stringBuilder.ToString();
					break;
				}

				match = _recipientRegex.Match(template);
				if (match.Success)
				{
					result = ParseRecipientTemplateItem(templateParser, match, templateParser.CurrentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Recipient).EndNode.Name);
					break;
				}

				match = _actionRegex.Match(template);
				if (match.Success)
				{
					//TODO В случае наличия нескольких подлежащих сказуемое должно быть во множественном числе.
					TemplateToken subject = templateParser.Context.LastOrDefault(context => context.PartOfSentence == PartOfSentence.Subject);
					string action = templateParser.CurrentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Action, true).EndNode.Name;
					string[] words = action.Split(' ');
					result = templateParser.ParseGrammemMatch(match, words[0], subject != null ? subject.Text : null);

					templateParser.Context.Add(new TemplateToken(match.ToString(), result, PartOfSentence.Predicate));

					foreach (string word in words.Skip(1))
					{
						result += " " + word;
					}
					break;
				}

				break;
			}

			return result;
		}
		#endregion

		
	}
}
