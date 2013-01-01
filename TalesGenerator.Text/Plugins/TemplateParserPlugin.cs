using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using TalesGenerator.Net;
using System.Text;

namespace TalesGenerator.Text.Plugins
{
	public abstract class TemplateParserPlugin : ITemplateParserPlugin
	{
		#region Fields

		private readonly IEnumerable<Regex> _availableRegexes;
		#endregion

		#region Properties

		public abstract string Name { get; }
		#endregion

		#region Constructors

		public TemplateParserPlugin()
		{
			_availableRegexes = GetAvailableRegexes();
		}
		#endregion

		#region Methods

		protected abstract IEnumerable<Regex> GetAvailableRegexes();

		protected NetworkNode GetContextNodeByIndex(Match match, int groupIndex, IEnumerable<NetworkNode> networkNodes)
		{
			Contract.Requires<ArgumentNullException>(match != null);
			Contract.Requires<ArgumentException>(groupIndex < match.Groups.Count);
			Contract.Requires<ArgumentNullException>(networkNodes != null);
			Contract.Ensures(Contract.Result<NetworkNode>() != null);

			NetworkNode networkNode = null;

			// Проверим, задан ли явно индекс.
			if (match.Groups.Count > groupIndex &&
				match.Groups[groupIndex].Success)
			{
				int index;

				if (int.TryParse(match.Groups[groupIndex].Value, out index))
				{
					Contract.Assume(networkNodes.Count() > index);

					networkNode = networkNodes.ElementAt(index);
				}
			}

			if (networkNode == null)
			{
				networkNode = networkNodes.First();
			}

			return networkNode;
		}

		protected string Lemmatize(TextAnalyzer textAnalyzer, string text)
		{
			Contract.Requires<ArgumentNullException>(textAnalyzer != null);
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(text));

			string trimText = LexerUtils.TrimText(text);
			string[] words = trimText.Split(' ');
			StringBuilder stringBuilder = new StringBuilder(1024);

			for (int i = 0; i < words.Length; i++)
			{
				LemmatizeResult lemmatizeResult = textAnalyzer.Lemmatize(words[i]).FirstOrDefault();

				if (lemmatizeResult != null)
				{
					string lemma = lemmatizeResult.GetTextByFormId(0).ToLower();

					if (i != words.Length - 1)
					{
						stringBuilder.AppendFormat("{0} ", lemma);
					}
					else
					{
						stringBuilder.Append(lemma);
					}
				}
			}

			return stringBuilder.ToString();
		}

		public abstract TemplateParserPluginResult Parse(ITemplateParser parser, string template);

		public bool CanParse(string template)
		{
			if (string.IsNullOrEmpty(template))
			{
				throw new ArgumentException("template");
			}

			return _availableRegexes.Any(regex => regex.IsMatch(template));
		}
		#endregion
	}
}
