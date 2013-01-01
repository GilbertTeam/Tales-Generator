using System;
using System.Diagnostics.Contracts;

namespace TalesGenerator.Text
{
	internal static class LexerUtils
	{
		internal static string TrimText(string text)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(text));
			Contract.Ensures(Contract.Result<string>() != null);

			string trimText = text;

			// TODO: Необходимо придумать более быстрый способ.
			foreach (char trimChar in Lexer.PunctuationChars)
			{
				if (trimChar != '-')
				trimText = trimText.Replace(trimChar.ToString(), string.Empty);
			}

			return trimText;
		}
	}
}
