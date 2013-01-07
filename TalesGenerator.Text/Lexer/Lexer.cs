using System;
using System.IO;
using System.Text;

namespace TalesGenerator.Text
{
	internal class Lexer : IDisposable
	{
		#region Fields

		private readonly StringReader _stringReader;

		private int _position;

		private bool _inTemplate;

		public static char[] PunctuationChars =
		{
			',', ':', ';', '!', '?', '-', '\"'
		};

		public static char[] SpaceChars =
		{
			' ', '\t'
		};

		public static char[] SpecialChars =
		{
			'~', ':'
		};
		#endregion

		public Lexer(string source)
		{
			if (source == null)
			{
				throw new ArgumentException("source");
			}

			_stringReader = new StringReader(source);
		}

		#region Methods

		private bool IsPunctuationChar(int character)
		{
			return Array.IndexOf(PunctuationChars, (char)character) != -1;
		}

		private bool IsSpaceChar(int character)
		{
			return Array.IndexOf(SpaceChars, (char)character) != -1;
		}

		private bool IsSpecialChar(int character)
		{
			return Array.IndexOf(SpecialChars, (char)character) != -1;
		}

		private int ReadNextChar()
		{
			_position++;

			return _stringReader.Read();
		}

		private int PeekNextChar()
		{
			return _stringReader.Peek();
		}

		private void RaiseLexerException(int invalidChar)
		{
			throw new LexerException(invalidChar, _position);
		}

		private void RaiseLexerException(int invalidChar, Exception innerException)
		{
			throw new LexerException(invalidChar, _position, null, innerException);
		}

		private bool AnalyzeChar(int character, out LexerResult lexerResult)
		{
			if (character == '{')
			{
				_inTemplate = true;

				lexerResult = new LexerResult("{", TokenType.LeftBrace);
				return true;
			}
			else if (character == '}')
			{
				_inTemplate = false;

				lexerResult = new LexerResult("}", TokenType.RightBrace);
				return true;
			}
			else if (character == '.')
			{
				lexerResult = new LexerResult(".", TokenType.Point);
				return true;
			}
			else if (character == ':')
			{
				lexerResult = new LexerResult(":", TokenType.Colon);
				return true;
			}
			else if (character == '\"')
			{
				lexerResult = new LexerResult("\"", TokenType.Quotes);
				return true;
			}
			else if (_inTemplate && IsSpecialChar(character))
			{
				lexerResult = new LexerResult(((char)character).ToString(), TokenType.SpecialSymbol);
				return true;
			}
			else if (IsPunctuationChar(character))
			{
				lexerResult = new LexerResult(((char)character).ToString(), TokenType.Punctuation);
				return true;
			}
			else if (char.IsLetter((char)character))
			{
				lexerResult = new LexerResult(((char)character).ToString(), TokenType.Letter);
				return true;
			}
			else if (char.IsDigit((char)character))
			{
				lexerResult = new LexerResult(((char)character).ToString(), TokenType.Digit);
				return true;
			}

			lexerResult = null;
			RaiseLexerException(character);
			return false;
		}

		public bool ReadNextToken(out LexerResult lexerResult)
		{
			int character = ReadNextChar();

			if (character == -1)
			{
				lexerResult = new LexerResult(null, TokenType.EndOfStream);
				return false;
			}

			if (IsSpaceChar(character))
			{
				while (IsSpaceChar(PeekNextChar()))
				{
					character = ReadNextChar();
				}

				lexerResult = new LexerResult(" ", TokenType.Space);
				return true;
			}

			return AnalyzeChar(character, out lexerResult);
		}

		public bool PeekNextToken(out LexerResult lexerResult)
		{
			int character = PeekNextChar();

			if (character == -1)
			{
				lexerResult = new LexerResult(null, TokenType.EndOfStream);
				return false;
			}

			if (IsSpaceChar(character))
			{
				while (IsSpaceChar(PeekNextChar()))
				{
					character = ReadNextChar();
				}

				lexerResult = new LexerResult(" ", TokenType.Space);
				return true;
			}

			return AnalyzeChar(character, out lexerResult);
		}

		public void Dispose()
		{
			_stringReader.Dispose();
		}
		#endregion
	}
}
