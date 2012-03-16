using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalesGenerator.Core.Collections
{
	public static class NetworkNodesExtension
	{
		public static NetworkNode GetNode(this IEnumerable<NetworkNode> networkNodes, string name)
		{
			return networkNodes.Where(node => node.Name.ToLower() == name.ToLower()).FirstOrDefault();
		}
	}
}
