using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;
using TalesGenerator.Text;

namespace TalesGenerator.TaleNet
{
	public class TaleItemNodeCollection : NetworkObjectCollection<TaleItemNode>, IEnumerable, IEnumerable<TaleItemNode>
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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<TaleItemNode>)this).GetEnumerator();
		}

		IEnumerator<TaleItemNode> IEnumerable<TaleItemNode>.GetEnumerator()
		{
			return Network.Nodes.OfType<TaleItemNode>().Where(node => node.IsInherit(_baseNode, false)).GetEnumerator();
		}

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
			return Add(name, (Grammem)0);
		}

		public TaleItemNode Add(string name, Grammem grammem)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));

			TaleItemNode taleItemNode = new TaleItemNode((TalesNetwork)Network, name) { Grammem = grammem };

			Network.Edges.Add(taleItemNode, _baseNode, NetworkEdgeType.IsA);

			Add(taleItemNode);

			return taleItemNode;
		}

		public TaleItemNode Add(string name,TaleItemNode baseNode)
		{
			return Add(name, baseNode, (Grammem)0);
		}

		public TaleItemNode Add(string name, TaleItemNode baseNode, Grammem grammem)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(name));
			Contract.Requires<ArgumentNullException>(baseNode != null);

			TaleItemNode taleItemNode = new TaleItemNode((TalesNetwork)Network, name, baseNode) { Grammem = grammem };

			Add(taleItemNode);

			return taleItemNode;
		}
		#endregion
	}
}
