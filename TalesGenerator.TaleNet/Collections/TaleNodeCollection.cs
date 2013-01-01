using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TalesGenerator.TaleNet.Collections
{
	public class TaleNodeCollection : BaseTaleNodeCollection<TaleNode>, IEnumerable, IEnumerable<TaleNode>
	{
		#region Constructors

		internal TaleNodeCollection(TalesNetwork talesNetwork)
			: base(talesNetwork)
		{

		}
		#endregion

		#region Methods

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<TaleNode>)this).GetEnumerator();
		}

		IEnumerator<TaleNode> IEnumerable<TaleNode>.GetEnumerator()
		{
			return Network.Nodes.OfType<TaleNode>().GetEnumerator();
		}

		public TaleNode Add(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}

			TaleNode taleNode = new TaleNode((TalesNetwork)Network, name);

			Network.Edges.Add(taleNode, ((TalesNetwork)Network).BaseTale, Net.NetworkEdgeType.IsA);

			Add(taleNode);

			return taleNode;
		}

		public TaleNode Add(string name, TaleNode baseTaleNode)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}
			if (baseTaleNode == null)
			{
				throw new ArgumentNullException("baseTaleNode");
			}

			TaleNode taleNode = new TaleNode((TalesNetwork)Network, name, baseTaleNode);

			Add(taleNode);

			return taleNode;
		}
		#endregion
	}
}
