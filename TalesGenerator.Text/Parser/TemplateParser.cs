using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TalesGenerator.Core.Plugins;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;
using TalesGenerator.Text.Plugins;
using System.Diagnostics.Contracts;

namespace TalesGenerator.Text
{
	public class TemplateParser : ITemplateParser
	{
		#region Fields

		private readonly TextAnalyzer _textAnalyzer;

		private readonly List<List<TemplateToken>> _sentenceContext = new List<List<TemplateToken>>();

		private List<TemplateToken> _currentSentence;

		private ITemplateParserContext _parserContext;
		#endregion

		#region Properties

		public IList<TemplateToken> CurrentSentence
		{
			get { return _currentSentence; }
		}

		public ITemplateParserContext ParserContext
		{
			get { return _parserContext; }
		}

		public TextAnalyzer TextAnalyzer
		{
			get { return _textAnalyzer; }
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

			// TODO: Необходимо продумать логику загрузки плагинов для TemplateParser.
			// Необходимо убедиться, в каком порядке загружаются плагины,
			// т.к. это влияет на порядок их применения.
			PluginManager.LoadPlugins<ITemplateParserPlugin>();
		}
		#endregion

		#region Methods

		private TemplateParserResult ParseTemplateItem(string templateItem)
		{
			TemplateParserResult result = null;
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

		private TemplateParserResult ParseNode(NetworkNode networkNode)
		{
			NetworkNode templateNode = networkNode.OutgoingEdges.GetEdge(NetworkEdgeType.Template, true).EndNode;
			string template = templateNode.Name;
			Lexer lexer = new Lexer(template);
			LexerResult lexerResult = null;
			StringBuilder stringBuilder = new StringBuilder(1024);
			List<NetworkEdgeType> unresolvedContext = new List<NetworkEdgeType>();

			_currentSentence = new List<TemplateToken>();

			while (lexer.GetNextToken(out lexerResult))
			{
				switch (lexerResult.Type)
				{
					case TokenType.Letter:
						stringBuilder.Append(lexerResult.Token);
						break;

					case TokenType.Colon:
						stringBuilder.Append(lexerResult.Token);

						while (lexer.GetNextToken(out lexerResult) &&
							lexerResult.Type == TokenType.Space)
						{
							stringBuilder.Append(lexerResult.Token);
						}

						if (lexerResult.Type == TokenType.Quotes)
						{
							Contract.Assume(_currentSentence != null);
							_sentenceContext.Add(_currentSentence);
							_currentSentence = new List<TemplateToken>();
							stringBuilder.Append(lexerResult.Token);
						}
						break;

					case TokenType.Space:
					case TokenType.Punctuation:
						stringBuilder.Append(lexerResult.Token);
						break;

					case TokenType.Point:
					case TokenType.EndOfStream:
						Contract.Assume(_currentSentence != null);
						_sentenceContext.Add(_currentSentence);
						_currentSentence = new List<TemplateToken>();
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
							throw new TemplateParserException();
						}

						TemplateParserResult parserResult = ParseTemplateItem(templateBuilder.ToString());

						if (parserResult.Text == null)
						{
							unresolvedContext.AddRange(parserResult.UnresolvedContext);
						}
						else
						{
							stringBuilder.Append(parserResult.Text);
						}
						break;
				}
			}

			return new TemplateParserResult(stringBuilder.ToString(), unresolvedContext);
		}

		public string ReconcileWord(string word, Grammem grammem)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(word));
			Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

			string resultWord = word;
			LemmatizeResult result = _textAnalyzer.Lemmatize(word).FirstOrDefault();

			if (result != null)
			{
				string text = result.GetTextByGrammem(grammem).FirstOrDefault();

				if (!string.IsNullOrEmpty(text))
				{
					resultWord = text.ToLower();
				}
			}

			return resultWord;
		}

		public string ReconcileWord(string word, string baseWord)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(word));
			Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

			LemmatizeResult result = _textAnalyzer.Lemmatize(baseWord).FirstOrDefault();

			if (result != null)
			{
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
			else
			{
				return word;
			}
		}

		public TemplateParserResult Parse(NetworkNode networkNode)
		{
			if (networkNode == null)
			{
				throw new ArgumentNullException("networkNode");
			}

			_parserContext = new TemplateParserNodeContext(networkNode);
			_currentSentence = null;
			_sentenceContext.Clear();

			return ParseNode(networkNode);
		}

		public TemplateParserResult Parse(NetworkNode networkNode, ITemplateParserContext parserContext)
		{
			if (networkNode == null)
			{
				throw new ArgumentNullException("networkNode");
			}
			if (parserContext == null)
			{
				throw new ArgumentNullException("parserContext");
			}

			_parserContext = parserContext;
			_currentSentence = null;
			_sentenceContext.Clear();

			return ParseNode(networkNode);
		}
		#endregion
	}
}
