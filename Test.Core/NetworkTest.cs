using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TalesGenerator.Core
{
	/// <summary>
	///Содержит модульные тесты класса TalesGenerator.Core.Network.
	///</summary>
	[TestClass()]
	public class NetworkTest
	{
		#region Fields

		/// <summary>
		/// Список имен файлов, используемых для тестирования механизма сериализации.
		/// </summary>
		private List<string> _fileNames;

		private TestContext _testContextInstance;
		#endregion

		#region Properties

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return _testContextInstance;
			}
			set
			{
				_testContextInstance = value;
			}
		}
		#endregion

		#region Methods

		private void CheckNetworkObjectId(NetworkObject networkObject)
		{
			var nodesWithSameId = networkObject.Network.Nodes.Where(node => node.Id == networkObject.Id).ToList();
			var edgesWithSameId = networkObject.Network.Edges.Where(edge => edge.Id == networkObject.Id).ToList();

			NetworkNode networkNode = networkObject as NetworkNode;
			if (networkNode != null)
			{
				Assert.AreEqual(1, nodesWithSameId.Count);
				Assert.AreEqual(networkNode, nodesWithSameId[0]);

				Assert.AreEqual(0, edgesWithSameId.Count);
			}

			NetworkEdge networkEdge = networkObject as NetworkEdge;
			if (networkEdge != null)
			{
				Assert.AreEqual(1, edgesWithSameId.Count);
				Assert.AreEqual(networkEdge, edgesWithSameId[0]);

				Assert.AreEqual(0, nodesWithSameId.Count);
			}
		}

		private void CheckNetworkNode(
			NetworkNode networkNode, Network network, string name,
			List<NetworkEdge> incomingEdges, List<NetworkEdge> outgoingEdges)
		{
			Assert.IsNotNull(networkNode);

			CheckNetworkObjectId(networkNode);

			Assert.AreEqual(network, networkNode.Network);
			Assert.AreEqual(name, networkNode.Name);

			Assert.AreEqual(incomingEdges.Count, networkNode.IncomingEdges.Count());
			for (int i = 0; i < incomingEdges.Count; i++)
			{
				Assert.AreEqual(incomingEdges[i], networkNode.IncomingEdges.ElementAt(i));
			}

			Assert.AreEqual(outgoingEdges.Count, networkNode.OutgoingEdges.Count());
			for (int i = 0; i < outgoingEdges.Count; i++)
			{
				Assert.AreEqual(outgoingEdges[i], networkNode.OutgoingEdges.ElementAt(i));
			}
		}

		private void CheckNetworkEdge(
			NetworkEdge networkEdge, Network network,
			NetworkNode startNode, NetworkNode endNode)
		{
			Assert.IsNotNull(networkEdge);

			CheckNetworkObjectId(networkEdge);

			Assert.AreEqual(network, networkEdge.Network);
			Assert.AreEqual(startNode, networkEdge.StartNode);
			Assert.AreEqual(endNode, networkEdge.EndNode);
		}

		private Network CreateNetwork()
		{
			Network network = new Network();

			NetworkNode node1 = network.Nodes.Add("Node 1");
			NetworkNode node2 = network.Nodes.Add("Node 2");
			NetworkNode node3 = network.Nodes.Add("Node 3");
			NetworkNode node4 = network.Nodes.Add("Node 4");
			NetworkNode node5 = network.Nodes.Add("Node 5");

			NetworkEdge edge12 = network.Edges.Add(node1, node2);
			NetworkEdge edge23 = network.Edges.Add(node2, node3);
			NetworkEdge edge34 = network.Edges.Add(node3, node4);
			NetworkEdge edge45 = network.Edges.Add(node4, node5);
			NetworkEdge edge51 = network.Edges.Add(node5, node1);

			return network;
		}

		private void CheckNetwork(Network network)
		{
			//Проверим общее количество элементов в коллекциях.
			Assert.AreEqual(5, network.Nodes.Count);
			Assert.AreEqual(5, network.Edges.Count);

			CheckNetworkNode(network.Nodes[0], network, "Node 1",
				new List<NetworkEdge> { network.Edges[4] }, new List<NetworkEdge> { network.Edges[0] });
			CheckNetworkNode(network.Nodes[1], network, "Node 2",
				new List<NetworkEdge> { network.Edges[0] }, new List<NetworkEdge> { network.Edges[1] });
			CheckNetworkNode(network.Nodes[2], network, "Node 3",
				new List<NetworkEdge> { network.Edges[1] }, new List<NetworkEdge> { network.Edges[2] });
			CheckNetworkNode(network.Nodes[3], network, "Node 4",
				new List<NetworkEdge> { network.Edges[2] }, new List<NetworkEdge> { network.Edges[3] });
			CheckNetworkNode(network.Nodes[4], network, "Node 5",
				new List<NetworkEdge> { network.Edges[3] }, new List<NetworkEdge> { network.Edges[4] });

			CheckNetworkEdge(network.Edges[0], network, network.Nodes[0], network.Nodes[1]);
			CheckNetworkEdge(network.Edges[1], network, network.Nodes[1], network.Nodes[2]);
			CheckNetworkEdge(network.Edges[2], network, network.Nodes[2], network.Nodes[3]);
			CheckNetworkEdge(network.Edges[3], network, network.Nodes[3], network.Nodes[4]);
			CheckNetworkEdge(network.Edges[4], network, network.Nodes[4], network.Nodes[0]);
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}

		[TestInitialize()]
		public void MyTestInitialize()
		{
			_fileNames = new List<string>();
		}
		
		[TestCleanup()]
		public void MyTestCleanup()
		{
			foreach (string fileName in _fileNames)
			{
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
			}
		}
		#endregion

		/// <summary>
		///A test for Network Constructor
		///</summary>
		[TestMethod()]
		public void NetworkConstructorTest()
		{
			Network network = new Network();

			Assert.IsNotNull(network.Nodes);
			Assert.AreEqual(0, network.Nodes.Count);

			Assert.IsNotNull(network.Edges);
			Assert.AreEqual(0, network.Edges.Count);
		}

		/// <summary>
		///A test for Nodes
		///</summary>
		[TestMethod()]
		public void NodesTest()
		{
			Network network = new Network();
			NetworkNode networkNode;
			List<NetworkNode> networkNodes;

			for (int i = 0; i < 5; i++)
			{
				networkNode = network.Nodes.Add();

				Assert.IsNotNull(networkNode);

				//Проверим, существует ли другие вершины с таким же индексом.
				networkNodes = network.Nodes.Where(node => node.Id == networkNode.Id).ToList();
				Assert.AreEqual(1, networkNodes.Count);
				Assert.AreEqual(networkNode, networkNodes[0]);
			}

			Assert.AreEqual(5, network.Nodes.Count);

			networkNode = network.Nodes[0];
			network.Nodes.RemoveAt(0);

			//Проверим, удалилась ли вершина.
			var nodesLikeAFirst = network.Nodes.Where(node => node == networkNode);
			Assert.AreEqual(0, nodesLikeAFirst.Count());
			Assert.AreEqual(4, network.Nodes.Count);

			networkNode = network.Nodes.Add();
			//Проверим, является ли индекс вновь добавленной вершины уникальным.
			networkNodes = network.Nodes.Where(node => node.Id == networkNode.Id).ToList();
			Assert.AreEqual(1, networkNodes.Count);
			Assert.AreEqual(networkNode, networkNodes[0]);
		}

		/// <summary>
		///A test for Edges
		///</summary>
		[TestMethod()]
		public void EdgesTest()
		{
			Network network = new Network();

			network.Nodes.Add();
			network.Nodes.Add();

			Assert.AreEqual(2, network.Nodes.Count);

			for (int i = 0; i < 5; i++)
			{
				network.Edges.Add(network.Nodes[0], network.Nodes[1]);
			}

			Assert.AreEqual(5, network.Edges.Count);

			network.Edges.RemoveAt(0);
			network.Edges.RemoveAt(1);

			Assert.AreEqual(3, network.Edges.Count);
		}

		/// <summary>
		///A test for SaveToFileAndLoadFromFileTest
		///</summary>
		[TestMethod()]
		public void SaveToFileAndLoadFromFileTest()
		{
			Network network = CreateNetwork();
			string fileName = Path.GetTempFileName();

			_fileNames.Add(fileName);

			network.Save(fileName);

			bool fileExists = File.Exists(fileName);

			Assert.AreEqual(true, fileExists);

			if (fileExists)
			{
				Network loadedNetwork = Network.Load(fileName);

				Assert.IsNotNull(loadedNetwork);

				if (loadedNetwork != null)
				{
					CheckNetwork(loadedNetwork);
				}
			}
		}

		[TestMethod()]
		public void SaveToFileAndLoadFromStream()
		{
			Network network = CreateNetwork();
			string fileName = Path.GetTempFileName();

			_fileNames.Add(fileName);

			network.Save(fileName);

			bool fileExists = File.Exists(fileName);

			Assert.AreEqual(true, fileExists);

			if (fileExists)
			{
				Network loadedNetwork;
				using (FileStream fileStream = File.Open(fileName, FileMode.Open))
				{
					loadedNetwork = Network.Load(fileStream);
				}

				Assert.IsNotNull(loadedNetwork);

				if (loadedNetwork != null)
				{
					CheckNetwork(loadedNetwork);
				}
			}
		}

		[TestMethod()]
		public void SaveToStreamAndLoadFromFile()
		{
			Network network = CreateNetwork();
			string fileName = Path.GetTempFileName();

			_fileNames.Add(fileName);
			
			using (FileStream fileStream = File.Open(fileName, FileMode.Create))
			{
				network.Save(fileStream);
			}
			bool fileExists = File.Exists(fileName);

			Assert.AreEqual(true, fileExists);

			if (fileExists)
			{
				Network loadedNetwork = Network.Load(fileName);

				Assert.IsNotNull(loadedNetwork);

				if (loadedNetwork != null)
				{
					CheckNetwork(loadedNetwork);
				}
			}
		}

		[TestMethod()]
		public void SaveToStreamAndLoadFromStream()
		{
			Network network = CreateNetwork();
			string fileName = Path.GetTempFileName();

			_fileNames.Add(fileName);

			using (FileStream fileStream = File.Open(fileName, FileMode.Create))
			{
				network.Save(fileStream);
			}
			bool fileExists = File.Exists(fileName);

			Assert.AreEqual(true, fileExists);

			if (fileExists)
			{
				Network loadedNetwork;
				using (FileStream fileStream = File.Open(fileName, FileMode.Open))
				{
					loadedNetwork = Network.Load(fileStream);
				}

				Assert.IsNotNull(loadedNetwork);

				if (loadedNetwork != null)
				{
					CheckNetwork(loadedNetwork);
				}
			}
		}
		#endregion
	}
}
