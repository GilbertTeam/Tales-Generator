using System.Collections;
using System.Collections.Generic;
using TalesGenerator.Net;

namespace TalesGenerator.Text
{
	public interface ITemplateParserContext : IEnumerable<KeyValuePair<NetworkEdgeType, IEnumerable<NetworkNode>>>, IEnumerable
	{
		#region Properties

		IEnumerable<NetworkNode> this[NetworkEdgeType edgeType] { get; }

		int Count { get; }
		#endregion
	}
}
