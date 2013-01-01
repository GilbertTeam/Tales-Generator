using System;
using System.Collections.Generic;
using System.Linq;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;

namespace TalesGenerator.Text
{
	public class TemplateParserNetworkNodeContext : ITemplateParserNetworkContext
	{
		#region Fields

		private readonly NetworkNode _networkNode;
		#endregion

		#region Properties

		public IEnumerable<NetworkNode> this[NetworkEdgeType edgeType]
		{
			get
			{
				return _networkNode.OutgoingEdges.GetEdges(edgeType, true).Select(edge => edge.EndNode);
			}
		}
		#endregion

		#region Constructors

		public TemplateParserNetworkNodeContext(NetworkNode networkNode)
		{
			if (networkNode == null)
			{
				throw new ArgumentNullException("networkNode");
			}

			_networkNode = networkNode;
		}
		#endregion
	}
}
