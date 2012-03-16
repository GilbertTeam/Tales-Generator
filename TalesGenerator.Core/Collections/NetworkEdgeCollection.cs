using System;

namespace TalesGenerator.Core.Collections
{
	public class NetworkEdgeCollection : NetworkObjectCollection<NetworkEdge>
	{
		#region Constructors

		internal NetworkEdgeCollection(Network network)
			: base(network)
		{
		}
		#endregion

		#region Methods

		public NetworkEdge Add(NetworkNode startNode, NetworkNode endNode)
		{
			NetworkEdge networkEdge = Add(startNode, endNode, NetworkEdgeType.IsA);

			return networkEdge;
		}

		public NetworkEdge Add(NetworkNode startNode, NetworkNode endNode, NetworkEdgeType edgeType)
		{
			if (startNode == null)
			{
				throw new ArgumentNullException("startNode");
			}
			if (endNode == null)
			{
				throw new ArgumentNullException("endNode");
			}
			if (startNode == endNode)
			{
				throw new ArgumentException(Properties.Resources.NetworkEdgeCreateError);
			}

			NetworkEdge networkEdge = new NetworkEdge(_network, startNode, endNode, edgeType);

			Add(networkEdge);

			if (edgeType == NetworkEdgeType.IsA)
			{
				startNode.BaseNode = endNode;
			}

			return networkEdge;
		}

		public override bool Remove(NetworkEdge networkEdge)
		{
			if (networkEdge == null)
			{
				throw new ArgumentNullException("networkEdge");
			}

			if (networkEdge.Type == NetworkEdgeType.IsA)
			{
				networkEdge.EndNode.BaseNode = null;
			}

			return base.Remove(networkEdge);
		}
		#endregion
	}
}
