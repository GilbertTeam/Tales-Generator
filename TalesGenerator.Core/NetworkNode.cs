using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace TalesGenerator.Core
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
		/// Возвращает набор входящих дуг.
		/// </summary>
		public IEnumerable<NetworkEdge> IncomingEdges
		{
			get
			{
				return _network.Edges.Where(edge => edge.StartNode == this);
			}
		}

		/// <summary>
		/// Возвращает набор выходящих дуг.
		/// </summary>
		public IEnumerable<NetworkEdge> OutgoingEdges
		{
			get
			{
				return _network.Edges.Where(edge => edge.EndNode == this);
			}
		}
		#endregion

		#region Constructors

		/// <summary>
		/// Создает новую вершину сети.
		/// </summary>
		/// <param name="network">Сеть, которой принадлежит вершина.</param>
		public NetworkNode(Network network)
			: base(network)
		{
		}

		/// <summary>
		/// Создает новую вершину сети с заданным именем.
		/// </summary>
		/// <param name="network">Сеть, которой должна принадлежать вершина.</param>
		/// <param name="name">Имя вершины.</param>
		public NetworkNode(Network network, string name)
			: this(network)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			Name = name;
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

		public override string ToString()
		{
			return Name;
		}
		#endregion
	}
}
