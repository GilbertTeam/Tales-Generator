using TalesGenerator.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test.Core
{
    
    
    /// <summary>
    ///This is a test class for NodeImplTest and is intended
    ///to contain all NodeImplTest Unit Tests
    ///</summary>
	[TestClass()]
	public class NodeImplTest
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


		/// <summary>
		///A test for NodeImpl Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("TalesGenerator.Core.dll")]
		public void NodeImplConstructorTest()
		{
			NodeImpl_Accessor target = new NodeImpl_Accessor();
			Assert.Inconclusive("TODO: Implement code to verify target");
		}
	}
}
