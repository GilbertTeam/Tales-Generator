using System.Collections.Generic;
using TalesGenerator.Net;
using System.Diagnostics.Contracts;
using System;

namespace TalesGenerator.Text
{
	public class TemplateParserNetworkDictionaryContext : ITemplateParserNetworkContext
	{
		#region Fields

		private readonly Dictionary<NetworkEdgeType, List<NetworkNode>> _dictionary = new Dictionary<NetworkEdgeType, List<NetworkNode>>();
		#endregion

		#region Properties

		public IEnumerable<NetworkNode> this[NetworkEdgeType edgeType]
		{
			get
			{
				return GetNetworkNodes(edgeType);
			}
		}
		#endregion

		#region Methods

		private List<NetworkNode> GetNetworkNodes(NetworkEdgeType edgeType)
		{
			List<NetworkNode> networkNodes;

			if (!_dictionary.TryGetValue(edgeType, out networkNodes))
			{
				networkNodes = new List<NetworkNode>();
				_dictionary[edgeType] = networkNodes;
			}

			return networkNodes;
		}

		public void Add(NetworkEdgeType edgeType, params NetworkNode[] networkNodes)
		{
			Add(edgeType, (IEnumerable<NetworkNode>)networkNodes);
		}

		public void Add(NetworkEdgeType edgeType, IEnumerable<NetworkNode> networkNodes)
		{
			Contract.Requires<ArgumentNullException>(networkNodes != null);

			GetNetworkNodes(edgeType).AddRange(networkNodes);
		}
		#endregion
	}
}
