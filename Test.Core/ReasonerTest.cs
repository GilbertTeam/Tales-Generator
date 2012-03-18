using TalesGenerator.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Test.Core
{
	[TestClass()]
	public class ReasonerTest
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

		private Network CreateTestNetworkWithoutIsA()
		{
			Network network = new Network();
			NetworkNode hero = network.Nodes.Add("Герой");
			NetworkNode pest = network.Nodes.Add("Злодей");
			NetworkNode student = network.Nodes.Add("Студент");
			NetworkNode teacher = network.Nodes.Add("Преподаватель");
			NetworkNode gingerbreadMan = network.Nodes.Add("Колобок");
			NetworkNode fox = network.Nodes.Add("Лиса");
			NetworkNode wolf = network.Nodes.Add("Волк");
			NetworkNode verb = network.Nodes.Add("Глагол");
			NetworkNode meeting = network.Nodes.Add("Встречаться");
			NetworkNode meetingHeroWithPest = network.Nodes.Add("Встреча главного героя с вредителем");
			NetworkNode meetingStudentWithPest = network.Nodes.Add("Встреча студента с вредителем");
			NetworkNode meetingStudentWithTeacher = network.Nodes.Add("Встреча с преподавателем");
			NetworkNode meetingGingerbreadManWithFox = network.Nodes.Add("Встреча с лисой");
			NetworkNode meetingGingerbreadManWithWolf = network.Nodes.Add("Встреча с волком");

			network.Edges.Add(meetingHeroWithPest, pest, NetworkEdgeType.Recipient);
			network.Edges.Add(meetingHeroWithPest, hero, NetworkEdgeType.Agent);
			network.Edges.Add(meetingStudentWithPest, student, NetworkEdgeType.Agent);
			network.Edges.Add(meetingStudentWithTeacher, teacher, NetworkEdgeType.Recipient);
			network.Edges.Add(meetingGingerbreadManWithFox, gingerbreadMan, NetworkEdgeType.Agent);
			network.Edges.Add(meetingGingerbreadManWithFox, fox, NetworkEdgeType.Recipient);
			network.Edges.Add(meetingGingerbreadManWithWolf, gingerbreadMan, NetworkEdgeType.Agent);
			network.Edges.Add(meetingGingerbreadManWithWolf, wolf, NetworkEdgeType.Recipient);

			network.Edges.Add(meeting, verb);
			network.Edges.Add(meetingHeroWithPest, meeting);
			network.Edges.Add(meetingStudentWithPest, meetingHeroWithPest);
			network.Edges.Add(meetingStudentWithTeacher, meetingStudentWithPest);
			network.Edges.Add(meetingGingerbreadManWithFox, meetingStudentWithPest);
			network.Edges.Add(meetingGingerbreadManWithWolf, meetingStudentWithPest);

			return network;
		}

		private Network CreateTestNetworkWithIsA()
		{
			Network network = CreateTestNetworkWithoutIsA();
			NetworkNode hero = network.Nodes.Single(node => node.Name == "Герой");
			NetworkNode pest = network.Nodes.Single(node => node.Name == "Злодей");
			NetworkNode student = network.Nodes.Single(node => node.Name == "Студент");
			NetworkNode teacher = network.Nodes.Single(node => node.Name == "Преподаватель");

			network.Edges.Add(teacher, pest);
			network.Edges.Add(student, hero);

			return network;
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

		[TestMethod()]
		public void ReasonerConstructorTest()
		{
			Network network = new Network();
			Reasoner target = new Reasoner(network);
		}

		[TestMethod()]
		public void ConfirmTest()
		{
			List<Tuple<string, bool>> questions = new List<Tuple<string ,bool>>()
			{
				new Tuple<string, bool>("Студент встречаться преподаватель", true),
				new Tuple<string, bool>("Студент встречаться злодей", true),
				//new Tuple<string, bool>("Студент встречаться герой", false),
				//new Tuple<string, bool>("Студент встречаться студент", false),
				new Tuple<string, bool>("Герой встречаться преподаватель", true),
				new Tuple<string, bool>("Герой встречаться злодей", true),
				//new Tuple<string, bool>("Герой встречаться герой", false),
				//new Tuple<string, bool>("Герой встречаться студент", false),
				new Tuple<string, bool>("\"Студент\" \"встреча с преподавателем\" \"преподаватель\"", true),
				new Tuple<string, bool>("\"Студент\" \"встреча с преподавателем\" \"злодей\"", true),
				new Tuple<string, bool>("\"Герой\" \"встреча с преподавателем\" \"преподаватель\"", true),
				new Tuple<string, bool>("\"Герой\" \"встреча с преподавателем\" \"злодей\"", true),
				new Tuple<string, bool>("\"Герой\" \"встреча с лисой\" \"злодей\"", true)
			};
			Network networkWithoutIsA = CreateTestNetworkWithoutIsA();
			Reasoner reasoner = new Reasoner(networkWithoutIsA);

			foreach (var question in questions)
			{
				bool result = reasoner.Confirm(question.Item1);
				Assert.AreEqual(question.Item2, result);
			}

			//Network networkWithIsA = CreateTestNetworkWithIsA();
			//reasoner.Network = networkWithoutIsA;
		}
		#endregion
	}
}
