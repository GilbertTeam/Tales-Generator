using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using TalesGenerator.Net.Collections;

namespace TalesGenerator.Net
{
	/// <summary>
	/// Представляет вершину сети.
	/// </summary>
	public class NetworkNode : NetworkObject
	{
		#region Fields

		private string _name;
		#endregion

		#region Properties

		/// <summary>
		/// Возвращает или задает имя вершины.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;

				OnPropertyChanged("Name");
			}
		}

		/// <summary>
		/// Возвращает базовую вершину.
		/// </summary>
		public NetworkNode BaseNode
		{
			get
			{
				NetworkEdge isAEdge = OutgoingEdges.GetEdge(NetworkEdgeType.IsA);

				return isAEdge != null ? isAEdge.EndNode : null;
			}
			set
			{
				//TODO Необходимо добавить валидацию по типу вершины.

				if (value == null)
				{
					NetworkEdge isAEdge = OutgoingEdges.GetEdge(NetworkEdgeType.IsA);

					if (isAEdge != null)
					{
						Network.Edges.Remove(isAEdge);
					}
				}
				else
				{
					Network.Edges.Add(this, value);
				}
			}
		}

		public NetworkNode InstanceNode
		{
			get
			{
				NetworkEdge isInstanceEdge = OutgoingEdges.GetEdge(NetworkEdgeType.IsInstance);

				return isInstanceEdge != null ? isInstanceEdge.EndNode : null;
			}
		}

		/// <summary>
		/// Возвращает набор входящих дуг.
		/// </summary>
		public virtual IEnumerable<NetworkEdge> IncomingEdges
		{
			get
			{
				return Network.Edges.Where(edge => edge.EndNode == this);
			}
		}

		/// <summary>
		/// Возвращает набор выходящих дуг.
		/// </summary>
		public virtual IEnumerable<NetworkEdge> OutgoingEdges
		{
			get
			{
				return Network.Edges.Where(edge => edge.StartNode == this);
			}
		}
		#endregion

		#region Constructors

		/// <summary>
		/// Создает новую вершину сети.
		/// </summary>
		/// <param name="network">Сеть, которой принадлежит вершина.</param>
		protected internal NetworkNode(Network network)
			: base(network)
		{
			_name = string.Empty;
		}

		/// <summary>
		/// Создает новую вершину сети с заданным именем.
		/// </summary>
		/// <param name="network">Сеть, которой должна принадлежать вершина.</param>
		/// <param name="name">Имя новой вершины.</param>
		protected internal NetworkNode(Network network, string name)
			: base(network)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			_name = Regex.Replace(name, @"\s+", " ");
		}

		/// <summary>
		/// Создает новую вершину сети с заданной базовой вершиной.
		/// </summary>
		/// <param name="network">Сеть, которой должна принадлежать вершина.</param>
		/// <param name="baseNode">Базовая вершина.</param>
		protected internal NetworkNode(Network network, NetworkNode baseNode)
			: base(network)
		{
			if (baseNode == null)
			{
				throw new ArgumentNullException("baseNode");
			}

			BaseNode = baseNode;
		}

		/// <summary>
		/// Создает новую вершину сети с заданными именем и базовой вершиной.
		/// </summary>
		/// <param name="network">Сеть, которой должна принадлежать вершина.</param>
		/// <param name="name">Имя новой вершины.</param>
		/// <param name="baseNode">Базовая вершина.</param>
		protected internal NetworkNode(Network network, string name, NetworkNode baseNode)
			: this(network, name)
		{
			if (baseNode == null)
			{
				throw new ArgumentNullException("baseNode");
			}

			BaseNode = baseNode;
		}
		#endregion

		#region Methods

		internal override XElement GetXml()
		{
			XElement xNetworkNode = new XElement(XNamespace + "Node");

			base.SaveToXml(xNetworkNode);

			xNetworkNode.Add(new XAttribute("name", Name));

			return xNetworkNode;
		}

		internal override void SaveToXml(XElement xElement)
		{
			base.SaveToXml(xElement);
		}

		internal override void LoadFromXml(XElement xElement)
		{
			base.LoadFromXml(xElement);

			Name = xElement.Attribute("name").Value;
		}

		public bool IsInherit(NetworkNode networkNode, bool useIsInstance)
		{
			if (networkNode == null)
			{
				throw new ArgumentNullException("networkNode");
			}

			bool isInherit = false;

			if (networkNode == this)
			{
				isInherit = true;
			}
			else
			{
				NetworkNode baseNode = BaseNode;

				if (useIsInstance &&
					baseNode == null)
				{
					baseNode = InstanceNode;
				}

				while (!isInherit && baseNode != null)
				{
					if (baseNode == networkNode)
					{
						isInherit = true;
					}

					baseNode = baseNode.BaseNode;
				}
			}

			return isInherit;
		}

		public override string ToString()
		{
			return Name;
		}

		public IEnumerable<NetworkNode> GetTypedIncomingLinkedNodes(NetworkEdgeType type)
		{
			return GetTypedLinkedNodes(type, IncomingEdges);
		}

		public IEnumerable<NetworkNode> GetTypedOutgoingLinkedNodes(NetworkEdgeType type)
		{
			return GetTypedLinkedNodes(type, OutgoingEdges);
		}

		internal static IEnumerable<NetworkNode> GetTypedLinkedNodes(NetworkEdgeType type,
			IEnumerable<NetworkEdge> colletion)
		{
			List<NetworkNode> result = new List<NetworkNode>();

			foreach (NetworkEdge edge in colletion)
			{
				if (edge.Type == type)
					result.Add(edge.StartNode);
			}

			return result;
		}

		public bool HasTypedOutgoingEdges(NetworkEdgeType type)
		{
			return HasTypedEdges(type, OutgoingEdges);
		}

		public bool HasTypedIncomingEdges(NetworkEdgeType type)
		{
			return HasTypedEdges(type, IncomingEdges);
		}

		internal static bool HasTypedEdges(NetworkEdgeType type, IEnumerable<NetworkEdge> collection)
		{
			bool result = false;

			foreach (NetworkEdge edge in collection)
			{
				if (edge.Type == type)
					result = true;
			}

			return result;
		}

		public IEnumerable<NetworkEdge> GetTypedOutGoingEdges(NetworkEdgeType type)
		{
			return GetTypedEdges(type, OutgoingEdges);
		}

		public IEnumerable<NetworkEdge> GetTypedIncomingEdges(NetworkEdgeType type)
		{
			return GetTypedEdges(type, IncomingEdges);
		}

		internal static IEnumerable<NetworkEdge> GetTypedEdges(NetworkEdgeType type, IEnumerable<NetworkEdge> edges)
		{
			List<NetworkEdge> result = new List<NetworkEdge>();

			foreach (NetworkEdge edge in edges)
			{
				if (edge.Type == type)
					result.Add(edge);
			}

			return result;
		}

		public NetworkEdge GetTypedOutgoingEdge(NetworkEdgeType type)
		{
			return GetTypedEdge(type, OutgoingEdges);
		}

		public NetworkEdge GetTypedIncomingEdge(NetworkEdgeType type)
		{
			return GetTypedEdge(type, IncomingEdges);
		}

		internal static NetworkEdge GetTypedEdge(NetworkEdgeType type, IEnumerable<NetworkEdge> edges)
		{
			NetworkEdge result = null;

			foreach(NetworkEdge edge in edges)
			{
				if (type == edge.Type)
				{
					result = edge;
					break;
				}
			}

			return result;
		}
		#endregion
	}
}
