using System.Collections.Generic;
using TalesGenerator.Net;

namespace TalesGenerator.Text
{
	public interface ITemplateParserNetworkContext
	{
		#region Properties

		IEnumerable<NetworkNode> this[NetworkEdgeType edgeType] { get; }
		#endregion
	}
}
