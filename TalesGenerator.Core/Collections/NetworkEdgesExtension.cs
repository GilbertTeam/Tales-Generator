using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalesGenerator.Core.Collections
{
	public static class NetworkEdgesExtension
	{
		public static NetworkEdge GetEdge(this IEnumerable<NetworkEdge> networkEdges, NetworkEdgeType edgeType)
		{
			return networkEdges.FirstOrDefault(edge => edge.Type == edgeType);
		}
	}
}
