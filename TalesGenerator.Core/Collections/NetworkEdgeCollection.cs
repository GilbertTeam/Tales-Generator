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

		protected override void RemoveItem(int index)
		{
			NetworkEdge deletingEdge = Items[index];

			if (deletingEdge.Type == NetworkEdgeType.IsA)
			{
				deletingEdge.EndNode.BaseNode = null;
			}

			base.RemoveItem(index);
		}

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
				endNode.BaseNode = startNode;
			}

			return networkEdge;
		}
		#endregion
	}
}
