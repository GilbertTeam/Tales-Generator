﻿using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using TalesGenerator.Core.Serialization;

namespace TalesGenerator.Core
{
	public enum NetworkEdgeType
	{
		IsA,
		Agent,
		Recipient
	}

	/// <summary>
	/// Представляет дугу сети.
	/// </summary>
	public class NetworkEdge : NetworkObject
	{
		#region Fields

		private NetworkNode _startNode;

		private NetworkNode _endNode;
		#endregion

		#region Properties

		/// <summary>
		/// Возвращает или задает тип данной дуги.
		/// </summary>
		public NetworkEdgeType Type { get; set; }

		/// <summary>
		/// Возвращает или задает вершину сети, из которой исходит данная дуга.
		/// </summary>
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

		/// <summary>
		/// Возвращает или задает вершину сети, в которую входит данная дуга.
		/// </summary>
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

		internal NetworkEdge(Network network)
			: base(network)
		{
		}

		/// <summary>
		/// Создает новую дугу сети.
		/// </summary>
		/// <param name="parent">Сеть, которой должна принадлежать дуга.</param>
		/// <param name="startNode">Вершина, из которой должна исходить данная дуга.</param>
		/// <param name="endNode">Вершина, в которую должна входить данная дуга.</param>
		internal NetworkEdge(Network parent, NetworkNode startNode, NetworkNode endNode)
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

		internal override XElement GetXml()
		{
			XNamespace xNamespace = Namespace;
			XElement xNetworkEdge = new XElement(xNamespace + "Edge");

			base.SaveToXml(xNetworkEdge);

			xNetworkEdge.Add(
				new XAttribute("startNodeId", _startNode.Id),
				new XAttribute("endNodeId", _endNode.Id));

			return xNetworkEdge;
		}

		internal override void SaveToXml(XElement xElement)
		{
			base.SaveToXml(xElement);
		}

		internal override void LoadFromXml(XElement xNetworkEdge)
		{
			if (xNetworkEdge == null)
			{
				throw new ArgumentNullException("xNetworkEdge");
			}

			base.LoadFromXml(xNetworkEdge);

			XAttribute xStartNodeIdAttribute = xNetworkEdge.Attribute("startNodeId");
			XAttribute xEndNodeIdAttribute = xNetworkEdge.Attribute("endNodeId");

			if (xStartNodeIdAttribute == null ||
				xEndNodeIdAttribute == null)
			{
				throw new SerializationException();
			}

			int startNodeId;
			int endNodeId;

			if (!int.TryParse(xStartNodeIdAttribute.Value, out startNodeId) ||
				!int.TryParse(xEndNodeIdAttribute.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out endNodeId))
			{
				throw new SerializationException();
			}

			_startNode = _network.Nodes.SingleOrDefault(node => node.Id == startNodeId);
			_endNode= _network.Nodes.SingleOrDefault(node => node.Id == endNodeId);

			if (_startNode == null ||
				_endNode == null)
			{
				throw new SerializationException();
			}
		}
		#endregion
	}
}
