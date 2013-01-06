using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using TalesGenerator.Core.Plugins;
using TalesGenerator.Net;
using TalesGenerator.TaleNet;
using TalesGenerator.Text.Plugins;

namespace TalesGenerator.Text
{
	public class TemplateParser : ITemplateParser, IDisposable
	{
		#region Fields

		private const int DefaultTemplateBufferSize = 2048;

		private readonly TextAnalyzer _textAnalyzer;

		private readonly List<List<TemplateToken>> _sentenceContext = new List<List<TemplateToken>>();

		private List<TemplateToken> _currentSentence;

		private ITemplateParserContext _networkContext;
		#endregion

		#region Properties

		public IEnumerable<TemplateToken> CurrentSentence
		{
			get { return _currentSentence; }
		}

		public ITemplateParserContext NetworkContext
		{
			get { return _networkContext; }
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

		private TemplateParserPluginResult ParseTemplateItem(string templateItem)
		{
			TemplateParserPluginResult result = null;
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

		private TemplateParserResult ParseNode(FunctionNode functionNode)
		{
			string template = functionNode.Template;
			Lexer lexer = new Lexer(template);
			LexerResult lexerResult = null;
			StringBuilder stringBuilder = new StringBuilder(DefaultTemplateBufferSize);
			List<NetworkEdgeType> unresolvedContext = new List<NetworkEdgeType>();

			_currentSentence = new List<TemplateToken>();

			while (lexer.ReadNextToken(out lexerResult))
			{
				switch (lexerResult.Type)
				{
					case TokenType.Letter:
						stringBuilder.Append(lexerResult.Token);
						break;

					case TokenType.Colon:
						// TODO: Костыль.
						// В случае начала прямой речи, необходимо обновить контекст.
						stringBuilder.Append(lexerResult.Token);

						while (lexer.PeekNextToken(out lexerResult) &&
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
						Contract.Assume(_currentSentence != null);
						_sentenceContext.Add(_currentSentence);
						_currentSentence = new List<TemplateToken>();
						stringBuilder.Append(lexerResult.Token);
						break;

					case TokenType.LeftBrace:
						StringBuilder templateBuilder = new StringBuilder(128);
						bool isOk = false;

						while (lexer.ReadNextToken(out lexerResult))
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

						TemplateParserPluginResult parserResult = ParseTemplateItem(templateBuilder.ToString());

						if (parserResult.Text == null)
						{
							unresolvedContext.AddRange(parserResult.UnresolvedContext);
						}
						else
						{
							_currentSentence.AddRange(parserResult.TemplateTokens);
							stringBuilder.Append(parserResult.Text);
						}
						break;
				}
			}

			if (_currentSentence != null &&
				_currentSentence.Count > 0)
			{
				_sentenceContext.Add(_currentSentence);
			}

			return new TemplateParserResult(stringBuilder.ToString(), _sentenceContext, unresolvedContext);
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

		public TemplateParserResult Parse(FunctionNode functionNode)
		{
			Contract.Requires<ArgumentNullException>(functionNode != null);
			Contract.Ensures(Contract.Result<TemplateParserResult>() != null);

			_networkContext = new TemplateParserNetworkNodeContext(functionNode);
			_currentSentence = null;
			_sentenceContext.Clear();

			return ParseNode(functionNode);
		}

		public TemplateParserResult Parse(FunctionNode functionNode, ITemplateParserContext parserContext)
		{
			Contract.Requires<ArgumentNullException>(functionNode != null);
			Contract.Requires<ArgumentNullException>(parserContext != null);
			Contract.Ensures(Contract.Result<TemplateParserResult>() != null);

			_networkContext = parserContext;
			_currentSentence = null;
			_sentenceContext.Clear();

			return ParseNode(functionNode);
		}

		public void Dispose()
		{
			_textAnalyzer.Dispose();
		}
		#endregion
	}
}
