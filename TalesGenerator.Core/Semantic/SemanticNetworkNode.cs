using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalesGenerator.Core.Semantic
{
	public class SemanticNetworkNode : NetworkNode
	{
		#region Fields

		private readonly NetworkNode _baseNode;
		#endregion

		#region Properties

		public override IEnumerable<NetworkEdge> OutgoingEdges
		{
			get
			{
				//Переопределенные дуги.
				var outgoingEdges = base.OutgoingEdges;

				if (_baseNode != null)
				{
					//Унаследованные дуги.
					outgoingEdges.Concat(_baseNode.OutgoingEdges);
				}

				return outgoingEdges;
			}
		}

		public NetworkNode BaseNode
		{
			get { return _baseNode; }
		}
		#endregion

		#region Constructors

		internal SemanticNetworkNode(Network network)
			: base(network)
		{

		}

		internal SemanticNetworkNode(Network network, string name)
			: base(network, name)
		{

		}

		internal SemanticNetworkNode(Network network, NetworkNode baseNode)
			: base(network)
		{
			_baseNode = baseNode;
		}

		internal SemanticNetworkNode(Network network, string name, NetworkNode baseNode)
			: base(network, name)
		{

		}
		#endregion

		#region Methods

		public SemanticNetworkEdge Override(SemanticNetworkEdge networkEdge, SemanticNetworkNode networkNode)
		{
			if (networkEdge == null)
			{
				throw new ArgumentNullException("networkEdge");
			}
			if (_baseNode == null)
			{
				throw new InvalidOperationException();
			}
			if (!_baseNode.IncomingEdges.Contains(networkEdge))
			{
				throw new ArgumentException("networkEdge");
			}
			if (networkNode == null)
			{
				throw new ArgumentNullException("networkNode");
			}

			SemanticNetworkEdge overridedEdge = new SemanticNetworkEdge(_network, this, networkNode, networkEdge);
			_network.Edges.Add(overridedEdge);

			return overridedEdge;
		}
		#endregion
	}
}
