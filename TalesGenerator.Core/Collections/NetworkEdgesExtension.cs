using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalesGenerator.Core.Collections
{
	public static class NetworkEdgesExtension
	{
		private static void GetAllEdges(NetworkEdge startEdge, IList<NetworkEdge> allEdges)
		{
			if (!allEdges.Contains(startEdge))
			{
				allEdges.Add(startEdge);

				var isAEdges = startEdge.StartNode.OutgoingEdges.Where(edge => edge.Type == NetworkEdgeType.IsA).ToList();

				foreach (NetworkEdge isAEdge in isAEdges)
				{
					GetAllEdges(isAEdge, allEdges);
				}
			}
		}

		private static void GetAllEdges(IEnumerable<NetworkEdge> edges, IList<NetworkEdge> allEdges)
		{
			foreach (NetworkEdge edge in edges)
			{
				if (!allEdges.Contains(edge))
				{
					allEdges.Add(edge);

					var isAEdges = edge.StartNode.OutgoingEdges.Where(e => e.Type == NetworkEdgeType.IsA);

					foreach (NetworkEdge isAEdge in isAEdges)
					{
						GetAllEdges(isAEdge.EndNode.OutgoingEdges, allEdges);
					}
				}
			}
		}

		public static NetworkEdge GetEdge(this IEnumerable<NetworkEdge> networkEdges, NetworkEdgeType edgeType)
		{
			return networkEdges.FirstOrDefault(edge => edge.Type == edgeType);
		}

		public static NetworkEdge GetEdge(this IEnumerable<NetworkEdge> networkEdges, NetworkEdgeType edgeType, bool recursively)
		{
			NetworkEdge currentEdge = GetEdge(networkEdges, edgeType);

			if (currentEdge == null && recursively)
			{
				List<NetworkEdge> allEdges = new List<NetworkEdge>();

				GetAllEdges(networkEdges, allEdges);

				currentEdge = allEdges.FirstOrDefault(edge => edge.Type == edgeType);
			}

			return currentEdge;
		}

		public static IEnumerable<NetworkEdge> GetEdges(this IEnumerable<NetworkEdge> networkEdges, NetworkEdgeType edgeType)
		{
			return networkEdges.Where(edge => edge.Type == edgeType);
		}

		public static IEnumerable<NetworkEdge> GetEdges(this IEnumerable<NetworkEdge> networkEdges, NetworkEdgeType edgeType, bool recursively)
		{
			return networkEdges.Where(edge => edge.Type == edgeType);
		}
	}
}
