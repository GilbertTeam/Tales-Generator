﻿using System.Collections.Generic;
using TalesGenerator.Net;

namespace TalesGenerator.Text
{
	public class TemplateParserDictionaryContext : ITemplateParserContext
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
			GetNetworkNodes(edgeType).AddRange(networkNodes);
		}
		#endregion
	}
}
