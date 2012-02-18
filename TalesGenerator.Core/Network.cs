using System;
using System.IO;
using System.Xml.Linq;

namespace TalesGenerator.Core
{
	/// <summary>
	/// Представляет сеть.
	/// </summary>
	public class Network
	{
		#region Fields

		private readonly NetworkNodeCollection _nodes = new NetworkNodeCollection();

		private readonly NetworkEdgeCollection _edges = new NetworkEdgeCollection();
		#endregion

		#region Properties

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

		//TODO Дописать метод загруки сети.
		private static Network LoadFromXml(XDocument xDocument)
		{
			Network network = new Network();

			XNamespace xNamespace = SerializableObject.XNamespace;
			XElement xNetwork = xDocument.Root;
			XElement xNodes = xNetwork.Element(xNamespace + "Nodes");

			foreach (XElement xNode in xNodes.Elements(xNamespace + "Node"))
			{
				NetworkNode networkNode = new NetworkNode(network);

				networkNode.LoadFromXml(xNode);
			}
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
