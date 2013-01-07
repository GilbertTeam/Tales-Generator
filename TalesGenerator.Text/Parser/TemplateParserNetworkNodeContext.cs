using System;
using System.Collections.Generic;
using System.Linq;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;

namespace TalesGenerator.Text
{
	public class TemplateParserNetworkNodeContext : ITemplateParserContext
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

		public int Count
		{
			get
			{
				throw new NotImplementedException();
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

		#region Methods

		public IEnumerator<KeyValuePair<NetworkEdgeType, IEnumerable<NetworkNode>>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
