using System;
using System.Collections.Generic;
using System.Linq;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;

namespace TalesGenerator.TaleNet.Collections
{
	//TODO Скорее всего нужно будет переделать на TaleNode.
	public class FunctionNodeActorCollection : NetworkObjectCollection<NetworkNode>
	{
		#region Fields

		private readonly FunctionNode _functionNode;

		private readonly NetworkEdgeType _edgeType;
		#endregion

		#region Constructors

		internal FunctionNodeActorCollection(FunctionNode functionNode, NetworkEdgeType edgeType)
			: base(functionNode.Network)
		{
			_functionNode = functionNode;
			_edgeType = edgeType;
		}
		#endregion

		#region Methods

		public override void Add(NetworkNode actorNode)
		{
			if (actorNode == null)
			{
				throw new ArgumentNullException("actorNode");
			}

			Network.Edges.Add(_functionNode, actorNode, _edgeType);

			base.Add(actorNode);
		}

		public override bool Remove(NetworkNode actorNode)
		{
			if (actorNode == null)
			{
				throw new ArgumentNullException("actorNode");
			}

			NetworkEdge actorEdge = _functionNode.OutgoingEdges.GetEdges(_edgeType).SingleOrDefault(edge => edge.EndNode == actorNode);

			if (actorEdge != null)
			{
				Network.Edges.Remove(actorEdge);
			}

			return base.Remove(actorNode);
		}

		public void Add(params NetworkNode[] actorNodes)
		{
			NetworkObjectCollection<NetworkNode> networkObjectCollection = (NetworkObjectCollection<NetworkNode>)this;

			foreach (NetworkNode actorNode in actorNodes)
			{
				networkObjectCollection.Add(actorNode);
			}
		}

		public void Add(IEnumerable<NetworkNode> actorNodes)
		{
			if (actorNodes == null)
			{
				throw new ArgumentNullException("actorNodes");
			}

			foreach (NetworkNode actorNode in actorNodes)
			{
				Add(actorNode);
			}
		}
		#endregion
	}
}
