using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalesGenerator.Core.Semantic
{
	public class SemanticNetworkEdge : NetworkEdge
	{
		#region Fields

		private readonly NetworkEdge _baseEdge;
		#endregion

		#region Constructors

		internal SemanticNetworkEdge(Network network, NetworkNode startNode, NetworkNode endNode)
			: base(network, startNode, endNode)
		{

		}

		internal SemanticNetworkEdge(Network network, NetworkNode startNode, NetworkNode endNode, NetworkEdge baseEdge)
			: base(network, startNode, endNode)
		{
			if (baseEdge == null)
			{
				throw new ArgumentNullException("baseEdge");
			}

			_edgeType = baseEdge.Type;
		}
		#endregion

		#region Methods

		public SemanticNetworkEdge Override(SemanticNetworkNode startNode, SemanticNetworkNode endNode)
		{
			if (startNode == null)
			{
				throw new ArgumentNullException("startNode");
			}
			if (startNode.IncomingEdges.Contains(this) ||
				startNode.OutgoingEdges.Contains(this))
			{
				throw new ArgumentException("startNode");
			}
			if (endNode == null)
			{
				throw new ArgumentNullException("endNode");
			}
			if (endNode.IncomingEdges.Contains(this) ||
				endNode.OutgoingEdges.Contains(this))
			{
				throw new ArgumentException("endNode");
			}

			SemanticNetworkEdge overridedEdge = new SemanticNetworkEdge(_network, startNode, endNode, this);
			_network.Edges.Add(overridedEdge);

			return overridedEdge;
		}
		#endregion
	}
}
