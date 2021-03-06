﻿using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using TalesGenerator.Net.Serialization;
using TalesGenerator.Net.Collections;

namespace TalesGenerator.Net
{
	public enum NetworkEdgeType
	{
		IsA,
		PartOf,
		Agent,
		Recipient,
		Locative,
		Goal,
		Follow,
		IsInstance,
		Template,
		Action
	}

	/// <summary>
	/// Представляет дугу сети.
	/// </summary>
	public class NetworkEdge : NetworkObject
	{
		#region Fields

		private NetworkNode _startNode;

		private NetworkNode _endNode;

		private NetworkEdgeType _edgeType;
		#endregion

		#region Properties

		/// <summary>
		/// Возвращает или задает тип данной дуги.
		/// </summary>
		public NetworkEdgeType Type
		{
			get { return _edgeType; }

			set
			{
				if (_edgeType != value)
				{
					_edgeType = value;

					OnPropertyChanged("Type");
				}
			}
		}

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

		protected internal NetworkEdge(Network network)
			: base(network)
		{
		}

		/// <summary>
		/// Создает новую дугу сети.
		/// </summary>
		/// <param name="parent">Сеть, которой должна принадлежать дуга.</param>
		/// <param name="startNode">Вершина, из которой должна исходить данная дуга.</param>
		/// <param name="endNode">Вершина, в которую должна входить данная дуга.</param>
		protected internal NetworkEdge(Network parent, NetworkNode startNode, NetworkNode endNode)
			: this(parent, startNode, endNode, NetworkEdgeType.IsA)
		{
			
		}

		/// <summary>
		/// Создает новую дугу сети.
		/// </summary>
		/// <param name="parent">Сеть, которой должна принадлежать дуга.</param>
		/// <param name="startNode">Вершина, из которой должна исходить данная дуга.</param>
		/// <param name="endNode">Вершина, в которую должна входить данная дуга.</param>
		/// <param name="edgeType">Тип дуги.</param>
		protected internal NetworkEdge(Network parent, NetworkNode startNode, NetworkNode endNode, NetworkEdgeType edgeType)
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
			if (startNode == null)
			{
				throw new ArgumentNullException("startNode");
			}

			_startNode = startNode;
			_endNode = endNode;
			_edgeType = edgeType;
		}
		#endregion

		#region Methods

		public override XElement GetXml()
		{
			XNamespace xNamespace = Namespace;
			XElement xNetworkEdge = new XElement(xNamespace + "Edge");

			base.SaveToXml(xNetworkEdge);

			xNetworkEdge.Add(
				new XAttribute("type", _edgeType),
				new XAttribute("startNodeId", _startNode.Id),
				new XAttribute("endNodeId", _endNode.Id));

			return xNetworkEdge;
		}

		public override void SaveToXml(XElement xElement)
		{
			base.SaveToXml(xElement);
		}

		public override void LoadFromXml(XElement xNetworkEdge)
		{
			if (xNetworkEdge == null)
			{
				throw new ArgumentNullException("xNetworkEdge");
			}

			base.LoadFromXml(xNetworkEdge);

			XAttribute xEdgeTypeAttribute = xNetworkEdge.Attribute("type");
			XAttribute xStartNodeIdAttribute = xNetworkEdge.Attribute("startNodeId");
			XAttribute xEndNodeIdAttribute = xNetworkEdge.Attribute("endNodeId");

			if (xEdgeTypeAttribute == null ||
				xStartNodeIdAttribute == null ||
				xEndNodeIdAttribute == null)
			{
				throw new SerializationException();
			}

			NetworkEdgeType edgeType;
			int startNodeId;
			int endNodeId;

			if (!Enum.TryParse<NetworkEdgeType>(xEdgeTypeAttribute.Value, out edgeType) ||
				!Int32.TryParse(xStartNodeIdAttribute.Value, out startNodeId) ||
				!Int32.TryParse(xEndNodeIdAttribute.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out endNodeId))
			{
				throw new SerializationException();
			}

			_startNode = Network.Nodes.SingleOrDefault(node => node.Id == startNodeId);
			
			_endNode= Network.Nodes.SingleOrDefault(node => node.Id == endNodeId);
			_edgeType = edgeType;

			if (_startNode == null ||
				_endNode == null)
			{
				throw new SerializationException();
			}
		}

		public override string ToString()
		{
			return string.Format("Start node: \"{0}\". End node: \"{1}\". Type: {2}.", _startNode, _endNode, _edgeType);
		}
		#endregion
	}
}
