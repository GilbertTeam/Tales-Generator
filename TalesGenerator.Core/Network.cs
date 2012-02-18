using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TalesGenerator.Core
{
	public class Network : SerializableObject
	{
		#region Fields

		private readonly NetworkNodeCollection _nodes = new NetworkNodeCollection();

		private readonly NetworkEdgeCollection _edges = new NetworkEdgeCollection();
		#endregion

		#region Properties

		public NetworkNodeCollection Nodes
		{
			get { return _nodes; }
		}

		public NetworkEdgeCollection Edges
		{
			get { return _edges; }
		}
		#endregion

		#region Constructors

		public Network()
		{

		}
		#endregion

		#region Methods

		internal override void SaveToXml(XElement xElement)
		{
			
		}

		public string SaveToXml()
		{
			XNamespace xNamespace = Namespace;
			XElement xNetwork = new XElement(xNamespace + "Network");

			XElement xNodes = new XElement(xNamespace + "Nodes");
			foreach (NetworkNode node in _nodes)
			{
				xNodes.Add(node.SaveToXml());
			}

			XElement xEdges = new XElement(xNamespace + "Edges");
			foreach (NetworkEdge edge in _edges)
			{
				xEdges.Add(edge);
			}

			return xNetwork.ToString();
		}

		public static Network LoadFromXml(string str)
		{
			Network network = new Network();

			XElement xNetwork = XElement.Parse(str);
			XElement xNodes = xNetwork.Element(XNamespace + "Nodes");

			foreach (XElement xNode in xNodes.Elements(XNamespace + "Node"))
			{
				NetworkNode networkNode = new NetworkNode(network);

				networkNode.LoadFromXml(xNode);
			}
			return network;
		}
		#endregion
	}
}
