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

		private Network CreateNetworkWithNodesAndEdges()
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

		private Network CreateNetworkWithOnlyNodes()
		{
			Network network = new Network();

			NetworkNode node1 = network.Nodes.Add("Node 1");
			NetworkNode node2 = network.Nodes.Add("Node 2");
			NetworkNode node3 = network.Nodes.Add("Node 3");
			NetworkNode node4 = network.Nodes.Add("Node 4");
			NetworkNode node5 = network.Nodes.Add("Node 5");

			return network;
		}

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

		private void CheckNetworkWithNodesAndEdges(Network network)
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

		private void CheckNetworkWithOnlyNodes(Network network)
		{
			//Проверим общее количество элементов в коллекциях.
			Assert.AreEqual(5, network.Nodes.Count);

			CheckNetworkNode(network.Nodes[0], network, "Node 1",
				new List<NetworkEdge> { }, new List<NetworkEdge> { });
			CheckNetworkNode(network.Nodes[1], network, "Node 2",
				new List<NetworkEdge> { }, new List<NetworkEdge> { });
			CheckNetworkNode(network.Nodes[2], network, "Node 3",
				new List<NetworkEdge> { }, new List<NetworkEdge> { });
			CheckNetworkNode(network.Nodes[3], network, "Node 4",
				new List<NetworkEdge> { }, new List<NetworkEdge> { });
			CheckNetworkNode(network.Nodes[4], network, "Node 5",
				new List<NetworkEdge> { }, new List<NetworkEdge> { });
		}

		private Network SaveToFileAndLoadFromFile(Network network)
		{
			string fileName = Path.GetTempFileName();
			Network loadedNetwork = null;

			_fileNames.Add(fileName);

			network.SaveToFile(fileName);
			Assert.IsFalse(network.IsDirty);

			bool fileExists = File.Exists(fileName);

			Assert.AreEqual(true, fileExists);

			if (fileExists)
			{
				loadedNetwork = Network.LoadFromFile(fileName);
				Assert.IsFalse(loadedNetwork.IsDirty);
			}

			Assert.IsNotNull(loadedNetwork);

			return loadedNetwork;
		}

		private Network SaveToFileAndLoadFromStream(Network network)
		{
			string fileName = Path.GetTempFileName();
			Network loadedNetwork = null;

			_fileNames.Add(fileName);

			network.SaveToFile(fileName);
			Assert.IsFalse(network.IsDirty);

			bool fileExists = File.Exists(fileName);

			Assert.AreEqual(true, fileExists);

			if (fileExists)
			{
				using (FileStream fileStream = File.Open(fileName, FileMode.Open))
				{
					loadedNetwork = Network.LoadFromStream(fileStream);
				}

				Assert.IsFalse(loadedNetwork.IsDirty);
			}

			Assert.IsNotNull(loadedNetwork);

			return loadedNetwork;
		}

		private Network SaveToStreamAndLoadFromFile(Network network)
		{
			string fileName = Path.GetTempFileName();
			Network loadedNetwork = null;

			_fileNames.Add(fileName);

			using (FileStream fileStream = File.Open(fileName, FileMode.Create))
			{
				network.SaveToStream(fileStream);
			}
			Assert.IsFalse(network.IsDirty);

			bool fileExists = File.Exists(fileName);

			Assert.AreEqual(true, fileExists);

			if (fileExists)
			{
				loadedNetwork = Network.LoadFromFile(fileName);
				Assert.IsFalse(loadedNetwork.IsDirty);
			}

			Assert.IsNotNull(loadedNetwork);

			return loadedNetwork;
		}

		private Network SaveToStreamAndLoadFromStream(Network network)
		{
			string fileName = Path.GetTempFileName();
			Network loadedNetwork = null;

			_fileNames.Add(fileName);

			using (FileStream fileStream = File.Open(fileName, FileMode.Create))
			{
				network.SaveToStream(fileStream);
			}
			Assert.IsFalse(network.IsDirty);

			bool fileExists = File.Exists(fileName);

			Assert.AreEqual(true, fileExists);

			if (fileExists)
			{
				using (FileStream fileStream = File.Open(fileName, FileMode.Open))
				{
					loadedNetwork = Network.LoadFromStream(fileStream);
				}

				Assert.IsFalse(loadedNetwork.IsDirty);
			}

			Assert.IsNotNull(loadedNetwork);

			return loadedNetwork;
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

		[TestMethod()]
		public void NetworkConstructorTest()
		{
			Network network = new Network();

			Assert.IsNotNull(network.Nodes);
			Assert.AreEqual(0, network.Nodes.Count);

			Assert.IsNotNull(network.Edges);
			Assert.AreEqual(0, network.Edges.Count);
		}

		[TestMethod()]
		public void NodesTest()
		{
			Network network = new Network();
			NetworkNode networkNode;

			for (int i = 0; i < 5; i++)
			{
				networkNode = network.Nodes.Add();

				Assert.IsNotNull(networkNode);

				CheckNetworkObjectId(networkNode);
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
			CheckNetworkObjectId(networkNode);
		}

		[TestMethod()]
		public void EdgesTest()
		{
			Network network = new Network();
			NetworkEdge networkEdge;

			network.Nodes.Add();
			network.Nodes.Add();

			Assert.AreEqual(2, network.Nodes.Count);

			for (int i = 0; i < 5; i++)
			{
				networkEdge = network.Edges.Add(network.Nodes[0], network.Nodes[1]);

				Assert.IsNotNull(networkEdge);

				//Проверим, существует ли другие вершины или дуги с таким же индексом.
				CheckNetworkObjectId(networkEdge);
			}
			Assert.AreEqual(5, network.Edges.Count);

			network.Edges.RemoveAt(0);
			network.Edges.RemoveAt(1);
			Assert.AreEqual(3, network.Edges.Count);

			network.Nodes.RemoveAt(0);
			Assert.AreEqual(1, network.Nodes.Count);

			network.Nodes.Add();
			Assert.AreEqual(2, network.Nodes.Count);

			networkEdge = network.Edges.Add(network.Nodes[0], network.Nodes[1]);
			Assert.IsNotNull(networkEdge);
			CheckNetworkObjectId(networkEdge);
		}

		[TestMethod]
		public void IsDirtyTest()
		{
			Network network = new Network();

			Assert.IsTrue(network.IsDirty);

			NetworkNode networkNode = network.Nodes.Add();
			Assert.IsTrue(network.IsDirty);

			using (MemoryStream memoryStream = new MemoryStream())
			{
				network.SaveToStream(memoryStream);
			}
			Assert.IsFalse(network.IsDirty);

			networkNode.Name = "Node 1";
			Assert.IsTrue(network.IsDirty);

			using (MemoryStream memoryStream = new MemoryStream())
			{
				network.SaveToStream(memoryStream);
			}
			Assert.IsFalse(network.IsDirty);
		}

		[TestMethod()]
		public void SaveToFileAndLoadFromFileTest()
		{
			Network network = CreateNetworkWithNodesAndEdges();
			Network loadedNetwork = SaveToFileAndLoadFromFile(network);

			CheckNetworkWithNodesAndEdges(loadedNetwork);

			network = CreateNetworkWithOnlyNodes();
			loadedNetwork = SaveToFileAndLoadFromFile(network);

			CheckNetworkWithOnlyNodes(loadedNetwork);
		}

		[TestMethod()]
		public void SaveToFileAndLoadFromStreamTest()
		{
			Network network = CreateNetworkWithNodesAndEdges();
			Network loadedNetwork = SaveToFileAndLoadFromStream(network);

			CheckNetworkWithNodesAndEdges(loadedNetwork);

			network = CreateNetworkWithOnlyNodes();
			loadedNetwork = SaveToFileAndLoadFromStream(network);

			CheckNetworkWithOnlyNodes(loadedNetwork);
		}

		[TestMethod()]
		public void SaveToStreamAndLoadFromFileTest()
		{
			Network network = CreateNetworkWithNodesAndEdges();
			Network loadedNetwork = SaveToStreamAndLoadFromFile(network);

			CheckNetworkWithNodesAndEdges(loadedNetwork);

			network = CreateNetworkWithOnlyNodes();
			loadedNetwork = SaveToStreamAndLoadFromFile(network);

			CheckNetworkWithOnlyNodes(loadedNetwork);
		}

		[TestMethod()]
		public void SaveToStreamAndLoadFromStreamTest()
		{
			Network network = CreateNetworkWithNodesAndEdges();
			Network loadedNetwork = SaveToStreamAndLoadFromStream(network);

			CheckNetworkWithNodesAndEdges(loadedNetwork);

			network = CreateNetworkWithOnlyNodes();
			loadedNetwork = SaveToStreamAndLoadFromStream(network);

			CheckNetworkWithOnlyNodes(loadedNetwork);
		}
		#endregion
	}
}
