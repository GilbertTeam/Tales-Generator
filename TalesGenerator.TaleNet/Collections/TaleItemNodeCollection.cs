using System;
using System.Diagnostics.Contracts;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;

namespace TalesGenerator.TaleNet
{
	public class TaleItemNodeCollection : NetworkObjectCollection<TaleItemNode>
	{
		#region Fields

		private readonly TaleItemNode _baseNode;
		#endregion

		#region Constructors

		internal TaleItemNodeCollection(TalesNetwork talesNetwork, TaleItemNode baseNode)
			: base(talesNetwork)
		{
			Contract.Requires<ArgumentNullException>(baseNode != null);

			_baseNode = baseNode;
		}
		#endregion

		#region Methods

		public override void Add(TaleItemNode taleItemNode)
		{
			Contract.Requires<ArgumentNullException>(taleItemNode != null);

			//Необходимо добавить вершину в саму сеть.
			Network.Nodes.Add(taleItemNode);

			base.Add(taleItemNode);
		}

		public override bool Remove(TaleItemNode taleItemNode)
		{
			Contract.Requires<ArgumentNullException>(taleItemNode != null);

			// 1. Сначала необходимо удалить дугу is-a:
			{
				NetworkEdge isAEdge = taleItemNode.OutgoingEdges.GetEdge(NetworkEdgeType.IsA);

				Contract.Assume(isAEdge != null);

				Network.Edges.Remove(isAEdge);
			}

			// 2. Затем необходимо удалить саму вершину из сети.
			Network.Nodes.Remove(taleItemNode);

			// 3. А затем удалить вершину из коллекции.
			return base.Remove(taleItemNode);
		}

		public TaleItemNode Add(string name)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));

			TaleItemNode taleItemNode = new TaleItemNode((TalesNetwork)Network, name);

			Network.Edges.Add(taleItemNode, _baseNode, NetworkEdgeType.IsA);

			Add(taleItemNode);

			return taleItemNode;
		}

		public TaleItemNode Add(string name, TaleItemNode baseNode)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));
			Contract.Requires<ArgumentNullException>(baseNode != null);

			TaleItemNode taleItemNode = new TaleItemNode((TalesNetwork)Network, name, baseNode);

			Add(taleItemNode);

			return taleItemNode;
		}
		#endregion
	}
}
