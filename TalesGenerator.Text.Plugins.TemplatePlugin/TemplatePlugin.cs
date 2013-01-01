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

		private readonly Regex _agentRegex = new Regex(@"^[Aa]gent(\d+)?(:\d+)?$");

		private readonly Regex _agentsRegex = new Regex(@"^[Aa]gents(:\d+)?$");

		private readonly Regex _recipientRegex = new Regex(@"^[Rr]ecipient(\d+)?(:\d+)?$");

		private readonly Regex _recipientsRegex = new Regex(@"^[Rr]ecipients(:\d+)?$");

		private readonly Regex _actionRegex = new Regex(@"^[Aa]ction(\d+)?(:\d+)?$");

		private readonly Regex _locativeRegex = new Regex(@"^[Ll]ocative(:\d+)?$");

		private readonly Dictionary<string, Grammem> _objectCasesDictionary = new Dictionary<string, Grammem>
		{
			{ "говорить", Grammem.Dativ },
			{ "сказать", Grammem.Dativ },
			{ "отвечать", Grammem.Dativ },
			{ "ответить", Grammem.Dativ },
			{ "видеть", Grammem.Genitiv },
			{ "увидеть", Grammem.Genitiv },
		};

		private ITemplateParser _templateParser;
		#endregion

		#region Properties

		public override string Name
		{
			get { return PluginName; }
		}
		#endregion

		#region Methods

		private string GetTextByGrammem(Match match, string word)
		{
			string result = null;

			Contract.Assume(_templateParser != null);

			// 3. Проверим на предмет явного задания граммемы.
			if (match.Groups.Count > 1)
			{
				string grammemText = null;

				if (match.Groups[1].Success &&
					match.Groups[1].Value.Contains(":"))
				{
					grammemText = match.Groups[1].Value.Substring(1);
				}
				else if (match.Groups[2].Success &&
						 match.Groups[2].Value.Contains(":"))
				{
					grammemText = match.Groups[2].Value.Substring(1);
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

		private Grammem FindObjectGrammem(TextAnalyzer textAnalyzer, string predicate)
		{
			Contract.Requires<ArgumentNullException>(textAnalyzer != null);
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(predicate));

			Grammem grammem = Grammem.None;
			string[] words = predicate.Split(' ');
			string verb = words.Length > 1 ? words[1] : words[0];
			LemmatizeResult result = textAnalyzer.Lemmatize(verb).FirstOrDefault();

			if (result != null)
			{
				_objectCasesDictionary.TryGetValue(result.GetTextByFormId(0).ToLower(), out grammem);
			}

			return grammem;
		}

		private TemplateToken ParseActorTemplate(string template, Match match, IList<NetworkEdgeType> unresolvedContext, NetworkEdgeType edgeType)
		{
			Contract.Requires<ArgumentException>(edgeType == NetworkEdgeType.Agent || edgeType == NetworkEdgeType.Recipient);
			Contract.Ensures(Contract.Result<TemplateToken>() != null);
			Contract.Assume(_templateParser != null);

			var actorNodes = _templateParser.NetworkContext[edgeType];

			if (!actorNodes.Any())
			{
				unresolvedContext.Add(edgeType);
				return null;
			}

			NetworkNode actorNode = GetContextNodeByIndex(match, 1, actorNodes);

			return ParseActorTemplate(match, actorNode.Name, actorNode, edgeType);
		}

		// TODO: Необходимо учитывать, что в actorName может быть несколько слов.
		private TemplateToken ParseActorTemplate(Match match, string actorName, NetworkNode actorNode, NetworkEdgeType edgeType, bool ignoreCase = false)
		{
			Contract.Requires<ArgumentException>(edgeType == NetworkEdgeType.Agent || edgeType == NetworkEdgeType.Recipient);
			Contract.Ensures(Contract.Result<TemplateToken>() != null);
			Contract.Assume(_templateParser != null);

			string lemma = Lemmatize(_templateParser.TextAnalyzer, actorName);
			string resolvedText = null;
			PartOfSentence partOfSentence = PartOfSentence.Subject;

			// 1. Проверим на предмет явного задания граммемы.
			resolvedText = GetTextByGrammem(match, actorName);

			// 2. Выполним поиск сказуемого.
			TemplateToken predicate = _templateParser.CurrentSentence.LastOrDefault(token => token.PartOfSentence == PartOfSentence.Predicate);

			if (predicate == null || edgeType == NetworkEdgeType.Agent)
			{
				partOfSentence = PartOfSentence.Subject;

				if (resolvedText == null)
				{
					resolvedText = actorName;
				}
			}
			else if (edgeType == NetworkEdgeType.Recipient)
			{
				Grammem objectGrammem = FindObjectGrammem(_templateParser.TextAnalyzer, predicate.Text);

				if (objectGrammem == Grammem.None)
				{
					objectGrammem = Grammem.Genitiv;
				}

				partOfSentence = PartOfSentence.Object;

				if (resolvedText == null)
				{
					resolvedText = _templateParser.ReconcileWord(actorName, objectGrammem);
				}
			}

			// 3. Согласуем регистр.
			if (!ignoreCase)
			{
				resolvedText = ReconcileCase(match.Groups[0].Value, resolvedText);
			}

			// 4. Запомним граммему.
			Grammem grammem = Grammem.None;
			TaleNet.IFormattable formattable = actorNode as TaleNet.IFormattable;

			if (formattable != null)
			{
				grammem = formattable.Grammem;
			}

			return
				new TemplateToken(resolvedText, lemma, match.ToString())
				{
					PartOfSentence = partOfSentence,
					Grammem = grammem
				};
		}

		private ParserResult ParseActorsTemplate(string template, Match match, IList<NetworkEdgeType> unresolvedContext, NetworkEdgeType edgeType)
		{
			Contract.Requires<ArgumentException>(edgeType == NetworkEdgeType.Agent || edgeType == NetworkEdgeType.Recipient);
			Contract.Assume(_templateParser != null);

			List<TemplateToken> templateTokens = new List<TemplateToken>();
			var actorNodes = _templateParser.NetworkContext[edgeType];

			if (!actorNodes.Any())
			{
				unresolvedContext.Add(edgeType);
				return null;
			}

			StringBuilder stringBuilder = new StringBuilder();
			TemplateToken actorToken = ParseActorTemplate(match, actorNodes.First().Name, actorNodes.First(), edgeType);

			templateTokens.Add(actorToken);
			stringBuilder.Append(actorToken.Text);

			foreach (var actorNode in actorNodes.Skip(1))
			{
				actorToken = ParseActorTemplate(match, actorNode.Name.ToLower(), actorNode, edgeType, true);

				templateTokens.Add(actorToken);
				stringBuilder.AppendFormat(", {0}", actorToken.Text);
			}

			return new ParserResult(stringBuilder.ToString(), templateTokens);
		}

		private TemplateToken ParseActionTemplate(string template, Match match, IList<NetworkEdgeType> unresolvedContext)
		{
			Contract.Assume(_templateParser != null);

			// TODO: Необходимо сделать шаблон более универсальным.
			// Сейчас предполагается, что формат строки action должен быть таким:
			// [не] verb {word}

			var actionNodes = _templateParser.NetworkContext[NetworkEdgeType.Action];

			if (!actionNodes.Any())
			{
				unresolvedContext.Add(NetworkEdgeType.Action);
				return null;
			}

			NetworkNode actionNode = GetContextNodeByIndex(match, 1, actionNodes);
			string resolvedText = null;
			string actionText = actionNode.Name;
			string lemma = Lemmatize(_templateParser.TextAnalyzer, actionText);
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
					TemplateToken subjectToken = _templateParser.CurrentSentence.First(token => token.PartOfSentence == PartOfSentence.Subject);

					if (subjectToken.Grammem != Grammem.None)
					{
						resolvedVerb = _templateParser.ReconcileWord(verb, subjectToken.Grammem);
					}
					else
					{
						resolvedVerb = _templateParser.ReconcileWord(verb, subjectToken.Text);
					}
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

			return
				new TemplateToken(resolvedText, lemma, match.ToString())
				{
					PartOfSpeech = PartOfSpeech.VERB,
					PartOfSentence = PartOfSentence.Predicate
				};
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

		public override TemplateParserPluginResult Parse(ITemplateParser templateParser, string template)
		{
			Contract.Requires<ArgumentNullException>(templateParser != null);
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(template));
			Contract.Ensures(Contract.Result<TemplateParserPluginResult>() != null);

			_templateParser = templateParser;

			string resolvedText = null;
			List<TemplateToken> templateTokens = new List<TemplateToken>();
			List<NetworkEdgeType> unresolvedContext = new List<NetworkEdgeType>();

			for (; ; )
			{
				Match match = _agentsRegex.Match(template);
				if (match.Success)
				{
					ParserResult parserResult = ParseActorsTemplate(template, match, unresolvedContext, NetworkEdgeType.Agent);

					resolvedText = parserResult.Text;
					templateTokens.AddRange(parserResult.TemplateTokens);
					break;
				}

				match = _agentRegex.Match(template);
				if (match.Success)
				{
					TemplateToken actorToken = ParseActorTemplate(template, match, unresolvedContext, NetworkEdgeType.Agent);

					resolvedText = actorToken.Text;
					templateTokens.Add(actorToken);
					break;
				}

				match = _recipientsRegex.Match(template);
				if (match.Success)
				{
					ParserResult parserResult = ParseActorsTemplate(template, match, unresolvedContext, NetworkEdgeType.Recipient);

					resolvedText = parserResult.Text;
					templateTokens.AddRange(parserResult.TemplateTokens);
					break;
				}

				match = _recipientRegex.Match(template);
				if (match.Success)
				{
					TemplateToken recipientToken = ParseActorTemplate(template, match, unresolvedContext, NetworkEdgeType.Recipient);

					resolvedText = recipientToken.Text;
					templateTokens.Add(recipientToken);
					break;
				}

				match = _actionRegex.Match(template);
				if (match.Success)
				{
					TemplateToken actionToken = ParseActionTemplate(template, match, unresolvedContext);

					resolvedText = actionToken.Text;
					templateTokens.Add(actionToken);
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

			return new TemplateParserPluginResult(resolvedText, templateTokens, unresolvedContext);
		}
		#endregion
	}
}
