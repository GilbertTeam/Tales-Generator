using System;
using System.IO;
using System.Text;

namespace TalesGenerator.Text
{
	internal class Lexer : IDisposable
	{
		#region Fields

		private readonly StringReader _stringReader;

		private readonly char[] _punctuationChars =
		{
			'.', ',', ':', '!', '?', '-', '\"'
		};

		private readonly char[] _specialChars =
		{
			'~'
		};

		private readonly char[] _spaceChars =
		{
			' ', '\t'
		};

		private int _position;

		private int _currentChar;
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
			return Array.IndexOf(_punctuationChars, (char)character) != -1;
		}

		private bool IsSpaceChar(int character)
		{
			return Array.IndexOf(_spaceChars, (char)character) != -1;
		}

		private bool IsSpecialChar(int character)
		{
			return Array.IndexOf(_specialChars, (char)character) != -1;
		}

		private int ReadNextChar()
		{
			_position++;

			return _currentChar = _stringReader.Read();
		}

		private int PeekNextChar()
		{
			return _stringReader.Peek();
		}

		private void RaiseLexerException()
		{
			throw new LexerException(_currentChar, _position);
		}

		private void RaiseLexerException(Exception innerException)
		{
			throw new LexerException(_currentChar, _position, null, innerException);
		}

		private void RaiseLexerException(string invalidLexeme)
		{
			throw new LexerException(_currentChar, _position, string.Format(Properties.Resources.InvalidLexemeError, invalidLexeme));
		}

		private void RaiseLexerException(string invalidLexeme, Exception innerException)
		{
			throw new LexerException(_currentChar, _position, string.Format(Properties.Resources.InvalidLexemeError, invalidLexeme), innerException);
		}

		public bool GetNextToken(out LexerResult lexerResult)
		{
			if (ReadNextChar() == -1)
			{
				lexerResult = new LexerResult(null, TokenType.EOF);
				return false;
			}

			if (IsSpaceChar(_currentChar))
			{
				while (IsSpaceChar(PeekNextChar()))
				{
					ReadNextChar();
				}

				lexerResult = new LexerResult(" ", TokenType.Space);
				return true;
			}

			if (_currentChar == '{')
			{
				lexerResult = new LexerResult("{", TokenType.LeftBrace);
				return true;
			}
			else if (_currentChar == '}')
			{
				lexerResult = new LexerResult("}", TokenType.RightBrace);
				return true;
			}
			else if (IsPunctuationChar(_currentChar))
			{
				lexerResult = new LexerResult(((char)_currentChar).ToString(), TokenType.Punctuation);
				return true;
			}
			else if (IsSpecialChar(_currentChar))
			{
				lexerResult = new LexerResult(((char)_currentChar).ToString(), TokenType.SpecialSymbol);
				return true;
			}
			else if (char.IsLetter((char)_currentChar))
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				int peekChar = _currentChar;

				while (true)
				{
					stringBuilder.Append((char)peekChar);
					peekChar = PeekNextChar();

					if ((peekChar == -1 || !char.IsLetter((char)peekChar)))
					{
						break;
					}

					ReadNextChar();
				}

				lexerResult = new LexerResult(stringBuilder.ToString(), TokenType.Word);
				return true;
			}

			//TODO Необходима нейтрализация ошибок.
			throw new InvalidOperationException();
		}

		public void Dispose()
		{
			_stringReader.Dispose();
		}
		#endregion
	}
}
