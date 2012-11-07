using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TalesGenerator.Core;
using TalesGenerator.Text;
using TalesGenerator.Core.Collections;

namespace Test
{
	public class Parser
	{
		#region Fields

		private readonly TextAnalyzer _textAnalyzer;

		private readonly List<Token> _context = new List<Token>();

		private NetworkNode _currentNode;
		#endregion

		#region Constructors

		public Parser(TextAnalyzer textAnalyzer)
		{
			if (textAnalyzer == null)
			{
				throw new ArgumentNullException("textAnalyzer");
			}

			_textAnalyzer = textAnalyzer;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Изменяет форму слова в соответствии с указанной граммемой.
		/// </summary>
		/// <param name="word">Слово, форму которого необходимо изменить.</param>
		/// <param name="grammem">Граммема, в соответствии с которой необходимо изменить форму слова.</param>
		/// <returns>Согласованная форма слова.</returns>
		private string ReconcileWord(string word, Grammem grammem)
		{
			var results = _textAnalyzer.Lemmatize(word);
			LemmatizeResult result = results.FirstOrDefault();

			if (result == null)
			{
				throw new InvalidOperationException();
			}

			string text = result.GetTextByGrammem(grammem).FirstOrDefault();

			if (string.IsNullOrEmpty(text))
			{
				return word;
			}
			else
			{
				return text.ToLower();
			}
		}

		/// <summary>
		/// Изменяет (согласует) форму слова, в соответствии с формой другого.
		/// </summary>
		/// <param name="word">Слово, форму которого необходимо согласовать.</param>
		/// <param name="baseWord">Слово, в соответствии с формой которого будет выполнено согласование.</param>
		/// <returns>Согласованная форма слова.</returns>
		private string ReconcileWord(string word, string baseWord)
		{
			var results = _textAnalyzer.Lemmatize(baseWord);
			LemmatizeResult result = results.FirstOrDefault();

			if (result == null)
			{
				throw new InvalidOperationException();
			}

			//TODO В случае если parentGrammem == 0, необходимо брать граммему из контекста.
			Grammem baseGrammem = result.GetGrammem();
			Grammem resultGrammem =
				  baseGrammem & Grammem.Plural
				| baseGrammem & Grammem.Singular
				| baseGrammem & Grammem.Masculinum
				| baseGrammem & Grammem.Feminum
				| baseGrammem & Grammem.Neutrum
				| baseGrammem & Grammem.MascFem;

			return ReconcileWord(word, resultGrammem);
		}

		private string ParseGrammemTemplate(string value, string grammem)
		{
			var results = _textAnalyzer.Lemmatize(value);
			LemmatizeResult result = results.FirstOrDefault();

			if (result == null)
			{
				throw new InvalidOperationException();
			}

			ulong grammemValue = ulong.Parse(grammem);

			var texts = result.GetTextByGrammem((Grammem)grammemValue);

			//TODO Возможно нужен exception в случае, если коллекция texts - пустая.
			return texts.FirstOrDefault();
		}

		private string ParseGrammemMatch(Match match, string value, string baseWord = null, bool ignoreCase = false)
		{
			string text = string.Empty;

			if (match.Groups[1].Success)
			{
				text = ParseGrammemTemplate(value, match.Groups[1].Value.Substring(1));
			}
			else if (baseWord != null)
			{
				text = ReconcileWord(value, baseWord);
			}
			else
			{
				text = value;
			}

			if (!ignoreCase)
			{
				if (char.IsUpper(match.Groups[0].Value, 0))
				{
					text = char.ToUpper(text[0]) + text.Substring(1);
				}
				else
				{
					text = char.ToLower(text[0]) + text.Substring(1);
				}
			}

			return text;
		}

		private string ParseAgentTemplateItem(Match match, string value, bool ignoreCase = false)
		{
			//TODO Пока рассматриваются предложения только с одним предикативным центром.
			Token predicate = _context.SingleOrDefault(context => context.PartOfSentence == PartOfSentence.Predicate);
			string text = ParseGrammemMatch(match, value, predicate != null ? predicate.Text : null, ignoreCase);

			_context.Add(new Token(match.ToString(), text, predicate != null ? PartOfSentence.Object : PartOfSentence.Subject));

			return text;
		}

		private string ParseRecipientTemplateItem(Match match, string value, bool ignoreCase = false)
		{
			string text = string.Empty;

			//TODO Пока рассматриваются предложения только с одним предикативным центром.
			Token predicate = _context.SingleOrDefault(context => context.PartOfSentence == PartOfSentence.Predicate);

			if (predicate != null)
			{
				text = ReconcileWord(value, Grammem.Accusativ);
			}
			else
			{
				text = ParseGrammemMatch(match, value, predicate != null ? predicate.Text : null, ignoreCase);
			}

			_context.Add(new Token(match.ToString(), text, predicate != null ? PartOfSentence.Object : PartOfSentence.Subject));

			return text;
		}

		private string ParseTemplateItem(string templateItem)
		{
			string result = string.Empty;

			for (; ; )
			{
				Match match = Regex.Match(templateItem, @"~(\w+):(\w+)");
				if (match.Success)
				{
					string wordSample = match.Groups[2].Value.ToLower();

					switch (wordSample)
					{
						case "agent":
							wordSample = _currentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Agent).EndNode.Name;
							break;

						case "recipient":
							wordSample = _currentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Recipient).EndNode.Name;
							break;

						case "action":
							wordSample = _currentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Action).EndNode.Name;
							break;

						default:
							throw new InvalidOperationException();
					}

					result = ReconcileWord(match.Groups[1].Value.Trim('~', ':'), wordSample);
					break;
				}

				match = Regex.Match(templateItem, @"[Aa]gents(:\d+)?");
				if (match.Success)
				{
					var agents = _currentNode.OutgoingEdges.GetEdges(NetworkEdgeType.Agent).Select(edge => edge.EndNode.Name);
					StringBuilder stringBuilder = new StringBuilder();

					if (agents.Count() == 0)
					{
						throw new InvalidOperationException();
					}

					stringBuilder.Append(ParseAgentTemplateItem(match, agents.First()));

					foreach (var agent in agents.Skip(1))
					{
						stringBuilder.AppendFormat(", {0}", ParseAgentTemplateItem(match, agent.ToLower(), true));
					}

					result = stringBuilder.ToString();
					break;
				}

				match = Regex.Match(templateItem, @"[Aa]gent(:\d+)?");
				if (match.Success)
				{
					result = ParseAgentTemplateItem(match, _currentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Agent).EndNode.Name);
					break;
				}

				match = Regex.Match(templateItem, @"[Rr]ecipients(:\d+)?");
				if (match.Success)
				{
					var recipients = _currentNode.OutgoingEdges.GetEdges(NetworkEdgeType.Recipient).Select(edge => edge.EndNode.Name);
					StringBuilder stringBuilder = new StringBuilder();

					if (recipients.Count() == 0)
					{
						throw new InvalidOperationException();
					}

					stringBuilder.Append(ParseRecipientTemplateItem(match, recipients.First()));

					foreach (var recipient in recipients.Skip(1))
					{
						stringBuilder.AppendFormat(", {0}", ParseRecipientTemplateItem(match, recipient.ToLower(), true));
					}

					result = stringBuilder.ToString();
					break;
				}

				match = Regex.Match(templateItem, @"[Rr]ecipient(:\d+)?");
				if (match.Success)
				{
					result = ParseRecipientTemplateItem(match, _currentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Recipient).EndNode.Name);
					break;
				}

				match = Regex.Match(templateItem, @"[Aa]ction(:\d+)?");
				if (match.Success)
				{
					//TODO В случае наличия нескольких подлежащих сказуемое должно быть во множественном числе.
					Token subject = _context.LastOrDefault(context => context.PartOfSentence == PartOfSentence.Subject);
					string action = _currentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Action, true).EndNode.Name;
					string[] words = action.Split(' ');
					result = ParseGrammemMatch(match, words[0], subject != null ? subject.Text : null);

					_context.Add(new Token(match.ToString(), result, PartOfSentence.Predicate));

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

		private string ParseWord(string word)
		{
			string text = word;
			var results = _textAnalyzer.Lemmatize(word);
			LemmatizeResult result = results.FirstOrDefault();

			if (result != null)
			{
				PartOfSpeech partOfSpeech = result.GetPartOfSpeech();

				switch (partOfSpeech)
				{
					case PartOfSpeech.NOUN:
					{
						Token predicate = _context.FirstOrDefault(token => token.PartOfSentence == PartOfSentence.Predicate);

						//TODO Пока предполагается, что подлежащее указывается раньше сказуемого.
						//Поэтому следующее условие говорит о том, что слово - подлежащее.
						if (predicate == null)
						{
							_context.Add(new Token(word, word, PartOfSentence.Subject));
							//TODO Возможно, необходимо переводить слово в именительный падеж.
							//text = word;
						}
						else
						{
							_context.Add(new Token(word, word, PartOfSentence.Object));
							//var texts = result.GetTextByGrammem(Grammem.Accusativ);
							//text = texts.FirstOrDefault();
						}

						break;
					}

					case PartOfSpeech.VERB:
					case PartOfSpeech.INFINITIVE:
					{
						_context.Add(new Token(word, word, PartOfSentence.Predicate));
						//Token subject = _context.FirstOrDefault(token => token.PartOfSentence == PartOfSentence.Subject);

						//if (subject != null)
						//{
						//    text = ReconcileWord(word, subject.Text);
						//}
						//else
						//{
						//    text = word;
						//}

						break;
					}

					default:
						//throw new NotImplementedException();
						text = word;
						break;
				}
			}

			if (string.IsNullOrEmpty(text))
			{
				text = word;
			}

			return text;
		}

		public string Parse(NetworkNode networkNode)
		{
			if (networkNode == null)
			{
				throw new ArgumentNullException("networkNode");
			}

			_currentNode = networkNode;
			_context.Clear();

			NetworkNode templateNode = _currentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Template, true).EndNode;
			string template = templateNode.Name;

			Lexer lexer = new Lexer(template);

			lexer.ParseTemplateItem += (templateItem) => { return ParseTemplateItem(templateItem); };
			lexer.ParseWord += (word) => { return ParseWord(word); };

			return lexer.Parse();
		}
		#endregion
	}
}
