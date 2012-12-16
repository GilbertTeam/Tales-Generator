using System;
using System.Collections.Generic;
using System.Linq;

namespace TalesGenerator.Net.Collections
{
	public static class NetworkEdgesExtension
	{
		private static void GetParentEdges(IEnumerable<NetworkEdge> edges, Func<NetworkEdge, bool> predicate, bool findAll, List<NetworkEdge> result)
		{
			NetworkEdge isAEdge = edges.SingleOrDefault(edge => edge.Type == NetworkEdgeType.IsA);

			if (isAEdge != null)
			{
				NetworkNode baseNode = isAEdge.EndNode;

				result.AddRange(baseNode.OutgoingEdges.Where(predicate));

				if (findAll || result.Count == 0)
				{
					//TODO Необходимо учитывать направление дуг (incoming/outgoing);
					GetParentEdges(baseNode.OutgoingEdges, predicate, findAll, result);
				}
			}
		}

		public static NetworkEdge GetEdge(this IEnumerable<NetworkEdge> networkEdges, NetworkEdgeType edgeType)
		{
			return networkEdges.FirstOrDefault(edge => edge.Type == edgeType);
		}

		public static NetworkEdge GetEdge(this IEnumerable<NetworkEdge> networkEdges, NetworkEdgeType edgeType, bool recursively)
		{
			NetworkEdge foundEdge = GetEdge(networkEdges, edgeType);

			if (foundEdge == null && recursively)
			{
				List<NetworkEdge> result = new List<NetworkEdge>();

				GetParentEdges(networkEdges, (edge) => edge.Type == edgeType, false, result);

				foundEdge = result.FirstOrDefault();
			}

			return foundEdge;
		}

		public static IEnumerable<NetworkEdge> GetEdges(this IEnumerable<NetworkEdge> networkEdges, NetworkEdgeType edgeType)
		{
			return networkEdges.Where(edge => edge.Type == edgeType);
		}

		public static IEnumerable<NetworkEdge> GetEdges(this IEnumerable<NetworkEdge> networkEdges, NetworkEdgeType edgeType, bool recursively)
		{
			var foundEdges = GetEdges(networkEdges, edgeType);

			if (!foundEdges.Any() && recursively)
			{
				List<NetworkEdge> result = new List<NetworkEdge>();

				GetParentEdges(networkEdges, (edge) => edge.Type == edgeType, true, result);

				foundEdges = result;
			}

			return foundEdges;
		}
	}
}
