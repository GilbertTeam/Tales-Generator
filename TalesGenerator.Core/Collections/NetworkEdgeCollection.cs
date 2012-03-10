using System;

namespace TalesGenerator.Core.Collections
{
	public class NetworkEdgeCollection : NetworkObjectCollection<NetworkEdge>
	{
		#region Constructors

		public NetworkEdgeCollection(Network network)
			: base(network)
		{
		}
		#endregion

		#region Methods

		public NetworkEdge Add(NetworkNode startNode, NetworkNode endNode)
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
				throw new ArgumentException("Невозможно создать дугу с началом, совпадающим с концом");
			}

			NetworkEdge networkEdge = new NetworkEdge(_network, startNode, endNode);

			Add(networkEdge);

			return networkEdge;
		}
		#endregion
	}
}
