using System.Collections.Generic;
using TalesGenerator.Net;

namespace TalesGenerator.Text
{
	public interface ITemplateParserContext
	{
		#region Properties

		IEnumerable<NetworkNode> this[NetworkEdgeType edgeType] { get; }
		#endregion
	}
}
