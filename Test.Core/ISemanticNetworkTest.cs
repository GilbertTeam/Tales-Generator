using TalesGenerator.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Test.Core
{
    
    
    /// <summary>
    ///This is a test class for ISemanticNetworkTest and is intended
    ///to contain all ISemanticNetworkTest Unit Tests
    ///</summary>
	[TestClass()]
	public class ISemanticNetworkTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
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


		internal virtual ISemanticNetwork CreateISemanticNetwork()
		{
			// TODO: Instantiate an appropriate concrete class.
			ISemanticNetwork target = null;
			return target;
		}

		/// <summary>
		///A test for Load
		///</summary>
		[TestMethod()]
		public void LoadTest()
		{
			ISemanticNetwork target = CreateISemanticNetwork(); // TODO: Initialize to an appropriate value
			Stream reader = null; // TODO: Initialize to an appropriate value
			target.Load(reader);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Load
		///</summary>
		[TestMethod()]
		public void LoadTest1()
		{
			ISemanticNetwork target = CreateISemanticNetwork(); // TODO: Initialize to an appropriate value
			string path = string.Empty; // TODO: Initialize to an appropriate value
			target.Load(path);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Redo
		///</summary>
		[TestMethod()]
		public void RedoTest()
		{
			ISemanticNetwork target = CreateISemanticNetwork(); // TODO: Initialize to an appropriate value
			target.Redo();
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Save
		///</summary>
		[TestMethod()]
		public void SaveTest()
		{
			ISemanticNetwork target = CreateISemanticNetwork(); // TODO: Initialize to an appropriate value
			string path = string.Empty; // TODO: Initialize to an appropriate value
			target.Save(path);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Save
		///</summary>
		[TestMethod()]
		public void SaveTest1()
		{
			ISemanticNetwork target = CreateISemanticNetwork(); // TODO: Initialize to an appropriate value
			Stream writer = null; // TODO: Initialize to an appropriate value
			target.Save(writer);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Undo
		///</summary>
		[TestMethod()]
		public void UndoTest()
		{
			ISemanticNetwork target = CreateISemanticNetwork(); // TODO: Initialize to an appropriate value
			target.Undo();
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}

		/// <summary>
		///A test for Dirty
		///</summary>
		[TestMethod()]
		public void DirtyTest()
		{
			ISemanticNetwork target = CreateISemanticNetwork(); // TODO: Initialize to an appropriate value
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
		public void EdgesTest()
		{
			ISemanticNetwork target = CreateISemanticNetwork(); // TODO: Initialize to an appropriate value
			IEdges actual;
			actual = target.Edges;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Name
		///</summary>
		[TestMethod()]
		public void NameTest()
		{
			ISemanticNetwork target = CreateISemanticNetwork(); // TODO: Initialize to an appropriate value
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
		public void NodesTest()
		{
			ISemanticNetwork target = CreateISemanticNetwork(); // TODO: Initialize to an appropriate value
			INodes actual;
			actual = target.Nodes;
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
	}
}
