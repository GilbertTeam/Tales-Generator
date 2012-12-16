using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;
using TalesGenerator.Text;
using TalesGenerator.Text.Plugins;
using System.Diagnostics.Contracts;

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

		private readonly Regex _actionRegex = new Regex(@"^([Aa]ction)(\d+)?(:\d+)?$");

		private readonly Regex _locativeRegex = new Regex(@"^[Ll]ocative(:\d+)?$");

		private ITemplateParser _templateParser;
		#endregion

		#region Properties

		public override string Name
		{
			get { return PluginName; }
		}
		#endregion

		#region Methods

		private string GetWord(Match match, IEnumerable<NetworkNode> networkNodes)
		{
			Contract.Ensures(Contract.Result<string>() != null);

			string text = null;

			// Проверим, задан ли явно индекс.
			if (match.Groups.Count > 2 &&
				match.Groups[2].Success)
			{
				int index;

				if (int.TryParse(match.Groups[2].Value, out index))
				{
					Contract.Assume(networkNodes.Count() > index);

					text = networkNodes.ElementAt(index).Name;
				}
			}

			if (text == null)
			{
				text = networkNodes.First().Name;
			}

			return text;
		}

		private string GetTextByGrammem(Match match, string word)
		{
			string result = null;

			Contract.Assume(_templateParser != null);

			// 3. Проверим на предмет явного задания граммемы.
			if (match.Groups.Count > 2)
			{
				string grammemText = null;

				if (match.Groups[2].Success &&
					match.Groups[2].Value.Contains(":"))
				{
					grammemText = match.Groups[2].Value.Substring(1);
				}
				else if (match.Groups[3].Success &&
						 match.Groups[3].Value.Contains(":"))
				{
					grammemText = match.Groups[3].Value.Substring(1);
				}

				int grammem;

				if (int.TryParse(grammemText, out grammem))
				{
					result = _templateParser.ReconcileWord(word, (Grammem)grammem);
				}
			}

			return result;
		}

		private string ReconcileCase(string pattern, string word)
		{
			Contract.Assume(pattern.Length > 0);
			Contract.Assume(word.Length > 0);

			if (char.IsUpper(pattern[0]))
			{
				return char.ToUpper(word[0]) + word.Substring(1);
			}
			else
			{
				return char.ToLower(word[0]) + word.Substring(1);
			}
		}

		private string ParseActorTemplate(string template, Match match, IList<NetworkEdgeType> unresolvedContext, NetworkEdgeType edgeType)
		{
			Contract.Ensures(edgeType == NetworkEdgeType.Agent || edgeType == NetworkEdgeType.Recipient);
			Contract.Assume(_templateParser != null);

			var actorNodes = _templateParser.ParserContext[edgeType];

			if (!actorNodes.Any())
			{
				unresolvedContext.Add(edgeType);
				return null;
			}

			string actorName = GetWord(match, actorNodes);

			return ParseActorTemplate(match, actorName);
		}

		private string ParseActorTemplate(Match match, string actorName, bool ignoreCase = false)
		{
			Contract.Assume(_templateParser != null);

			string resolvedText = null;
			PartOfSentence partOfSentence = PartOfSentence.Subject;

			// 1. Проверим на предмет явного задания граммемы.
			resolvedText = GetTextByGrammem(match, actorName);

			if (resolvedText == null)
			{
				// 2. Выполним поиск сказуемого.
				TemplateToken predicate = _templateParser.CurrentSentence.LastOrDefault(token => token.PartOfSentence == PartOfSentence.Predicate);

				if (predicate == null)
				{
					partOfSentence = PartOfSentence.Subject;
					resolvedText = actorName;
				}
				else
				{
					partOfSentence = PartOfSentence.Object;
					resolvedText = _templateParser.ReconcileWord(actorName, predicate.Text);
				}
			}

			if (!ignoreCase)
			{
				resolvedText = ReconcileCase(match.Groups[0].Value, resolvedText);
			}

			_templateParser.CurrentSentence.Add(new TemplateToken(match.ToString(), resolvedText, partOfSentence));

			return resolvedText;
		}

		private string ParseActorsTemplate(string template, Match match, IList<NetworkEdgeType> unresolvedContext, NetworkEdgeType edgeType)
		{
			Contract.Ensures(edgeType == NetworkEdgeType.Agent || edgeType == NetworkEdgeType.Recipient);
			Contract.Assume(_templateParser != null);

			string resolvedText = null;
			var actorNodes = _templateParser.ParserContext[NetworkEdgeType.Agent];

			if (!actorNodes.Any())
			{
				unresolvedContext.Add(NetworkEdgeType.Agent);
				return null;
			}

			var actorNames = actorNodes.Select(node => node.Name);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(ParseActorTemplate(match, actorNames.First()));

			foreach (var actorName in actorNames.Skip(1))
			{
				stringBuilder.AppendFormat(", {0}", ParseActorTemplate(match, actorName.ToLower(), true));
			}

			resolvedText = stringBuilder.ToString();

			return resolvedText;
		}

		private string ParseActionTemplate(string template, Match match, IList<NetworkEdgeType> unresolvedContext)
		{
			Contract.Assume(_templateParser != null);

			string resolvedText = null;

			// TODO: Необходимо сделать шаблон более универсальным.
			// Сейчас предполагается, что формат строки action должен быть таким:
			// [не] verb {word}

			var actionNodes = _templateParser.ParserContext[NetworkEdgeType.Action];

			if (!actionNodes.Any())
			{
				unresolvedContext.Add(NetworkEdgeType.Action);
				return null;
			}

			string actionText = GetWord(match, actionNodes);
			string[] words = actionText.Split(' ');
			string verb = null;
			string resolvedVerb = null;
			int verbIndex = 0;

			Contract.Assume(words.Length > 0);

			// 1. Определим наличие отрицания.
			if (string.Equals(words[0], "не", StringComparison.OrdinalIgnoreCase))
			{
				verbIndex = 1;
			}

			// 2. Проверим часть речи.
			Contract.Assume(words.Count() > verbIndex);

			verb = words[verbIndex];
			// TODO: Не всегда возможно проверить часть речи. Например, для "жили-были".
			//LemmatizeResult result = _templateParser.TextAnalyzer.Lemmatize(words[verbIndex]).FirstOrDefault();
			//Contract.Assume(result != null && result.GetPartOfSpeech() == PartOfSpeech.VERB);

			// 3. Проверим на предмет явного задания граммемы.
			resolvedVerb = GetTextByGrammem(match, verb);

			if (resolvedVerb == null)
			{
				// 4. Определим число подлежащих в текущем предложении.
				int subjectCount = _templateParser.CurrentSentence.Count(context => context.PartOfSentence == PartOfSentence.Subject);

				if (subjectCount == 0)
				{
					resolvedVerb = verb;
				}
				else if (subjectCount == 1)
				{
					string subject = _templateParser.CurrentSentence.First(token => token.PartOfSentence == PartOfSentence.Subject).Text;
					resolvedVerb = _templateParser.ReconcileWord(verb, subject);
				}
				else
				{
					resolvedVerb = _templateParser.ReconcileWord(verb, Grammem.Plural);
				}
			}

			// 5. Сформируем словосочетание.
			StringBuilder stringBuilder = new StringBuilder(template.Length);

			for (int i = 0; i < words.Length; i++)
			{
				string word = i == verbIndex ? resolvedVerb : words[i];

				if (i == 0)
				{
					word = ReconcileCase(match.Groups[0].Value, word);

					stringBuilder.AppendFormat("{0}", word);
				}
				else
				{
					stringBuilder.AppendFormat(" {0}", word);
				}
			}

			resolvedText = stringBuilder.ToString();

			_templateParser.CurrentSentence.Add(new TemplateToken(match.ToString(), resolvedText, PartOfSentence.Predicate));

			return resolvedText;
		}

		protected override IEnumerable<Regex> GetAvailableRegexes()
		{
			return new List<Regex>
			{
				_agentRegex,
				_agentsRegex,
				_recipientRegex,
				_recipientsRegex,
				_actionRegex,
				_locativeRegex
			};
		}

		public override TemplateParserResult Parse(ITemplateParser templateParser, string template)
		{
			Contract.Requires<ArgumentNullException>(templateParser != null);
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(template));

			_templateParser = templateParser;

			string resolvedText = null;
			List<NetworkEdgeType> unresolvedContext = new List<NetworkEdgeType>();

			for (; ; )
			{
				Match match = _agentsRegex.Match(template);
				if (match.Success)
				{
					resolvedText = ParseActorsTemplate(template, match, unresolvedContext, NetworkEdgeType.Agent);
					break;
				}

				match = _agentRegex.Match(template);
				if (match.Success)
				{
					resolvedText = ParseActorTemplate(template, match, unresolvedContext, NetworkEdgeType.Agent);
					break;
				}

				match = _recipientsRegex.Match(template);
				if (match.Success)
				{
					resolvedText = ParseActorsTemplate(template, match, unresolvedContext, NetworkEdgeType.Recipient);
					break;
				}

				match = _recipientRegex.Match(template);
				if (match.Success)
				{
					resolvedText = ParseActorTemplate(template, match, unresolvedContext, NetworkEdgeType.Recipient);
					break;
				}

				match = _actionRegex.Match(template);
				if (match.Success)
				{
					resolvedText = ParseActionTemplate(template, match, unresolvedContext);
					break;
				}

				match = _locativeRegex.Match(template);
				if (match.Success)
				{
					throw new NotImplementedException();
				}

				break;
			}

			_templateParser = null;

			return new TemplateParserResult(resolvedText, unresolvedContext);
		}
		#endregion
	}
}
