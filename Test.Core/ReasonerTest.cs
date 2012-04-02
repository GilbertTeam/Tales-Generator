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

		private const string yesAnswer = "Да";
		private const string noAnswer = "Нет";
		private const string dontKnowAnswer = "Не знаю";

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

		private Network LoadNetwork()
		{
			return Network.LoadFromFile(@"c:\Projects\Git\TalesGenerator\TestNetwork.tgp");
		}

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

		private void TestQueries(List<Tuple<string, string>> questions)
		{
			Network network = LoadNetwork();
			Reasoner reasoner = new Reasoner(network);
			int index = 0;

			foreach (var question in questions)
			{
				index++;
				string result = reasoner.Confirm(question.Item1);
				Assert.AreEqual(
					question.Item2,
					result,
					string.Format("Problem query ({0}): {1}.", index, question.Item1));
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

		[TestMethod()]
		public void ReasonerConstructorTest()
		{
			Network network = new Network();
			Reasoner target = new Reasoner(network);
		}

		[TestMethod()]
		public void WhoQueriesTest()
		{
			List<Tuple<string, string>> questions = new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("Кто персонаж", "Персонаж"),
				new Tuple<string, string>("Кто злодей", "Персонаж"),
				new Tuple<string, string>("Кто преподаватель", "Злодей"),
				new Tuple<string, string>("Кто Чуприна Светлана Игоревна", "Преподаватель"),
				new Tuple<string, string>("Кто Светлана Игоревна", dontKnowAnswer),
				new Tuple<string, string>("Кто Юрков Кирилл Александрович ", "Преподаватель"),
				new Tuple<string, string>("Кто Кирилл Александрович", dontKnowAnswer),
				new Tuple<string, string>("Кто глагол", "Глагол"),
				new Tuple<string, string>("Кто глагол123", dontKnowAnswer),
				new Tuple<string, string>("Кто глагол 123", "Глагол"),
				new Tuple<string, string>("Кто победа злодея над главным героем", "Победить"),
				new Tuple<string, string>("Кто получить зачет", "Глагол")
			};

			TestQueries(questions);
		}

		[TestMethod()]
		public void IsQueriesTest()
		{
			List<Tuple<string, string>> questions = new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("Персонаж это персонаж", yesAnswer),
				new Tuple<string, string>("Персонаж123 это персонаж", dontKnowAnswer),
				new Tuple<string, string>("Персонаж 123 это персонаж ", yesAnswer),
				new Tuple<string, string>("Персонаж 123 это персонаж 123", yesAnswer),
				new Tuple<string, string>("Персонаж это преподаватель", noAnswer),
				new Tuple<string, string>("Персонаж это студент", noAnswer),
				
				new Tuple<string, string>("Злодей это злодей", yesAnswer),
				new Tuple<string, string>("Злодей это персонаж", yesAnswer),
				new Tuple<string, string>("Злодей это герой", noAnswer),

				new Tuple<string, string>("Преподаватель это преподаватель", yesAnswer),
				new Tuple<string, string>("Преподаватель это злодей", yesAnswer),
				new Tuple<string, string>("Преподаватель это персонаж", yesAnswer),

				new Tuple<string, string>("Чуприна Светлана Игоревна это преподаватель", yesAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна это злодей", yesAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна это персонаж", yesAnswer),

				new Tuple<string, string>("Юрков Кирилл Александрович это преподаватель", yesAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович это злодей", yesAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович это персонаж", yesAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович это Чуприна Светлана Игоревна", noAnswer),

				new Tuple<string, string>("Сторожев Антон это персонаж", yesAnswer),
				new Tuple<string, string>("Сторожев Антон это герой", yesAnswer),
				new Tuple<string, string>("Сторожев Антон это студент", yesAnswer),
				new Tuple<string, string>("Сторожев Антон это злодей", noAnswer),
				new Tuple<string, string>("Сторожев Антон это преподаватель", noAnswer),
				new Tuple<string, string>("Сторожев Антон это Чуприна Светлана Игоревна", noAnswer),

				new Tuple<string, string>("Глагол это глагол", yesAnswer),
				new Tuple<string, string>("Победить это глагол", yesAnswer)
			};

			TestQueries(questions);
		}

		[TestMethod()]
		public void WhereQueriesTest()
		{
			List<Tuple<string, string>> questions = new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("Где отправляться", dontKnowAnswer),
				new Tuple<string, string>("Где отправление главного героя", "Место"),
				new Tuple<string, string>("Где отправление колобка", "Место"),
				new Tuple<string, string>("Где отправление студента", "Место"),

				new Tuple<string, string>("Где встреча главного героя с вредителем", "Место"),
				new Tuple<string, string>("Где встреча колобка с вредителем", "Место"),
				new Tuple<string, string>("Где встреча студента с вредителем", "Место")
			};

			TestQueries(questions);
		}

		[TestMethod()]
		public void GoalFirstQueriesTest()
		{
			List<Tuple<string, string>> questions = new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("Цель отправление", dontKnowAnswer),
				new Tuple<string, string>("Цель отправление главного героя", "Глагол"),
				new Tuple<string, string>("Цель отправление колобка", "Искать приключения"),
				new Tuple<string, string>("Цель отправление студента", "Получить зачет"),
				new Tuple<string, string>("Цель отправление студента получить зачет", dontKnowAnswer),
			};

			TestQueries(questions);
		}

		[TestMethod()]
		public void GoalSecondQueriesTest()
		{
			List<Tuple<string, string>> questions = new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("Глагол цель отправление главного героя", yesAnswer),

				new Tuple<string, string>("Глагол цель отправление колобка", yesAnswer),
				new Tuple<string, string>("Искать приключения цель отправление колобка", yesAnswer),

				new Tuple<string, string>("Глагол цель отправление студента", yesAnswer),
				new Tuple<string, string>("Получить зачет цель отправление студента", yesAnswer),
			};

			TestQueries(questions);
		}

		[TestMethod()]
		public void AgentFirstQueriesTest()
		{
			List<Tuple<string, string>> questions = new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("Агент глагол", dontKnowAnswer),
				new Tuple<string, string>("Агент встречаться", dontKnowAnswer),
				new Tuple<string, string>("Агент встреча главного героя с вредителем", "Герой"),
				new Tuple<string, string>("Агент встреча колобка с вредителем", "Колобок"),
				new Tuple<string, string>("Агент встреча с лисой", "Колобок"),
				new Tuple<string, string>("Агент встреча с волком", "Колобок"),
				new Tuple<string, string>("Агент встреча с медведем", "Колобок"),
				new Tuple<string, string>("Агент встреча с зайцем", "Колобок"),
				new Tuple<string, string>("Агент встреча студента с вредителем", "Студент"),
				new Tuple<string, string>("Агент встреча с преподавателем", "Студент")
			};

			TestQueries(questions);
		}

		[TestMethod()]
		public void AgentSecondQueriesTest()
		{
			List<Tuple<string, string>> questions = new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("Персонаж агент глагол", yesAnswer),
				new Tuple<string, string>("Герой агент глагол", yesAnswer),
				new Tuple<string, string>("Злодей агент глагол", yesAnswer),
				new Tuple<string, string>("Колобок агент глагол", yesAnswer),
				new Tuple<string, string>("Лиса агент глагол", yesAnswer),
				new Tuple<string, string>("Медведь агент глагол", yesAnswer),
				new Tuple<string, string>("Заяц агент глагол", yesAnswer),
				new Tuple<string, string>("Преподаватель агент глагол", yesAnswer),
				new Tuple<string, string>("Студент агент глагол", yesAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна агент глагол", noAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович агент глагол", noAnswer),
				new Tuple<string, string>("Сторожев Антон агент глагол", noAnswer),

				new Tuple<string, string>("Персонаж агент встречаться", yesAnswer),
				new Tuple<string, string>("Герой агент встречаться", yesAnswer),
				new Tuple<string, string>("Злодей агент встречаться", noAnswer),
				new Tuple<string, string>("Колобок агент встречаться", yesAnswer),
				new Tuple<string, string>("Лиса агент встречаться", noAnswer),
				new Tuple<string, string>("Медведь агент встречаться", noAnswer),
				new Tuple<string, string>("Заяц агент встречаться", noAnswer),
				new Tuple<string, string>("Преподаватель агент встречаться", noAnswer),
				new Tuple<string, string>("Студент агент встречаться", yesAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна агент встречаться", noAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович агент встречаться", noAnswer),
				new Tuple<string, string>("Сторожев Антон агент встречаться", noAnswer),

				new Tuple<string, string>("Персонаж агент встреча главного героя с вредителем", yesAnswer),
				new Tuple<string, string>("Герой агент встреча главного героя с вредителем", yesAnswer),
				new Tuple<string, string>("Злодей агент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Колобок агент встреча главного героя с вредителем", yesAnswer),
				new Tuple<string, string>("Лиса агент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Медведь агент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Заяц агент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Преподаватель агент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Студент агент встреча главного героя с вредителем", yesAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна агент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович агент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Сторожев Антон агент встреча главного героя с вредителем", noAnswer),

				new Tuple<string, string>("Персонаж агент встреча колобка с вредителем", yesAnswer),
				new Tuple<string, string>("Герой агент встреча колобка с вредителем", yesAnswer),
				new Tuple<string, string>("Злодей агент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Колобок агент встреча колобка с вредителем", yesAnswer),
				new Tuple<string, string>("Лиса агент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Медведь агент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Заяц агент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Преподаватель агент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Студент агент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна агент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович агент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Сторожев Антон агент встреча колобка с вредителем", noAnswer),

				new Tuple<string, string>("Персонаж агент встреча студента с вредителем", yesAnswer),
				new Tuple<string, string>("Герой агент встреча студента с вредителем", yesAnswer),
				new Tuple<string, string>("Злодей агент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Колобок агент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Лиса агент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Медведь агент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Заяц агент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Преподаватель агент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Студент агент встреча студента с вредителем", yesAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна агент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович агент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Сторожев Антон агент встреча студента с вредителем", noAnswer),
			};

			TestQueries(questions);
		}

		[TestMethod()]
		public void RecipientFirstQueriesTest()
		{
			List<Tuple<string, string>> questions = new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("Реципиент глагол", dontKnowAnswer),
				new Tuple<string, string>("Реципиент встречаться", dontKnowAnswer),
				new Tuple<string, string>("Реципиент встреча главного героя с вредителем", "Злодей"),
				new Tuple<string, string>("Реципиент встреча колобка с вредителем", "Злодей"),
				new Tuple<string, string>("Реципиент встреча колобка с вредителем", "Злодей"),
				new Tuple<string, string>("Реципиент встреча с лисой", "Лиса"),
				new Tuple<string, string>("Реципиент встреча с волком", "Волк"),
				new Tuple<string, string>("Реципиент встреча с медведем", "Медведь"),
				new Tuple<string, string>("Реципиент встреча с зайцем", "Заяц"),
				new Tuple<string, string>("Реципиент встреча студента с вредителем", "Злодей"),
				new Tuple<string, string>("Реципиент встреча с преподавателем", "Преподаватель")
			};

			TestQueries(questions);
		}

		[TestMethod()]
		public void RecipientSecondQueriesTest()
		{
			List<Tuple<string, string>> questions = new List<Tuple<string, string>>()
			{
				new Tuple<string, string>("Персонаж реципиент глагол", yesAnswer),
				new Tuple<string, string>("Герой реципиент глагол", yesAnswer),
				new Tuple<string, string>("Злодей реципиент глагол", yesAnswer),
				new Tuple<string, string>("Колобок реципиент глагол", yesAnswer),
				new Tuple<string, string>("Лиса реципиент глагол", yesAnswer),
				new Tuple<string, string>("Медведь реципиент глагол", yesAnswer),
				new Tuple<string, string>("Заяц реципиент глагол", yesAnswer),
				new Tuple<string, string>("Преподаватель реципиент глагол", yesAnswer),
				new Tuple<string, string>("Студент реципиент глагол", yesAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна реципиент глагол", noAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович реципиент глагол", noAnswer),
				new Tuple<string, string>("Сторожев Антон реципиент глагол", noAnswer),

				new Tuple<string, string>("Персонаж реципиент встречаться", yesAnswer),
				new Tuple<string, string>("Герой реципиент встречаться", yesAnswer),
				new Tuple<string, string>("Злодей реципиент встречаться", noAnswer),
				new Tuple<string, string>("Колобок реципиент встречаться", yesAnswer),
				new Tuple<string, string>("Лиса реципиент встречаться", noAnswer),
				new Tuple<string, string>("Медведь реципиент встречаться", noAnswer),
				new Tuple<string, string>("Заяц реципиент встречаться", noAnswer),
				new Tuple<string, string>("Преподаватель реципиент встречаться", noAnswer),
				new Tuple<string, string>("Студент реципиент встречаться", yesAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна реципиент встречаться", noAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович реципиент встречаться", noAnswer),
				new Tuple<string, string>("Сторожев Антон реципиент встречаться", noAnswer),

				new Tuple<string, string>("Персонаж реципиент встреча главного героя с вредителем", yesAnswer),
				new Tuple<string, string>("Герой реципиент встреча главного героя с вредителем", yesAnswer),
				new Tuple<string, string>("Злодей реципиент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Колобок реципиент встреча главного героя с вредителем", yesAnswer),
				new Tuple<string, string>("Лиса реципиент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Медведь реципиент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Заяц реципиент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Преподаватель реципиент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Студент реципиент встреча главного героя с вредителем", yesAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна реципиент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович реципиент встреча главного героя с вредителем", noAnswer),
				new Tuple<string, string>("Сторожев Антон реципиент встреча главного героя с вредителем", noAnswer),

				new Tuple<string, string>("Персонаж реципиент встреча колобка с вредителем", yesAnswer),
				new Tuple<string, string>("Герой реципиент встреча колобка с вредителем", yesAnswer),
				new Tuple<string, string>("Злодей реципиент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Колобок реципиент встреча колобка с вредителем", yesAnswer),
				new Tuple<string, string>("Лиса реципиент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Медведь реципиент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Заяц реципиент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Преподаватель реципиент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Студент реципиент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна реципиент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович реципиент встреча колобка с вредителем", noAnswer),
				new Tuple<string, string>("Сторожев Антон реципиент встреча колобка с вредителем", noAnswer),

				new Tuple<string, string>("Персонаж реципиент встреча студента с вредителем", yesAnswer),
				new Tuple<string, string>("Герой реципиент встреча студента с вредителем", yesAnswer),
				new Tuple<string, string>("Злодей реципиент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Колобок реципиент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Лиса реципиент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Медведь реципиент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Заяц реципиент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Преподаватель реципиент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Студент реципиент встреча студента с вредителем", yesAnswer),
				new Tuple<string, string>("Чуприна Светлана Игоревна реципиент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Юрков Кирилл Александрович реципиент встреча студента с вредителем", noAnswer),
				new Tuple<string, string>("Сторожев Антон реципиент встреча студента с вредителем", noAnswer),
			};

			TestQueries(questions);
		}
		#endregion
	}
}
