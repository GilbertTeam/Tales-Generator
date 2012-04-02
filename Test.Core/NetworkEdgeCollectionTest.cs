using TalesGenerator.Core.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TalesGenerator.Core;

namespace Test.Core
{
	[TestClass()]
	public class NetworkEdgeCollectionTest
	{
		#region Fields

		private TestContext _testContextInstance;
		#endregion

		#region Properties

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
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		[TestMethod()]
		public void NetworkEdgeCollectionConstructorTest()
		{
			Network network = new Network();
			NetworkEdgeCollection target = new NetworkEdgeCollection(network);
		}

		[TestMethod()]
		public void AddEdgeWithoutTypeTest()
		{
			Network network = new Network();
			NetworkNode startNode = network.Nodes.Add();
			NetworkNode endNode = network.Nodes.Add();
			NetworkEdge newEdge = network.Edges.Add(startNode, endNode);

			Assert.AreEqual(NetworkEdgeType.IsA, newEdge.Type);
			Assert.AreEqual(endNode, startNode.BaseNode);
		}

		[TestMethod()]
		public void AddEdgeWithTypeTest()
		{
			Network network = new Network();
			NetworkEdgeCollection target = new NetworkEdgeCollection(network);
			NetworkNode startNode = network.Nodes.Add();
			NetworkNode endNode = network.Nodes.Add();
			NetworkEdge newEdge = target.Add(startNode, endNode, NetworkEdgeType.Agent);

			Assert.AreEqual(NetworkEdgeType.Agent, newEdge.Type);
			Assert.IsNull(startNode.BaseNode);
		}
		#endregion
	}
}
