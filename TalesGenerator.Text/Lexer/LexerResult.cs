using System;

namespace TalesGenerator.Text
{
	internal enum TokenType
	{
		Word,
		Space,
		Punctuation,
		SpecialSymbol,
		LeftBrace,
		RightBrace,
		EOF
	}

	internal class LexerResult
	{
		#region Properties

		public string Token { get; private set; }

		public TokenType Type { get; private set; }
		#endregion

		#region Constructors

		public LexerResult(string token, TokenType tokenType)
		{
			Token = token;
			Type = tokenType;
		}
		#endregion

		#region Methods

		public override string ToString()
		{
			return string.Format("Token = \"{0}\", Type = {1}.", Token, Type);
		}
		#endregion
	}
}
