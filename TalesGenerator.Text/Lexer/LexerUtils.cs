using System;
using System.Diagnostics.Contracts;

namespace TalesGenerator.Text
{
	internal static class LexerUtils
	{
		internal static string TrimText(string text)
		{
			return TrimText(text, '-');
		}

		internal static string TrimText(string text, params char[] skipChars)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(text));
			Contract.Ensures(Contract.Result<string>() != null);

			string trimText = text;

			// TODO: Необходимо придумать более быстрый способ.
			foreach (char trimChar in Lexer.PunctuationChars)
			{
				if (Array.IndexOf(skipChars, trimChar) == -1)
				{
					trimText = trimText.Replace(trimChar.ToString(), string.Empty);
				}
			}

			return trimText;
		}
	}
}
