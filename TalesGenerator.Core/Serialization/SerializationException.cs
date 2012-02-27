using System;
using RuntimeSerialization = System.Runtime.Serialization;

namespace TalesGenerator.Core.Serialization
{
	[Serializable]
	public class SerializationException : Exception
	{
		#region Constructors

		public SerializationException()
			: base()
		{
		}

		public SerializationException(string message)
			: base(message)
		{
		}

		public SerializationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public SerializationException(RuntimeSerialization.SerializationInfo serializationInfo, RuntimeSerialization.StreamingContext context)
			: base(serializationInfo, context)
		{

		}
		#endregion
	}
}
