using TalesGenerator.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Test.Core
{
	/// <summary>
	///This is a test class for SemanticNetworkImplTest and is intended
	///to contain all SemanticNetworkImplTest Unit Tests
	///</summary>
	[TestClass()]
	public class SemanticNetworkImplTest
	{
		#region Fields

		private TestContext _testContextInstance;
		#endregion

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


		/// <summary>
		///A test for SemanticNetworkImpl Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void SemanticNetworkImplConstructorTest()
		{
			SemanticNetworkImpl_Accessor target = new SemanticNetworkImpl_Accessor();
			Assert.Inconclusive("TODO: Implement code to verify target");
		}

		/// <summary>
		///A test for Load
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void LoadTest()
		{
			SemanticNetworkImpl_Accessor target = new SemanticNetworkImpl_Accessor(); // TODO: Initialize to an appropriate value
			Stream reader = null; // TODO: Initialize to an appropriate value
			target.Load(reader);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Load
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void LoadTest1()
		{
			SemanticNetworkImpl_Accessor target = new SemanticNetworkImpl_Accessor(); // TODO: Initialize to an appropriate value
			string path = string.Empty; // TODO: Initialize to an appropriate value
			target.Load(path);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Redo
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void RedoTest()
		{
			SemanticNetworkImpl_Accessor target = new SemanticNetworkImpl_Accessor(); // TODO: Initialize to an appropriate value
			target.Redo();
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Save
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void SaveTest()
		{
			SemanticNetworkImpl_Accessor target = new SemanticNetworkImpl_Accessor(); // TODO: Initialize to an appropriate value
			Stream writer = null; // TODO: Initialize to an appropriate value
			target.Save(writer);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Save
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void SaveTest1()
		{
			SemanticNetworkImpl_Accessor target = new SemanticNetworkImpl_Accessor(); // TODO: Initialize to an appropriate value
			string path = string.Empty; // TODO: Initialize to an appropriate value
			target.Save(path);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Undo
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void UndoTest()
		{
			SemanticNetworkImpl_Accessor target = new SemanticNetworkImpl_Accessor(); // TODO: Initialize to an appropriate value
			target.Undo();
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Dirty
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void DirtyTest()
		{
			SemanticNetworkImpl_Accessor target = new SemanticNetworkImpl_Accessor(); // TODO: Initialize to an appropriate value
			bool expected = false; // TODO: Initialize to an appropriate value
			bool actual;
			target.Dirty = expected;
			actual = target.Dirty;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Edges
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void EdgesTest()
		{
			SemanticNetworkImpl_Accessor target = new SemanticNetworkImpl_Accessor(); // TODO: Initialize to an appropriate value
			IEdges actual;
			actual = target.Edges;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Name
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void NameTest()
		{
			SemanticNetworkImpl_Accessor target = new SemanticNetworkImpl_Accessor(); // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			target.Name = expected;
			actual = target.Name;
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Nodes
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void NodesTest()
		{
			SemanticNetworkImpl_Accessor target = new SemanticNetworkImpl_Accessor(); // TODO: Initialize to an appropriate value
			INodes actual;
			actual = target.Nodes;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
	}
}
