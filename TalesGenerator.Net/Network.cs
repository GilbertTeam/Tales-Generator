using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TalesGenerator.Net.Collections;
using TalesGenerator.Net.Serialization;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace TalesGenerator.Net
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

		protected bool _isDirty;
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
			XDocument xDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

			xDocument.Add(SaveToXElement());

			return xDocument;
		}

		private void LoadFromXDocument(XDocument xDocument)
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

			LoadFromXElement(xNetwork);
		}

		protected virtual void LoadFromXElement(XElement xNetwork)
		{
			XNamespace xNamespace = SerializableObject.XNamespace;

			XElement xNodesBase = xNetwork.Element(xNamespace + "Nodes");
			var xNodes = xNodesBase.Elements(xNamespace + "Node");
			foreach (XElement xNode in xNodes)
			{
				NetworkNode networkNode = new NetworkNode(this);

				networkNode.LoadFromXml(xNode);

				// TODO: Необходимо избавиться от этого костыля.
				if (!Nodes.Where(node => node.Id == networkNode.Id).Any())
				{
					Nodes.Add(networkNode);
				}
			}

			XElement xEdgesBase = xNetwork.Element(xNamespace + "Edges");
			var xEdges = xEdgesBase.Elements(xNamespace + "Edge");
			foreach (XElement xEdge in xEdges)
			{
				NetworkEdge networkEdge = new NetworkEdge(this);

				networkEdge.LoadFromXml(xEdge);

				// TODO: Необходимо избавиться от этого костыля.
				if (!Edges.Where(edge => edge.Id == networkEdge.Id).Any())
				{
					Edges.Add(networkEdge);
				}
			}

			SetId();

			_isDirty = false;
		}

		protected void SetId()
		{
			//TODO Необходимо доработать логику десериализации.
			if (_nodes.Count == 0)
			{
				_nextId = _edges.Max(edge => edge.Id) + 1;
			}
			else if (_edges.Count == 0)
			{
				_nextId = _nodes.Max(node => node.Id) + 1;
			}
			else
			{
				_nextId = Math.Max(Nodes.Max(node => node.Id), Edges.Max(edge => edge.Id)) + 1;
			}
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
		public void LoadFromXml(XElement xNetwork)
		{
			Contract.Requires<ArgumentNullException>(xNetwork != null);

			LoadFromXElement(xNetwork);
		}

		/// <summary>
		/// Загружает сеть из XML.
		/// </summary>
		/// <param name="xDocument">XML документ, содержащий представление сети.</param>
		/// <returns></returns>
		public void LoadFromXml(XDocument xDocument)
		{
			Contract.Requires<ArgumentNullException>(xDocument != null);

			LoadFromXDocument(xDocument);
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
		public void LoadFromFile(string path)
		{
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(path));

			XDocument xDocument = XDocument.Load(path);
			LoadFromXDocument(xDocument);
		}

		/// <summary>
		/// Загружает сеть из потока.
		/// </summary>
		/// <param name="stream">Поток, из которого должна быть загружена сеть.</param>
		/// <returns>Загруженная сеть.</returns>
		public void LoadFromStream(Stream stream)
		{
			Contract.Requires<ArgumentNullException>(stream != null);

			XDocument xDocument = XDocument.Load(stream);
			LoadFromXDocument(xDocument);
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
