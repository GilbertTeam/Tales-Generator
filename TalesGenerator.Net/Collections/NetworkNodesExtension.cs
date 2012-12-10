using System;
using System.Collections.Generic;
using System.Linq;

namespace TalesGenerator.Net.Collections
{
	public static class NetworkNodesExtension
	{
		public static NetworkNode GetNode(this IEnumerable<NetworkNode> networkNodes, string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}

			string temp = name.ToLower().Trim();

			return networkNodes.Where(node => node.Name.ToLower() == temp).FirstOrDefault();
		}
	}
}
