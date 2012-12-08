using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TalesGenerator.Core;
using TalesGenerator.Core.Collections;
using TalesGenerator.Core.Plugins;
using TalesGenerator.Text.Plugins;

namespace TalesGenerator.Text
{
	internal class TemplateParser : ITemplateParser
	{
		#region Fields

		private readonly TextAnalyzer _textAnalyzer;

		private readonly List<TemplateToken> _context = new List<TemplateToken>();

		private NetworkNode _currentNode;
		#endregion

		#region Properties

		public NetworkNode CurrentNode
		{
			get { return _currentNode; }
		}

		public List<TemplateToken> Context
		{
			get { return _context; }
		}
		#endregion

		#region Constructors

		public TemplateParser(TextAnalyzer textAnalyzer)
		{
			if (textAnalyzer == null)
			{
				throw new ArgumentNullException("textAnalyzer");
			}

			_textAnalyzer = textAnalyzer;

			//TODO Не уверен, что этот вызов должен быть здесь.
			PluginManager.LoadPlugins<ITemplateParserPlugin>();
		}
		#endregion

		#region Methods

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

		public string ParseGrammemMatch(Match match, string value, string baseWord = null, bool ignoreCase = false)
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

		private string ParseTemplateItem(string templateItem)
		{
			string result = string.Empty;
			var parserPlugins = PluginManager.GetPlugins<ITemplateParserPlugin>();
			ITemplateParserPlugin parserPlugin = parserPlugins.FirstOrDefault(plugin => plugin.CanParse(templateItem));

			if (parserPlugin != null)
			{
				result = parserPlugin.Parse(this, templateItem);
			}
			else
			{
				throw new PluginNotFoundException("Template Parser Plugin");
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
						TemplateToken predicate = _context.FirstOrDefault(token => token.PartOfSentence == PartOfSentence.Predicate);

						//TODO Пока предполагается, что подлежащее указывается раньше сказуемого.
						//Поэтому следующее условие говорит о том, что слово - подлежащее.
						if (predicate == null)
						{
							_context.Add(new TemplateToken(word, word, PartOfSentence.Subject));
							//TODO Возможно, необходимо переводить слово в именительный падеж.
							//text = word;
						}
						else
						{
							_context.Add(new TemplateToken(word, word, PartOfSentence.Object));
							//var texts = result.GetTextByGrammem(Grammem.Accusativ);
							//text = texts.FirstOrDefault();
						}

						break;
					}

					case PartOfSpeech.VERB:
					case PartOfSpeech.INFINITIVE:
					{
						_context.Add(new TemplateToken(word, word, PartOfSentence.Predicate));
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

		public string ReconcileWord(string word, Grammem grammem)
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

		public string ReconcileWord(string word, string baseWord)
		{
			var results = _textAnalyzer.Lemmatize(baseWord);
			LemmatizeResult result = results.FirstOrDefault();

			if (result == null)
			{
				throw new InvalidOperationException();
			}

			//TODO В случае если baseGrammem == 0, необходимо брать граммему из контекста.
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
			LexerResult lexerResult = null;
			StringBuilder stringBuilder = new StringBuilder(1024);

			while (lexer.GetNextToken(out lexerResult))
			{
				switch (lexerResult.Type)
				{
					case TokenType.Word:
						stringBuilder.Append(lexerResult.Token);
						break;

					case TokenType.Space:
					case TokenType.Punctuation:
						stringBuilder.Append(lexerResult.Token);
						break;

					case TokenType.LeftBrace:
						StringBuilder templateBuilder = new StringBuilder(128);
						bool isOk = false;

						while (lexer.GetNextToken(out lexerResult))
						{
							if (lexerResult.Type == TokenType.RightBrace)
							{
								isOk = true;
								break;
							}

							templateBuilder.Append(lexerResult.Token);
						}

						if (!isOk)
						{
							throw new InvalidOperationException();
						}

						stringBuilder.Append(ParseTemplateItem(templateBuilder.ToString()));
						break;
				}
			}

			return stringBuilder.ToString();
		}
		#endregion
	}
}
