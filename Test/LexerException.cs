using System;

namespace Test
{
	public class LexerException : Exception
	{
		#region Properties

		public int InvalidChar { get; private set; }

		public int InvalidPosition { get; private set; }
		#endregion

		#region Constructors

		public LexerException(int invalidChar, int invalidPosition)
			: this(invalidChar, invalidPosition, null)
		{

		}

		public LexerException(int invalidChar, int invalidPosition, string message)
			: this(invalidChar, invalidPosition, message, null)
		{

		}

		public LexerException(int invalidChar, int invalidPosition, string message, Exception innerException)
			: base(message, innerException)
		{
			InvalidChar = invalidChar;
			InvalidPosition = invalidPosition;
		}
		#endregion
	}
}
