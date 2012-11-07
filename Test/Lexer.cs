using System;
using System.IO;
using System.Text;

namespace Test
{
	public delegate string ParseStringDelegate(string value);

	public class Lexer
	{
		#region Fields

		private readonly StringReader _stringReader;

		private readonly StringWriter _stringWriter;

		private readonly char[] _punctuations =
		{
			'.', ',', ':', '!', '?', '-'
		};

		private int _position;

		private int _character;
		#endregion

		#region Events

		public event ParseStringDelegate ParseTemplateItem;

		public event ParseStringDelegate ParseWord;
		#endregion

		#region Constructors

		public Lexer(string template)
		{
			if (string.IsNullOrEmpty(template))
			{
				throw new ArgumentException("template");
			}

			_stringReader = new StringReader(template);
			_stringWriter = new StringWriter();
		}
		#endregion

		#region Methods

		private bool IsPunctuationChar(char c)
		{
			return Array.IndexOf(_punctuations, c) != -1;
		}

		private int GetNextChar()
		{
			_position++;

			return _character = _stringReader.Read();
		}

		private void RaiseLexerException()
		{
			throw new LexerException(_character, _position);
		}

		private void RaiseLexerException(Exception innerException)
		{
			throw new LexerException(_character, _position, null, innerException);
		}

		private void RaiseLexerException(string invalidLexeme)
		{
			throw new LexerException(_character, _position, string.Format(Resources.InvalidLexemeError, invalidLexeme));
		}

		private void RaiseLexerException(string invalidLexeme, Exception innerException)
		{
			throw new LexerException(_character, _position, string.Format(Resources.InvalidLexemeError, invalidLexeme), innerException);
		}

		public string Parse()
		{
			if (ParseTemplateItem == null)
			{
				throw new InvalidOperationException();
			}
			if (ParseWord == null)
			{
				throw new InvalidOperationException();
			}

			StringBuilder stringBuilder = new StringBuilder(1024);
			GetNextChar();

			while (_character != -1)
			{
				if (_character == ' ')
				{
					_stringWriter.Write(' ');

					while (GetNextChar() == ' ') ;
				}

				if (_character == '{')
				{
					GetNextChar();

					stringBuilder.Clear();

					while (_character != -1 && _character != '}')
					{
						stringBuilder.Append((char)_character);
						GetNextChar();
					}

					if (_character == -1)
					{
						RaiseLexerException();
					}

					string result = string.Empty;
					string lexeme = stringBuilder.ToString();

					try
					{
						result = ParseTemplateItem(lexeme);
					}
					catch (Exception exception)
					{
						RaiseLexerException(lexeme, exception);
					}

					if (string.IsNullOrEmpty(result))
					{
						RaiseLexerException();
					}

					_stringWriter.Write(result);
				}
				else if (_character == '}')
				{
					GetNextChar();
				}
				else if (char.IsPunctuation((char)_character))
				{
					_stringWriter.Write((char)_character);
					GetNextChar();
				}
				else if (char.IsLetter((char)_character))
				{
					_stringWriter.Write((char)_character);
					GetNextChar();
					//stringBuilder.Clear();

					//while (_character != -1 && char.IsLetter((char)_character))
					//{
					//    stringBuilder.Append((char)_character);
					//    GetNextChar();
					//}

					//if (_character == -1)
					//{
					//    RaiseLexerException();
					//}

					//string result = string.Empty;
					//string lexeme = stringBuilder.ToString();

					//try
					//{
					//    result = ParseWord(lexeme);
					//}
					//catch (Exception exception)
					//{
					//    RaiseLexerException(lexeme, exception);
					//}

					//if (string.IsNullOrEmpty(result))
					//{
					//    RaiseLexerException();
					//}

					//_stringWriter.Write(result);
				}
				else
				{
					RaiseLexerException();
				}
			}

			return _stringWriter.ToString();
		}
		#endregion
	}
}
