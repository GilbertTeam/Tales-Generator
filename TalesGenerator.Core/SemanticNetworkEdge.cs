using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalesGenerator.Core
{
	public class SemanticNetworkEdge : NetworkObject
	{
		#region Properties

		public NetworkNode StartNode { get; set; }

		public NetworkNode EndNode { get; set; }
		#endregion

		#region Constructors

		public SemanticNetworkEdge()
		{
		}

		public SemanticNetworkEdge(NetworkNode startNode, NetworkNode endNode)
		{
			if (startNode == null)
			{
				throw new ArgumentNullException("startNode");
			}
			if (endNode == null)
			{
				throw new ArgumentNullException("endNode");
			}

			StartNode = startNode;
			EndNode = endNode;
		}
		#endregion
	}
}
