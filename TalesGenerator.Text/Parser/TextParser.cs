using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace TalesGenerator.Text
{
	public class TextParser : IDisposable
	{
		#region Fields

		private readonly TextAnalyzer _textAnalyzer;
		#endregion

		#region Constructors

		public TextParser(TextAnalyzer textAnalyzer)
		{
			Contract.Requires<ArgumentNullException>(textAnalyzer != null);

			_textAnalyzer = textAnalyzer;
		}
		#endregion

		#region Methods

		private IEnumerable<SentenceToken> ParseSentence(string sentence)
		{
			List<SentenceToken> sentenceTokens = new List<SentenceToken>();
			string trimSentence = LexerUtils.TrimText(sentence);
			string[] words = trimSentence.Split(' ');

			foreach (string word in words)
			{
				LemmatizeResult result = _textAnalyzer.Lemmatize(word).FirstOrDefault();

				if (result != null)
				{
					sentenceTokens.Add(new SentenceToken(word, result.GetTextByFormId(0).ToLower()) { PartOfSpeech = result.GetPartOfSpeech() });
				}
			}

			return sentenceTokens;
		}

		public IEnumerable<IEnumerable<SentenceToken>> Parse(string text)
		{
			return Parse(text, Environment.NewLine);
		}

		public IEnumerable<IEnumerable<SentenceToken>> Parse(string text, string separator)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(text));
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(separator));
			Contract.Ensures(Contract.Result<IEnumerable<IEnumerable<SentenceToken>>>() != null);

			string[] sentences = text.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
			List<IEnumerable<SentenceToken>> textTokens = new List<IEnumerable<SentenceToken>>();

			foreach (string sentence in sentences)
			{
				textTokens.Add(ParseSentence(sentence));
			}

			return textTokens;
		}

		public void Dispose()
		{
			_textAnalyzer.Dispose();
		}
		#endregion
	}
}
