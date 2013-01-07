using System.Collections.Generic;
using TalesGenerator.Net;
using System.Diagnostics.Contracts;
using System;
using System.Linq;
using System.Collections;

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

		public int Count
		{
			get
			{
				return _dictionary.Count;
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

			List<NetworkNode> dictionaryList = GetNetworkNodes(edgeType);

			foreach (NetworkNode node in networkNodes)
			{
				if (!dictionaryList.Contains(node))
				{
					dictionaryList.Add(node);
				}
			}
		}

		public IEnumerator<KeyValuePair<NetworkEdgeType, IEnumerable<NetworkNode>>> GetEnumerator()
		{
			return
				_dictionary
				.Select(pair => new KeyValuePair<NetworkEdgeType, IEnumerable<NetworkNode>>(pair.Key, pair.Value))
				.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}
		#endregion
	}
}
