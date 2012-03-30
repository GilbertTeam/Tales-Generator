using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TalesGenerator.Core.Collections;
using TalesGenerator.Core.Serialization;
using System.Collections.Generic;

namespace TalesGenerator.Core
{
	/// <summary>
	/// Представляет сеть.
	/// </summary>
	public class Network
	{
		#region Fields

		/// <summary>
		/// Коллекция вершин сети.
		/// </summary>
		private readonly NetworkNodeCollection _nodes;

		/// <summary>
		/// Коллекция дуг сети.
		/// </summary>
		private readonly NetworkEdgeCollection _edges;

		/// <summary>
		/// Следующий свободный индекс объекта сети.
		/// </summary>
		private int _nextId;

		private bool _isDirty;
		#endregion

		#region Properties

		/// <summary>
		/// Возвращает идентификатор объекта.
		/// </summary>
		internal int NextId
		{
			get
			{
				return _nextId++;
			}
		}

		/// <summary>
		/// Возвращает коллекцию вершин сети.
		/// </summary>
		public NetworkNodeCollection Nodes
		{
			get { return _nodes; }
		}

		/// <summary>
		/// Возвращает коллекцию дуг сети.
		/// </summary>
		public NetworkEdgeCollection Edges
		{
			get { return _edges; }
		}

		/// <summary>
		/// Возвращает значение, определяющая имеет ли сеть несохраненные изменения.
		/// </summary>
		public bool IsDirty
		{
			get { return _isDirty; }
		}
		#endregion

		#region Constructors

		/// <summary>
		/// Создает новую сеть.
		/// </summary>
		public Network()
		{
			_nextId = 0;

			_nodes = new NetworkNodeCollection(this);
			_nodes.CollectionChanged += OnNetworkObjectCollectionChanged;
			_nodes.PropertyChanged += OnNetworkObjectPropertyChanged;

			_edges = new NetworkEdgeCollection(this);
			_edges.CollectionChanged += OnNetworkObjectCollectionChanged;
			_edges.PropertyChanged += OnNetworkObjectPropertyChanged;

			_isDirty = true;
		}
		#endregion

		#region Event Handlers

		private void OnNetworkObjectCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			_isDirty = true;
		}

		private void OnNetworkObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_isDirty = true;
		}
		#endregion

		#region Methods

		private XElement SaveToXElement()
		{
			XNamespace xNamespace = SerializableObject.XNamespace;
			XElement xNetwork = new XElement(xNamespace + "Network");

			XElement xNodes = new XElement(xNamespace + "Nodes");
			foreach (NetworkNode node in _nodes)
			{
				xNodes.Add(node.GetXml());
			}
			xNetwork.Add(xNodes);

			XElement xEdges = new XElement(xNamespace + "Edges");
			foreach (NetworkEdge edge in _edges)
			{
				xEdges.Add(edge.GetXml());
			}
			xNetwork.Add(xEdges);

			return xNetwork;
		}

		private XDocument SaveToXDocument()
		{
			XNamespace xNamespace = SerializableObject.XNamespace;
			XDocument xDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

			xDocument.Add(SaveToXElement());

			return xDocument;
		}

		private static Network LoadFromXElement(XElement xNetwork)
		{
			Network network = new Network();
			XNamespace xNamespace = SerializableObject.XNamespace;

			XElement xNodesBase = xNetwork.Element(xNamespace + "Nodes");
			var xNodes = xNodesBase.Elements(xNamespace + "Node");
			foreach (XElement xNode in xNodes)
			{
				NetworkNode networkNode = new NetworkNode(network);

				networkNode.LoadFromXml(xNode);

				network.Nodes.Add(networkNode);
			}

			XElement xEdgesBase = xNetwork.Element(xNamespace + "Edges");
			var xEdges = xEdgesBase.Elements(xNamespace + "Edge");
			foreach (XElement xEdge in xEdges)
			{
				NetworkEdge networkEdge = new NetworkEdge(network);

				networkEdge.LoadFromXml(xEdge);

				network.Edges.Add(networkEdge);
			}

			//TODO Необходимо доработать логику десериализации.
			if (network._nodes.Count == 0)
			{
				network._nextId = 0;
			}
			else if (network._edges.Count == 0)
			{
				network._nextId = network._nodes.Max(node => node.Id) + 1;
			}
			else
			{
				network._nextId = Math.Max(network.Nodes.Max(node => node.Id), network.Edges.Max(edge => edge.Id)) + 1;
			}

			network._isDirty = false;

			return network;
		}

		private static Network LoadFromXDocument(XDocument xDocument)
		{
			XNamespace xNamespace = SerializableObject.XNamespace;
			XElement xNetwork = xDocument.Element(xNamespace + "Network");
			if (xNetwork == null && xDocument.Root != null)
			{
				xNetwork = xDocument.Root.Element(xNamespace + "Network");
			}

			if (xNetwork == null)
			{
				throw new SerializationException();
			}

			Network network = LoadFromXElement(xNetwork);

			return network;
		}

		/// <summary>
		/// Сохраняет сеть в XML.
		/// </summary>
		/// <returns>XML представление сети.</returns>
		public XElement SaveToXml()
		{
			XElement xNetwork = SaveToXElement();

			return xNetwork;
		}

		/// <summary>
		/// Загружает сеть из XML.
		/// </summary>
		/// <param name="xNetwork">XML представление сети.</param>
		/// <returns>Загруженная сеть.</returns>
		public static Network LoadFromXml(XElement xNetwork)
		{
			if (xNetwork == null)
			{
				throw new ArgumentException("xElement");
			}

			Network network = LoadFromXElement(xNetwork);

			return network;
		}

		/// <summary>
		/// Загружает сеть из XML.
		/// </summary>
		/// <param name="xDocument">XML документ, содержащий представление сети.</param>
		/// <returns></returns>
		public static Network LoadFromXml(XDocument xDocument)
		{
			if (xDocument == null)
			{
				throw new ArgumentNullException("xDocument");
			}

			Network network = LoadFromXDocument(xDocument);

			return network;
		}

		/// <summary>
		/// Сохраняет сеть в файл.
		/// </summary>
		/// <param name="fileName">Имя файла, в который должна быть сохранена сеть.</param>
		public void SaveToFile(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentException("fileName");
			}

			XDocument xDocument = SaveToXDocument();
			xDocument.Save(fileName);

			_isDirty = false;
		}

		/// <summary>
		/// Сохраняет сеть в поток.
		/// </summary>
		/// <param name="stream">Поток, в который должна быть сохранена сеть.</param>
		public void SaveToStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}

			XDocument xDocument = SaveToXDocument();
			xDocument.Save(stream);

			_isDirty = false;
		}

		/// <summary>
		/// Загружает сеть из файла.
		/// </summary>
		/// <param name="path">Файл, из которого должна быть загружена сеть.</param>
		/// <returns>Загруженная сеть.</returns>
		public static Network LoadFromFile(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("path");
			}

			XDocument xDocument = XDocument.Load(path);
			Network network = LoadFromXDocument(xDocument);

			return network;
		}

		/// <summary>
		/// Загружает сеть из потока.
		/// </summary>
		/// <param name="stream">Поток, из которого должна быть загружена сеть.</param>
		/// <returns>Загруженная сеть.</returns>
		public static Network LoadFromStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}

			XDocument xDocument = XDocument.Load(stream);
			Network network = LoadFromXDocument(xDocument);

			return network;
		}

		public IEnumerable<NetworkNode> GetPrimaryNodes()
		{
			List<NetworkNode> result = new List<NetworkNode>();

			foreach (NetworkNode node in Nodes)
			{
				if (!node.HasTypedOutgoingEdges(NetworkEdgeType.IsInstance) &&
					!node.HasTypedOutgoingEdges(NetworkEdgeType.IsA))
					result.Add(node);
			}

			return result;
		}

		public NetworkObject FindById(int id)
		{
			NetworkObject obj = null;
			obj = Nodes.FindById(id);
			if (obj == null)
				obj = Edges.FindById(id);
			return obj;
		}

		#endregion
	}
}
