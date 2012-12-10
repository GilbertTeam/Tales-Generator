using System;

namespace TalesGenerator.Text
{
	public class TemplateParserException : Exception
	{
		#region Constructors

		public TemplateParserException()
		{

		}

		public TemplateParserException(string message)
			: base(message)
		{

		}

		public TemplateParserException(string message, Exception innerException)
			: base(message, innerException)
		{

		}
		#endregion
	}
}
