using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TalesGenerator.Core.Collections;
using TalesGenerator.Core.Serialization;

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
		/// Следующий используемый индекс объекта сети.
		/// </summary>
		private int _nextId;
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
		#endregion

		#region Constructors

		/// <summary>
		/// Создает новую сеть.
		/// </summary>
		public Network()
		{
			_nextId = 0;

			_nodes = new NetworkNodeCollection(this);

			_edges = new NetworkEdgeCollection(this);
		}
		#endregion

		#region Methods

		private XDocument SaveToXml()
		{
			XNamespace xNamespace = SerializableObject.XNamespace;
			XDocument xDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
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

			xDocument.AddFirst(xNetwork);

			return xDocument;
		}

		private static Network LoadFromXml(XDocument xDocument)
		{
			Network network = new Network();

			XNamespace xNamespace = SerializableObject.XNamespace;
			XElement xNetwork = xDocument.Root;

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
			network._nextId = Math.Max(network.Nodes.Select(node => node.Id).Max(), network.Edges.Select(edge => edge.Id).Max()) + 1;

			return network;
		}

		/// <summary>
		/// Сохраняет сеть в файл.
		/// </summary>
		/// <param name="fileName">Имя файла, в который должна быть сохранена сеть.</param>
		public void Save(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentException("fileName");
			}

			XDocument xDocument = SaveToXml();
			xDocument.Save(fileName);
		}

		/// <summary>
		/// Сохраняет сеть в поток.
		/// </summary>
		/// <param name="stream">Поток, в который должна быть сохранена сеть.</param>
		public void Save(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}

			XDocument xDocument = SaveToXml();
			xDocument.Save(stream);
		}

		/// <summary>
		/// Загружает сеть из файла.
		/// </summary>
		/// <param name="path">Файл, из которого должна быть загружена сеть.</param>
		/// <returns>Загруженная сеть.</returns>
		public static Network Load(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("path");
			}

			XDocument xDocument = XDocument.Load(path);

			return LoadFromXml(xDocument);
		}

		/// <summary>
		/// Загружает сеть из потока.
		/// </summary>
		/// <param name="stream">Поток, из которого должна быть загружена сеть.</param>
		/// <returns>Загруженная сеть.</returns>
		public static Network Load(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}

			XDocument xDocument = XDocument.Load(stream);

			return LoadFromXml(xDocument);
		}
		#endregion
	}
}
