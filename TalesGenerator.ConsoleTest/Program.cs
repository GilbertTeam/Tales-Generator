﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TalesGenerator.Core;
using TalesGenerator.Core.Collections;
using TalesGenerator.Text;

namespace TalesGenerator.ConsoleTest
{
	public static class Program
	{
		#region Methods

		private static Network CreateNetwork()
		{
			Network network = new Network();
			NetworkNode taleNode = network.Nodes.Add("Сказка");
			NetworkNode heroNode = network.Nodes.Add("Герой");
			NetworkNode antagonistNode = network.Nodes.Add("Злодей");
			NetworkNode grandMotherNode = network.Nodes.Add("Старушка");
			NetworkNode grandFatherNode = network.Nodes.Add("Старичок");
			NetworkNode girlNode = network.Nodes.Add("Девочка");
			NetworkNode boyNode = network.Nodes.Add("Мальчик");
			NetworkNode geeseNode = network.Nodes.Add("Гуси-ледеди");
			NetworkNode stoveNode = network.Nodes.Add("Печка");
			NetworkNode appleTreeNode = network.Nodes.Add("Яблоня");
			NetworkNode riverNode = network.Nodes.Add("Река");
			NetworkNode babaYagaNode = network.Nodes.Add("Баба-Яга");
			NetworkNode cabinOnChickenLegsNode = network.Nodes.Add("Избушка на курьих ножках");

			#region Base Network

			NetworkNode initialStateNode = network.Nodes.Add("Начальная ситуация");
			NetworkNode initialStateTemplateNode = network.Nodes.Add("{Action} {agents}.");
			NetworkNode initialStateActionNode = network.Nodes.Add("Жили-были");
			network.Edges.Add(initialStateNode, initialStateTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(initialStateNode, initialStateActionNode, NetworkEdgeType.Action);

			NetworkNode prohibitionNode = network.Nodes.Add("Запрет");
			NetworkNode prohibitionTemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"{Recipient}, {action}.\"");
			NetworkNode prohibitionActionNode = network.Nodes.Add("Не уходи со двора");
			network.Edges.Add(prohibitionNode, prohibitionTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(prohibitionNode, prohibitionActionNode, NetworkEdgeType.Action);

			NetworkNode seniorsDepartureNode = network.Nodes.Add("Уход старших");
			NetworkNode seniorsDepartureTemplateNode = network.Nodes.Add("{Agents} {action}.");
			NetworkNode seniorsDepartureActionNode = network.Nodes.Add("Уходить");
			network.Edges.Add(seniorsDepartureNode, seniorsDepartureTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(seniorsDepartureNode, seniorsDepartureActionNode, NetworkEdgeType.Action);

			NetworkNode prohibitionViolationNode = network.Nodes.Add("Нарушение запрета");
			NetworkNode prohibitionViolationTemplateNode = network.Nodes.Add("{Agent} {action}.");
			NetworkNode prohibitionViolationActionNode = network.Nodes.Add("Нарушить запрет");
			network.Edges.Add(prohibitionViolationNode, prohibitionViolationTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(prohibitionViolationNode, prohibitionViolationActionNode, NetworkEdgeType.Action);

			NetworkNode sabotageNode = network.Nodes.Add("Вредительство");
			NetworkNode sabotageTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode sabotageActionNode = network.Nodes.Add("Похитить");
			network.Edges.Add(sabotageNode, sabotageTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(sabotageNode, sabotageActionNode, NetworkEdgeType.Action);

			NetworkNode woesPostNode = network.Nodes.Add("Сообщение беды");
			NetworkNode woesPostTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode woesPostActionNode = network.Nodes.Add("Обнаружить пропажу");
			network.Edges.Add(woesPostNode, woesPostTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(woesPostNode, woesPostActionNode, NetworkEdgeType.Action);

			NetworkNode searchSubmittingNode = network.Nodes.Add("Отправка на поиски");
			NetworkNode searchSubmittingTemplateNode = network.Nodes.Add("{Agent} {action}.");
			NetworkNode searchSubmittingActionNode = network.Nodes.Add("Отправилась на поиски");
			network.Edges.Add(searchSubmittingNode, searchSubmittingTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(searchSubmittingNode, searchSubmittingActionNode, NetworkEdgeType.Action);

			NetworkNode testerMeetingNode = network.Nodes.Add("Встреча с испытателем");
			NetworkNode testerMeetingTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode testerMeetingActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(testerMeetingNode, testerMeetingTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(testerMeetingNode, testerMeetingActionNode, NetworkEdgeType.Action);

			NetworkNode testNode = network.Nodes.Add("Испытание");
			NetworkNode testTemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"Съешь моего ржаного пирожка - скажу\".");
			NetworkNode testActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(testNode, testTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(testNode, testActionNode, NetworkEdgeType.Action);

			NetworkNode testAttemptNode = network.Nodes.Add("Попытка пройти испытание");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode testAttemptTemplateNode = network.Nodes.Add("{Agent} {~ответил:agent} {recipient}: \"О, у моего батюшки пшеничные не едятся\".");
			NetworkNode testAttemptActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(testAttemptNode, testAttemptTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(testAttemptNode, testAttemptActionNode, NetworkEdgeType.Action);

			NetworkNode testResultNode = network.Nodes.Add("Результат испытания");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode testResultTemplateNode = network.Nodes.Add("{Agent} не сказала {recipient}.");
			NetworkNode testResultActionNode = network.Nodes.Add("Сказать");
			network.Edges.Add(testResultNode, testResultTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(testResultNode, testResultActionNode, NetworkEdgeType.Action);

			NetworkNode antagonistHomeNode = network.Nodes.Add("Жилище антагониста");
			NetworkNode antagonistHomeTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode antagonistHomeActionNode = network.Nodes.Add("Увидеть");
			network.Edges.Add(antagonistHomeNode, antagonistHomeTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(antagonistHomeNode, antagonistHomeActionNode, NetworkEdgeType.Action);

			NetworkNode antagonistMeetingNode = network.Nodes.Add("Облик антагониста");
			NetworkNode antagonistMeetingTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode antagonistMeetingActionNode = network.Nodes.Add("Увидеть");
			network.Edges.Add(antagonistMeetingNode, antagonistMeetingTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(antagonistMeetingNode, antagonistMeetingActionNode, NetworkEdgeType.Action);
			network.Edges.Add(antagonistMeetingNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(antagonistMeetingNode, babaYagaNode, NetworkEdgeType.Recipient);

			NetworkNode desiredCharacterAppearanceNode = network.Nodes.Add("Появление искомого персонажа");
			NetworkNode desiredCharacterAppearanceTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode desiredCharacterAppearanceActionNode = network.Nodes.Add("Увидеть");
			network.Edges.Add(desiredCharacterAppearanceNode, desiredCharacterAppearanceTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(desiredCharacterAppearanceNode, desiredCharacterAppearanceActionNode, NetworkEdgeType.Action);

			NetworkNode desiredCharacterLiberationNode = network.Nodes.Add("Добыча искомого персонажа с применением хитрости или силы");
			NetworkNode desiredCharacterLiberationTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode desiredCharacterLiberationActionNode = network.Nodes.Add("Схватить и унести");
			network.Edges.Add(desiredCharacterLiberationNode, desiredCharacterLiberationTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(desiredCharacterLiberationNode, desiredCharacterLiberationActionNode, NetworkEdgeType.Action);

			NetworkNode persecutionBeginningNode = network.Nodes.Add("Начало преследования");
			NetworkNode persecutionBeginningTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode persecutionBeginningActionNode = network.Nodes.Add("Начать преследовать");
			network.Edges.Add(persecutionBeginningNode, persecutionBeginningTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(persecutionBeginningNode, persecutionBeginningActionNode, NetworkEdgeType.Action);
			#endregion

			#region Сказка "Гуси-лебеди"

			NetworkNode geeseTaleNode = network.Nodes.Add("Сказка \"Гуси-лебеди\"");
			network.Edges.Add(taleNode, geeseTaleNode, NetworkEdgeType.IsA);

			NetworkNode gtInitialStateNode = network.Nodes.Add("Начальная ситуация");
			network.Edges.Add(gtInitialStateNode, initialStateNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtInitialStateNode, grandMotherNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtInitialStateNode, grandFatherNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtInitialStateNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtInitialStateNode, boyNode, NetworkEdgeType.Agent);

			NetworkNode gtProhibitionNode = network.Nodes.Add("Запрет");
			network.Edges.Add(gtProhibitionNode, prohibitionNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtProhibitionNode, grandMotherNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtProhibitionNode, girlNode, NetworkEdgeType.Recipient);

			NetworkNode gtSeniorsDepartureNode = network.Nodes.Add("Уход старших");
			network.Edges.Add(gtSeniorsDepartureNode, seniorsDepartureNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtSeniorsDepartureNode, grandMotherNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtSeniorsDepartureNode, grandFatherNode, NetworkEdgeType.Agent);

			NetworkNode gtProhibitionViolationNode = network.Nodes.Add("Нарушение запрета");
			network.Edges.Add(gtProhibitionViolationNode, prohibitionViolationNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtProhibitionViolationNode, girlNode, NetworkEdgeType.Agent);

			NetworkNode gtSabotageNode = network.Nodes.Add("Вредительство");
			NetworkNode gtSabotageTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode gtSabotageActionNode = network.Nodes.Add("Похитить");
			network.Edges.Add(gtSabotageNode, gtSabotageTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(gtSabotageNode, gtSabotageActionNode, NetworkEdgeType.Action);
			network.Edges.Add(gtSabotageNode, geeseNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtSabotageNode, boyNode, NetworkEdgeType.Recipient);

			NetworkNode gtWoesPostNode = network.Nodes.Add("Сообщение беды");
			network.Edges.Add(gtWoesPostNode, woesPostNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtWoesPostNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtWoesPostNode, boyNode, NetworkEdgeType.Recipient);

			NetworkNode gtSearchSubmittingNode = network.Nodes.Add("Отправка на поиски");
			network.Edges.Add(gtSearchSubmittingNode, searchSubmittingNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtSearchSubmittingNode, girlNode, NetworkEdgeType.Agent);

			NetworkNode gtFirstTesterMeetingNode = network.Nodes.Add("Встреча с испытателем");
			network.Edges.Add(gtFirstTesterMeetingNode, testerMeetingNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtFirstTesterMeetingNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtFirstTesterMeetingNode, stoveNode, NetworkEdgeType.Recipient);

			NetworkNode gtFirstTestNode = network.Nodes.Add("Испытание");
			network.Edges.Add(gtFirstTestNode, testNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtFirstTestNode, stoveNode, NetworkEdgeType.Agent);

			NetworkNode gtFirstTestAttemptNode = network.Nodes.Add("Попытка пройти испытание");
			network.Edges.Add(gtFirstTestAttemptNode, testAttemptNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtFirstTestAttemptNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtFirstTestAttemptNode, stoveNode, NetworkEdgeType.Recipient);

			NetworkNode gtFirstTestResultNode = network.Nodes.Add("Результат испытания");
			network.Edges.Add(gtFirstTestResultNode, testResultNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtFirstTestResultNode, stoveNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtFirstTestResultNode, girlNode, NetworkEdgeType.Recipient);

			NetworkNode gtSecondTesterMeetingNode = network.Nodes.Add("Встреча с испытателем");
			network.Edges.Add(gtSecondTesterMeetingNode, testerMeetingNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtSecondTesterMeetingNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtSecondTesterMeetingNode, appleTreeNode, NetworkEdgeType.Recipient);

			NetworkNode gtSecondTestNode = network.Nodes.Add("Испытание");
			network.Edges.Add(gtSecondTestNode, testNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtSecondTestNode, appleTreeNode, NetworkEdgeType.Agent);

			NetworkNode gtSecondTestAttemptNode = network.Nodes.Add("Попытка пройти испытание");
			network.Edges.Add(gtSecondTestAttemptNode, testAttemptNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtSecondTestAttemptNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtSecondTestAttemptNode, appleTreeNode, NetworkEdgeType.Recipient);

			NetworkNode gtSecondTestFailNode = network.Nodes.Add("Результат испытания");
			network.Edges.Add(gtSecondTestFailNode, testResultNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtSecondTestFailNode, appleTreeNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtSecondTestFailNode, girlNode, NetworkEdgeType.Recipient);

			NetworkNode gtThirdTesterMeetingNode = network.Nodes.Add("Встреча с испытателем");
			network.Edges.Add(gtThirdTesterMeetingNode, testerMeetingNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtThirdTesterMeetingNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtThirdTesterMeetingNode, riverNode, NetworkEdgeType.Recipient);

			NetworkNode gtThirdTestNode = network.Nodes.Add("Испытание");
			network.Edges.Add(gtThirdTestNode, testNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtThirdTestNode, riverNode, NetworkEdgeType.Agent);

			NetworkNode gtThirdTestAttemptNode = network.Nodes.Add("Попытка пройти испытание");
			network.Edges.Add(gtThirdTestAttemptNode, testAttemptNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtThirdTestAttemptNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtThirdTestAttemptNode, riverNode, NetworkEdgeType.Recipient);

			NetworkNode gtThirdTestFailNode = network.Nodes.Add("Результат испытания");
			network.Edges.Add(gtThirdTestFailNode, testResultNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtThirdTestFailNode, riverNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtThirdTestFailNode, girlNode, NetworkEdgeType.Recipient);

			//NetworkNode fourthTesterMeetingNode = network.Nodes.Add("Встреча с испытателем");
			//NetworkNode fourthTesterMeetingTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			//NetworkNode fourthTesterMeetingActionNode = network.Nodes.Add("Встретить");
			//network.Edges.Add(fourthTesterMeetingNode, thirdTesterMeetingTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(fourthTesterMeetingNode, thirdTesterMeetingActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(fourthTesterMeetingNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(fourthTesterMeetingNode, riverNode, NetworkEdgeType.Recipient);

			//NetworkNode fourthTestNode = network.Nodes.Add("Испытание");
			//NetworkNode fourthTestTemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"Поешь моего простого киселька с молочком - скажу.\".");
			//NetworkNode fourthTestActionNode = network.Nodes.Add("Встретить");
			//network.Edges.Add(fourthTestNode, thirdTestTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(fourthTestNode, thirdTestActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(fourthTestNode, riverNode, NetworkEdgeType.Agent);

			//NetworkNode fourthTestAttemptNode = network.Nodes.Add("Попытка пройти испытание");
			////TODO В этом случае ответил не определяется как сказуемое.
			//NetworkNode fourthTestAttemptTemplateNode = network.Nodes.Add("{Agent} {~ответил:agent} {recipient}: \"У моего батюшки и сливочки не едятся...\".");
			//NetworkNode fourthTestAttemptActionNode = network.Nodes.Add("Встретить");
			//network.Edges.Add(fourthTestAttemptNode, secondTestAttemptTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(fourthTestAttemptNode, secondTestAttemptActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(fourthTestAttemptNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(fourthTestAttemptNode, riverNode, NetworkEdgeType.Recipient);

			//NetworkNode fourthTestFailNode = network.Nodes.Add("Результат испытания");
			////TODO В этом случае ответил не определяется как сказуемое.
			//NetworkNode fourthTestFailTemplateNode = network.Nodes.Add("{Agent} не {action} {recipient}.");
			//NetworkNode fourthTestFailActionNode = network.Nodes.Add("Сказать");
			//network.Edges.Add(fourthTestFailNode, thirdTestFailTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(fourthTestFailNode, thirdTestFailActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(fourthTestFailNode, riverNode, NetworkEdgeType.Agent);
			//network.Edges.Add(fourthTestFailNode, girlNode, NetworkEdgeType.Recipient);

			NetworkNode gtAntagonistHomeNode = network.Nodes.Add("Жилище антагониста");
			network.Edges.Add(gtAntagonistHomeNode, antagonistHomeNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtAntagonistHomeNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtAntagonistHomeNode, cabinOnChickenLegsNode, NetworkEdgeType.Recipient);

			NetworkNode gtAntagonistMeetingNode = network.Nodes.Add("Облик антагониста");
			network.Edges.Add(gtAntagonistMeetingNode, antagonistMeetingNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtAntagonistMeetingNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtAntagonistMeetingNode, babaYagaNode, NetworkEdgeType.Recipient);

			NetworkNode gtDesiredCharacterAppearanceNode = network.Nodes.Add("Появление искомого персонажа");
			network.Edges.Add(gtDesiredCharacterAppearanceNode, desiredCharacterAppearanceNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtDesiredCharacterAppearanceNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtDesiredCharacterAppearanceNode, boyNode, NetworkEdgeType.Recipient);

			NetworkNode gtDesiredCharacterLiberationNode = network.Nodes.Add("Добыча искомого персонажа с применением хитрости или силы");
			network.Edges.Add(gtDesiredCharacterLiberationNode, desiredCharacterLiberationNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtDesiredCharacterLiberationNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtDesiredCharacterLiberationNode, boyNode, NetworkEdgeType.Recipient);

			NetworkNode gtPersecutionBeginningNode = network.Nodes.Add("Начало преследования");
			network.Edges.Add(gtPersecutionBeginningNode, persecutionBeginningNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtPersecutionBeginningNode, geeseNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtPersecutionBeginningNode, girlNode, NetworkEdgeType.Recipient);

			//TODO Необходимо дублировать данные вершины, в противном случае будет цикл.
			NetworkNode gtThirdTesterMeeting2Node = network.Nodes.Add("Встреча с испытателем");
			network.Edges.Add(gtThirdTesterMeeting2Node, testerMeetingNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtThirdTesterMeeting2Node, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtThirdTesterMeeting2Node, riverNode, NetworkEdgeType.Recipient);

			NetworkNode gtThirdTest2Node = network.Nodes.Add("Испытание");
			network.Edges.Add(gtThirdTest2Node, testNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtThirdTest2Node, riverNode, NetworkEdgeType.Agent);

			NetworkNode gtThirdTestAttempt2Node = network.Nodes.Add("Попытка пройти испытание");
			network.Edges.Add(gtThirdTestAttempt2Node, testAttemptNode, NetworkEdgeType.IsA);
			network.Edges.Add(gtThirdTestAttempt2Node, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtThirdTestAttempt2Node, riverNode, NetworkEdgeType.Recipient);

			NetworkNode gtThirdTestSuccessNode = network.Nodes.Add("Результат испытания");
			//TODO Непонятно, как представлять данное предложение.
			NetworkNode thirdTestSuccessTemplateNode = network.Nodes.Add("{Agent} укрыла ее под кисельным бережком. Гуси-лебеди не увидали, пролетели мимо.");
			NetworkNode thirdTestSuccessActionNode = network.Nodes.Add("Сказать");
			network.Edges.Add(gtThirdTestSuccessNode, thirdTestSuccessTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(gtThirdTestSuccessNode, thirdTestSuccessActionNode, NetworkEdgeType.Action);
			network.Edges.Add(gtThirdTestSuccessNode, riverNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtThirdTestSuccessNode, girlNode, NetworkEdgeType.Recipient);

			NetworkNode secondTesterMeeting2Node = network.Nodes.Add("Встреча с испытателем");
			NetworkNode secondTesterMeeting2TemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode secondTesterMeeting2ActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(secondTesterMeeting2Node, secondTesterMeeting2TemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(secondTesterMeeting2Node, secondTesterMeeting2ActionNode, NetworkEdgeType.Action);
			network.Edges.Add(secondTesterMeeting2Node, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(secondTesterMeeting2Node, appleTreeNode, NetworkEdgeType.Recipient);

			NetworkNode secondTest2Node = network.Nodes.Add("Испытание");
			NetworkNode secondTest2TemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"Поешь моего лесного яблочка - скажу.\".");
			NetworkNode secondTest2ActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(secondTest2Node, secondTest2TemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(secondTest2Node, secondTest2ActionNode, NetworkEdgeType.Action);
			network.Edges.Add(secondTest2Node, appleTreeNode, NetworkEdgeType.Agent);

			NetworkNode secondTestAttempt2Node = network.Nodes.Add("Попытка пройти испытание");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode secondTestAttempt2TemplateNode = network.Nodes.Add("{Agent} {~ответил:agent} {recipient}: \"У моего батюшки и садовые не едятся...\".");
			NetworkNode secondTestAttempt2ActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(secondTestAttempt2Node, secondTestAttempt2TemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(secondTestAttempt2Node, secondTestAttempt2ActionNode, NetworkEdgeType.Action);
			network.Edges.Add(secondTestAttempt2Node, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(secondTestAttempt2Node, appleTreeNode, NetworkEdgeType.Recipient);

			NetworkNode secondTestSuccessNode = network.Nodes.Add("Результат испытания");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode secondTestSuccessTemplateNode = network.Nodes.Add("{Agent} не сказала {recipient}.");
			NetworkNode secondTestSuccessActionNode = network.Nodes.Add("Сказать");
			network.Edges.Add(secondTestSuccessNode, secondTestSuccessTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(secondTestSuccessNode, secondTestSuccessActionNode, NetworkEdgeType.Action);
			network.Edges.Add(secondTestSuccessNode, appleTreeNode, NetworkEdgeType.Agent);
			network.Edges.Add(secondTestSuccessNode, girlNode, NetworkEdgeType.Recipient);

			NetworkNode firstTesterMeeting2Node = network.Nodes.Add("Встреча с испытателем");
			NetworkNode firstTesterMeeting2TemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode firstTesterMeeting2ActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(firstTesterMeeting2Node, firstTesterMeeting2TemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(firstTesterMeeting2Node, firstTesterMeeting2ActionNode, NetworkEdgeType.Action);
			network.Edges.Add(firstTesterMeeting2Node, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(firstTesterMeeting2Node, stoveNode, NetworkEdgeType.Recipient);

			NetworkNode firstTest2Node = network.Nodes.Add("Испытание");
			NetworkNode firstTest2TemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"Съешь моего ржаного пирожка - скажу\".");
			NetworkNode firstTest2ActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(firstTest2Node, firstTest2TemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(firstTest2Node, firstTest2ActionNode, NetworkEdgeType.Action);
			network.Edges.Add(firstTest2Node, stoveNode, NetworkEdgeType.Agent);

			NetworkNode firstTestAttempt2Node = network.Nodes.Add("Попытка пройти испытание");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode firstTestAttempt2TemplateNode = network.Nodes.Add("{Agent} {~ответил:agent} {recipient}: \"О, у моего батюшки пшеничные не едятся\".");
			NetworkNode firstTestAttempt2ActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(firstTestAttempt2Node, firstTestAttempt2TemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(firstTestAttempt2Node, firstTestAttempt2ActionNode, NetworkEdgeType.Action);
			network.Edges.Add(firstTestAttempt2Node, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(firstTestAttempt2Node, stoveNode, NetworkEdgeType.Recipient);

			NetworkNode firstTestSuccessNode = network.Nodes.Add("Результат испытания");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode firstTestSuccessTemplateNode = network.Nodes.Add("{Agent} не сказала {recipient}.");
			NetworkNode firstTestSuccessActionNode = network.Nodes.Add("Сказать");
			network.Edges.Add(firstTestSuccessNode, firstTestSuccessTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(firstTestSuccessNode, firstTestSuccessActionNode, NetworkEdgeType.Action);
			network.Edges.Add(firstTestSuccessNode, stoveNode, NetworkEdgeType.Agent);
			network.Edges.Add(firstTestSuccessNode, girlNode, NetworkEdgeType.Recipient);

			network.Edges.Add(geeseTaleNode, gtInitialStateNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtInitialStateNode, gtProhibitionNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtProhibitionNode, gtSeniorsDepartureNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtSeniorsDepartureNode, gtProhibitionViolationNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtProhibitionViolationNode, gtSabotageNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtSabotageNode, gtWoesPostNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtWoesPostNode, gtSearchSubmittingNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtSearchSubmittingNode, gtFirstTesterMeetingNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtFirstTesterMeetingNode, gtFirstTestNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtFirstTestNode, gtFirstTestAttemptNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtFirstTestAttemptNode, gtFirstTestResultNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtFirstTestResultNode, gtSecondTesterMeetingNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtSecondTesterMeetingNode, gtSecondTestNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtSecondTestNode, gtSecondTestAttemptNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtSecondTestAttemptNode, gtSecondTestFailNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtSecondTestFailNode, gtThirdTesterMeetingNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtThirdTesterMeetingNode, gtThirdTestNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtThirdTestNode, gtThirdTestAttemptNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtThirdTestAttemptNode, gtThirdTestFailNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtThirdTestFailNode, gtAntagonistHomeNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtAntagonistHomeNode, gtAntagonistMeetingNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtAntagonistMeetingNode, gtDesiredCharacterAppearanceNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtDesiredCharacterAppearanceNode, gtDesiredCharacterLiberationNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtDesiredCharacterLiberationNode, gtPersecutionBeginningNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtPersecutionBeginningNode, gtThirdTesterMeeting2Node, NetworkEdgeType.Follow);
			network.Edges.Add(gtThirdTesterMeeting2Node, gtThirdTest2Node, NetworkEdgeType.Follow);
			network.Edges.Add(gtThirdTest2Node, gtThirdTestAttempt2Node, NetworkEdgeType.Follow);
			network.Edges.Add(gtThirdTestAttempt2Node, gtThirdTestSuccessNode, NetworkEdgeType.Follow);
			#endregion

			#region Сказка

			#endregion

			return network;
		}

		private static void AnalyzeText(string path, Network network)
		{
			TextAnalyzer textAnalyzer = new TextAnalyzer(AdapterKind.RussianCp1251Adapter);
			textAnalyzer.Load(
				Path.Combine(Environment.CurrentDirectory, "Dictionaries", "Russian", "Dictionary.auto"),
				Path.Combine(Environment.CurrentDirectory, "Dictionaries", "Russian", "Paradigms.bin"),
				Path.Combine(Environment.CurrentDirectory, "Dictionaries", "Russian", "PredictionDictionary.auto"));

			List<NetworkNode> context = new List<NetworkNode>();
			List<NetworkNode> context2 = new List<NetworkNode>();

			using (FileStream fileStream = File.Open(path, FileMode.Open))
			using (StreamReader reader = new StreamReader(fileStream))
			{
				while (!reader.EndOfStream)
				{
					string currentLine = reader.ReadLine();
					string[] words = currentLine.Split(' ');

					foreach (string word in words)
					{
						var foundNodes = network.Nodes.Where(node => string.Equals(node.Name, word, StringComparison.InvariantCultureIgnoreCase));
						context2.AddRange(network.Nodes.Where(node => node.Name.Contains(word)));

						if (foundNodes.Count() > 0)
						{
							context.AddRange(foundNodes);
						}
						else
						{
							var results = textAnalyzer.Lemmatize(word, true);
							LemmatizeResult result = results.FirstOrDefault();

							if (result != null)
							{
								PartOfSpeech partOfSpeech = result.GetPartOfSpeech();

								//if (partOfSpeech == PartOfSpeech.NOUN)
								{
									string lemma = result.GetTextByFormId(0);

									context.AddRange(network.Nodes.Where(node => string.Equals(node.Name, lemma, StringComparison.InvariantCultureIgnoreCase)));
									context2.AddRange(network.Nodes.Where(node => node.Name.Contains(lemma)));
								}
							}
						}
					}
				}
			}
		}

		public static void Main(string[] args)
		{
			Network network = CreateNetwork();

			//AnalyzeText("OriginalText.txt", network);

			NetworkNode taleNode = network.Nodes.SingleOrDefault(node => node.Name.Equals("Сказка"));

			if (taleNode != null)
			{
				using (TextAnalyzer textAnalyzer = new TextAnalyzer(AdapterKind.RussianCp1251Adapter))
				{
					textAnalyzer.Load(
						Path.Combine(Environment.CurrentDirectory, "Dictionaries", "Russian", "Dictionary.auto"),
						Path.Combine(Environment.CurrentDirectory, "Dictionaries", "Russian", "Paradigms.bin"),
						Path.Combine(Environment.CurrentDirectory, "Dictionaries", "Russian", "PredictionDictionary.auto"));

					TextGenerator textGenerator = new TextGenerator(textAnalyzer);
					var taleNodes = taleNode.OutgoingEdges.GetEdges(NetworkEdgeType.IsA).Select(edge => edge.EndNode);

					foreach (NetworkNode currentTaleNode in taleNodes)
					{
						NetworkNode startNode = currentTaleNode.OutgoingEdges.GetEdge(NetworkEdgeType.Follow).EndNode;

						string text = textGenerator.GenerateText(startNode);
					}
				}
			}
		}
		#endregion
	}
}