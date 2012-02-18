using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace TalesGenerator.Core
{
	public class SemanticNetwork
	{
		public IEnumerable<NetworkNode> Nodes { get; set; }

		public NetworkEdges Edges { get; set; }

		public static SemanticNetwork LoadFromXml(string xSemanticNetwork)
		{
			throw new NotImplementedException();
		}

		public string SaveToXml()
		{
			throw new NotImplementedException();

			XElement xSemanticNetwork = new XElement("Network");
			
			//foreach (SemanticNetworkNode 

		}
	}
}
