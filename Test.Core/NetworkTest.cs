using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TalesGenerator.Core;
using System.Collections.Generic;

namespace Test.Core
{
	/// <summary>
	///This is a test class for NetworkTest and is intended
	///to contain all NetworkTest Unit Tests
	///</summary>
	[TestClass()]
	public class NetworkTest
	{
		#region Fields

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

		private Network CreateNetwork()
		{
			Network network = new Network();
			NetworkNode node1 = new NetworkNode(network, "Node 1");
			NetworkNode node2 = new NetworkNode(network, "Node 2");
			NetworkNode node3 = new NetworkNode(network, "Node 3");
			NetworkNode node4 = new NetworkNode(network, "Node 4");
			NetworkNode node5 = new NetworkNode(network, "Node 5");

			network.Nodes.AddRange(new[] { node1, node2, node3, node4, node5 });

			NetworkEdge edge12 = new NetworkEdge(network, node1, node2);
			NetworkEdge edge23 = new NetworkEdge(network, node2, node3);
			NetworkEdge edge34 = new NetworkEdge(network, node3, node4);
			NetworkEdge edge45 = new NetworkEdge(network, node4, node5);
			NetworkEdge edge51 = new NetworkEdge(network, node5, node1);

			network.Edges.AddRange(new[] { edge12, edge23, edge34, edge45, edge51 });

			return network;
		}

		private void CheckNetwork(Network network)
		{
			//Проверим общее количество элементов в коллекциях.
			Assert.AreEqual(5, network.Nodes.Count);
			Assert.AreEqual(5, network.Edges.Count);

			//Проверим имена вершин.
			Assert.AreEqual("Node 1", network.Nodes[0]);
			Assert.AreEqual("Node 2", network.Nodes[1]);
			Assert.AreEqual("Node 3", network.Nodes[2]);
			Assert.AreEqual("Node 4", network.Nodes[3]);
			Assert.AreEqual("Node 5", network.Nodes[4]);

			//Проверим число входящих и исходящих дуг для каждой из вершин.
			Assert.AreEqual(1, network.Nodes[0].IncomingEdges.Count());
			Assert.AreEqual(1, network.Nodes[0].OutgoingEdges.Count());

			Assert.AreEqual(1, network.Nodes[1].IncomingEdges.Count());
			Assert.AreEqual(1, network.Nodes[1].OutgoingEdges.Count());

			Assert.AreEqual(1, network.Nodes[2].IncomingEdges.Count());
			Assert.AreEqual(1, network.Nodes[2].OutgoingEdges.Count());

			Assert.AreEqual(1, network.Nodes[3].IncomingEdges.Count());
			Assert.AreEqual(1, network.Nodes[3].OutgoingEdges.Count());

			Assert.AreEqual(1, network.Nodes[4].IncomingEdges.Count());
			Assert.AreEqual(1, network.Nodes[4].OutgoingEdges.Count());

			//Проверим входящие и исходящие вершины для дуг.
			Assert.AreEqual(network.Nodes[0], network.Edges[0].StartNode);
			Assert.AreEqual(network.Nodes[1], network.Edges[0].EndNode);

			Assert.AreEqual(network.Nodes[0], network.Edges[0].StartNode);
			Assert.AreEqual(network.Nodes[1], network.Edges[0].EndNode);

			Assert.AreEqual(network.Nodes[0], network.Edges[0].StartNode);
			Assert.AreEqual(network.Nodes[1], network.Edges[0].EndNode);

			Assert.AreEqual(network.Nodes[0], network.Edges[0].StartNode);
			Assert.AreEqual(network.Nodes[1], network.Edges[0].EndNode);

			Assert.AreEqual(network.Nodes[0], network.Edges[0].StartNode);
			Assert.AreEqual(network.Nodes[1], network.Edges[0].EndNode);
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
			Assert.IsNotNull(network.Edges);
		}

		/// <summary>
		///A test for Nodes
		///</summary>
		[TestMethod()]
		public void NodesTest()
		{
			Network network = new Network();

			for (int i = 0; i < 5; i++)
			{
				network.Nodes.Add(new NetworkNode(network));
			}

			Assert.AreEqual(5, network.Nodes.Count);

			network.Nodes.RemoveAt(0);
			network.Nodes.RemoveAt(1);

			Assert.AreEqual(3, network.Nodes.Count);
		}

		/// <summary>
		///A test for Edges
		///</summary>
		[TestMethod()]
		public void EdgesTest()
		{
			Network network = new Network();

			network.Nodes.Add(new NetworkNode(network));
			network.Nodes.Add(new NetworkNode(network));

			Assert.AreEqual(2, network.Nodes.Count);

			for (int i = 0; i < 5; i++)
			{
				network.Edges.Add(new NetworkEdge(network, network.Nodes[0], network.Nodes[1]));
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
