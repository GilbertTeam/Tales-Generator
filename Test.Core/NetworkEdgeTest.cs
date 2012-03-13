using TalesGenerator.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;
using System.IO;

namespace Test.Core
{
	[TestClass()]
	public class NetworkEdgeTest
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

		//private NetworkEdge CreateNetworkEdge()
		//{

		//}

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
		public void NetworkEdgeConstructorTest1()
		{
			Network network = new Network();
			NetworkEdge networkEdge = new NetworkEdge(network);

			Assert.AreEqual(network, networkEdge.Network);
			Assert.AreEqual(NetworkEdgeType.IsA, networkEdge.Type);
			Assert.IsNull(networkEdge.StartNode);
			Assert.IsNull(networkEdge.EndNode);
		}

		[TestMethod()]
		public void NetworkEdgeConstructorTest2()
		{
			Network network = new Network();
			NetworkNode networkNode1 = network.Nodes.Add();
			NetworkNode networkNode2 = network.Nodes.Add();

			NetworkEdge networkEdge = new NetworkEdge(network, networkNode1, networkNode2);
			Assert.AreEqual(NetworkEdgeType.IsA, networkEdge.Type);
			Assert.AreEqual(network, networkEdge.Network);
			Assert.AreEqual(networkNode1, networkEdge.StartNode);
			Assert.AreEqual(networkNode2, networkEdge.EndNode);
		}

		//[TestMethod()]
		//public void GetXmlTest()
		//{
		//    Network network = null; // TODO: Initialize to an appropriate value
		//    NetworkEdge target = new NetworkEdge(network); // TODO: Initialize to an appropriate value
		//    XElement expected = null; // TODO: Initialize to an appropriate value
		//    XElement actual;
		//    actual = target.GetXml();
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		//[TestMethod()]
		//public void LoadFromXmlTest()
		//{
		//    Network network = null; // TODO: Initialize to an appropriate value
		//    NetworkEdge target = new NetworkEdge(network); // TODO: Initialize to an appropriate value
		//    XElement xNetworkEdge = null; // TODO: Initialize to an appropriate value
		//    target.LoadFromXml(xNetworkEdge);
		//    Assert.Inconclusive("A method that does not return a value cannot be verified.");
		//}

		//[TestMethod()]
		//public void SaveToXmlTest()
		//{
		//    Network network = null; // TODO: Initialize to an appropriate value
		//    NetworkEdge target = new NetworkEdge(network); // TODO: Initialize to an appropriate value
		//    XElement xElement = null; // TODO: Initialize to an appropriate value
		//    target.SaveToXml(xElement);
		//    Assert.Inconclusive("A method that does not return a value cannot be verified.");
		//}

		//[TestMethod()]
		//public void EndNodeTest()
		//{
		//    Network network = null; // TODO: Initialize to an appropriate value
		//    NetworkEdge target = new NetworkEdge(network); // TODO: Initialize to an appropriate value
		//    NetworkNode expected = null; // TODO: Initialize to an appropriate value
		//    NetworkNode actual;
		//    target.EndNode = expected;
		//    actual = target.EndNode;
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		//[TestMethod()]
		//public void StartNodeTest()
		//{
		//    Network network = null; // TODO: Initialize to an appropriate value
		//    NetworkEdge target = new NetworkEdge(network); // TODO: Initialize to an appropriate value
		//    NetworkNode expected = null; // TODO: Initialize to an appropriate value
		//    NetworkNode actual;
		//    target.StartNode = expected;
		//    actual = target.StartNode;
		//    Assert.AreEqual(expected, actual);
		//    Assert.Inconclusive("Verify the correctness of this test method.");
		//}

		[TestMethod()]
		public void TypeTest()
		{
			Network network = new Network();

			Assert.IsTrue(network.IsDirty);

			network.Nodes.Add();
			network.Nodes.Add();

			Assert.IsTrue(network.IsDirty);

			NetworkEdge networkEdge = network.Edges.Add(network.Nodes[0], network.Nodes[1]);
			Assert.AreEqual(NetworkEdgeType.IsA, networkEdge.Type);

			using (MemoryStream memoryStream = new MemoryStream())
			{
				network.SaveToStream(memoryStream);
			}

			Assert.IsFalse(network.IsDirty);
			networkEdge.Type = NetworkEdgeType.Agent;
			Assert.IsTrue(network.IsDirty);
			Assert.AreEqual(NetworkEdgeType.Agent, networkEdge.Type);
		}
		#endregion
	}
}
