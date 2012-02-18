using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TalesGenerator.Core
{
	public class NetworkEdge : NetworkObject
	{
		#region Fields

		private NetworkNode _startNode;

		private NetworkNode _endNode;
		#endregion

		#region Properties

		public NetworkNode StartNode
		{
			get { return _startNode; }

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_startNode = value;

				OnPropertyChanged("StartNode");
			}
		}

		public NetworkNode EndNode
		{
			get { return _endNode; }

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_endNode = value;

				OnPropertyChanged("EndNode");
			}
		}
		#endregion

		#region Constructors

		public NetworkEdge(Network parent, NetworkNode startNode, NetworkNode endNode)
			: base(parent)
		{
			if (startNode == null)
			{
				throw new ArgumentNullException("startNode");
			}
			if (endNode == null)
			{
				throw new ArgumentNullException("endNode");
			}

			_startNode = startNode;
			_endNode = endNode;
		}
		#endregion

		#region Methods

		internal override void SaveToXml(XElement xElement)
		{
			base.SaveToXml(xElement);
		}

		internal override void LoadFromXml(XElement xElement)
		{
			base.LoadFromXml(xElement);

			_startNode = _network.Nodes.Single(node => node.Id == xElement.Attribute("startNode").Value);
			_endNode= _network.Nodes.Single(node => node.Id == xElement.Attribute("endNode").Value);
		}

		internal override XElement GetXml()
		{
			XNamespace xNamespace = Namespace;
			XElement xNetworkEdge = new XElement(xNamespace + "NetworkEdge");

			base.SaveToXml(xNetworkEdge);

			xNetworkEdge.Add(
				new XAttribute("startNode", _startNode.Id),
				new XAttribute("endNode", _endNode.Id));

			return xNetworkEdge;
		}
		#endregion
	}
}
