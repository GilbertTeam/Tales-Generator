using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;

namespace TalesGenerator.TaleNet.Collections
{
	public class FunctionNodeContextNodeCollection : NetworkObjectCollection<TaleItemNode>, IEnumerable, IEnumerable<TaleItemNode>
	{
		#region Fields

		private readonly FunctionNode _functionNode;

		private readonly NetworkEdgeType _edgeType;
		#endregion

		#region Constructors

		internal FunctionNodeContextNodeCollection(FunctionNode functionNode, NetworkEdgeType edgeType)
			: base(functionNode.Network)
		{
			_functionNode = functionNode;
			_edgeType = edgeType;
		}
		#endregion

		#region Methods

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<TaleItemNode>)this).GetEnumerator();
		}

		IEnumerator<TaleItemNode> IEnumerable<TaleItemNode>.GetEnumerator()
		{
			//var contextNodes = Network.Nodes.OfType<TaleItemNode>().Where(
			//	node =>
			//	{
			//		NetworkEdge contextEdge = node.IncomingEdges.GetEdge(_edgeType, false);

			//		return contextEdge != null && contextEdge.StartNode == _functionNode;
			//	}
			//);
			var contextNodes = _functionNode
				.OutgoingEdges
				.GetEdges(_edgeType)
				.Select(edge => edge.EndNode)
				.OfType<TaleItemNode>();

			if (!contextNodes.Any() &&
				_functionNode.BaseNode != null)
			{
				contextNodes = _functionNode.BaseNode
				.OutgoingEdges
				.GetEdges(_edgeType)
				.Select(edge => edge.EndNode)
				.OfType<TaleItemNode>();
			}
			
			return contextNodes.GetEnumerator();
		}

		public override void Add(TaleItemNode contextNode)
		{
			Contract.Requires<ArgumentNullException>(contextNode != null);

			Network.Edges.Add(_functionNode, contextNode, _edgeType);

			base.Add(contextNode);
		}

		public override bool Remove(TaleItemNode contextNode)
		{
			Contract.Requires<ArgumentNullException>(contextNode != null);

			NetworkEdge actorEdge = _functionNode.OutgoingEdges.GetEdges(_edgeType).Single(edge => edge.EndNode == contextNode);

			Network.Edges.Remove(actorEdge);

			return base.Remove(contextNode);
		}

		public void Add(params TaleItemNode[] actorNodes)
		{
			Add((IEnumerable<TaleItemNode>)actorNodes);
		}

		public void Add(IEnumerable<TaleItemNode> actorNodes)
		{
			Contract.Requires<ArgumentNullException>(actorNodes != null);

			foreach (TaleItemNode actorNode in actorNodes)
			{
				((NetworkObjectCollection<TaleItemNode>)this).Add(actorNode);
			}
		}
		#endregion
	}
}
