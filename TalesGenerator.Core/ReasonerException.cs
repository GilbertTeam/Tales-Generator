using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalesGenerator.Core
{
	public class ReasonerException : Exception
	{
		#region Constructors

		public ReasonerException()
		{

		}

		public ReasonerException(string message)
			: base(message)
		{

		}

		public ReasonerException(string message, Exception innerException)
			: base(message, innerException)
		{

		}
		#endregion
	}
}
