using System;

namespace TalesGenerator.Core.Collections
{
	/// <summary>
	/// Коллекция дуг сети.
	/// </summary>
	public class NetworkEdgeCollection : NetworkObjectCollection<NetworkEdge>
	{
		#region Constructors

		internal NetworkEdgeCollection(Network network)
			: base(network)
		{
		}
		#endregion

		#region Methods

		/// <summary>
		/// Добавляет новую дугу сети с типом is-a.
		/// </summary>
		/// <param name="startNode">Вершина, из которой должна исходить новая дуга.</param>
		/// <param name="endNode">Вершина, в которую должна входить новая дуга.</param>
		/// <returns>Новая дуга.</returns>
		public NetworkEdge Add(NetworkNode startNode, NetworkNode endNode)
		{
			NetworkEdge networkEdge = Add(startNode, endNode, NetworkEdgeType.IsA);

			return networkEdge;
		}

		/// <summary>
		/// Добавляет новую дугу сети с указанным типом.
		/// </summary>
		/// <param name="startNode">Вершина, из которой должна исходить новая дуга.</param>
		/// <param name="endNode">Вершина, в которую должна входить новая дуга.</param>
		/// <param name="edgeType">Тип дуги.</param>
		/// <returns>Новая дуга.</returns>
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

			if (edgeType == NetworkEdgeType.IsA ||
				edgeType == NetworkEdgeType.IsInstance)
			{
				if (startNode.BaseNode != null ||
					startNode.InstanceNode != null)
				{
					throw new ArgumentException(Properties.Resources.NetworkIsInstanceEdgeError);
				}

				if (endNode.InstanceNode != null)
				{
					throw new ArgumentException(Properties.Resources.NetworkIsInstanceEdgeError2);
				}
			}

			//if (startNode.OutgoingEdges.GetEdge(edgeType) != null)
			//{
			//    throw new ArgumentException(Properties.Resources.NetworkSameEdgesError);
			//}

			NetworkEdge networkEdge = new NetworkEdge(_network, startNode, endNode, edgeType);

			Add(networkEdge);

			return networkEdge;
		}

		public override bool Remove(NetworkEdge networkEdge)
		{
			if (networkEdge == null)
			{
				throw new ArgumentNullException("networkEdge");
			}

			return base.Remove(networkEdge);
		}
		#endregion
	}
}
