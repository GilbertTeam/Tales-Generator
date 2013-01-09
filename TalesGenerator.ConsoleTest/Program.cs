using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TalesGenerator.Net;
using TalesGenerator.TaleNet;
using TalesGenerator.Text;
using System.Xml.Linq;

namespace TalesGenerator.ConsoleTest
{
	public static class Program
	{
		#region Methods

		private static TalesNetwork CreateNetwork()
		{
			TalesNetwork talesNetwork = new TalesNetwork();
			TaleItemNode positivePersonNode = talesNetwork.Persons.Add("Положительный персонаж");
			TaleItemNode negativePersonNode = talesNetwork.Persons.Add("Отрицательный персонаж");
			TaleItemNode parentsNode = talesNetwork.Persons.Add("Родители", positivePersonNode);
			TaleItemNode heroNode = talesNetwork.Persons.Add("Герой", positivePersonNode);
			TaleItemNode donorNode = talesNetwork.Persons.Add("Испытатель", positivePersonNode);
			TaleItemNode seekingPersonNode = talesNetwork.Persons.Add("Искомый персонаж", positivePersonNode);
			TaleItemNode antagonistNode = talesNetwork.Persons.Add("Злодей", negativePersonNode);

			#region Scenario Tale

			TaleNode scenarioTaleNode = talesNetwork.Tales.Add("Сказка-сценарий");

			FunctionNode initialStateNode = scenarioTaleNode.Functions.Add("Начальная ситуация", FunctionType.InitialState);
			initialStateNode.Template = "{Action} {agents}.";
			initialStateNode.Actions.Add(talesNetwork.Actions.Add("Жили-были"));
			initialStateNode.Agents.Add(heroNode);

			FunctionNode interdictionNode = scenarioTaleNode.Functions.Add("Запрет", FunctionType.Interdiction);
			interdictionNode.Template = "{Agent} {~сказал:agent}: \"{Recipient}, {action}.\"";
			interdictionNode.Actions.Add(talesNetwork.Actions.Add("Не уходить"));
			interdictionNode.Agents.Add(parentsNode);
			interdictionNode.Recipients.Add(heroNode);

			FunctionNode absentationNode = scenarioTaleNode.Functions.Add("Отлучка старших", FunctionType.Absentation);
			absentationNode.Template = "{Agents} {action}.";
			absentationNode.Actions.Add(talesNetwork.Actions.Add("Уйти"));
			absentationNode.Agents.Add(parentsNode);

			FunctionNode violationOfInterdictionNode = scenarioTaleNode.Functions.Add("Нарушение запрета", FunctionType.ViolationOfInterdiction);
			violationOfInterdictionNode.Template = "{Agent} {action}.";
			violationOfInterdictionNode.Actions.Add(talesNetwork.Actions.Add("Нарушить запрет"));
			violationOfInterdictionNode.Agents.Add(heroNode);

			FunctionNode villainyNode = scenarioTaleNode.Functions.Add("Вредительство", FunctionType.Villainy);
			villainyNode.Template = "{Agent} {action} {recipient}.";
			villainyNode.Actions.Add(talesNetwork.Actions.Add("Похитить"));
			villainyNode.Agents.Add(antagonistNode);
			villainyNode.Recipients.Add(seekingPersonNode);

			FunctionNode mediationNode = scenarioTaleNode.Functions.Add("Сообщение беды", FunctionType.Mediation);
			mediationNode.Template = "{Agent} {action} {recipient}.";
			mediationNode.Actions.Add(talesNetwork.Actions.Add("Обнаружить пропажу"));
			mediationNode.Agents.Add(heroNode);
			mediationNode.Recipients.Add(seekingPersonNode);

			FunctionNode departureNode = scenarioTaleNode.Functions.Add("Отправка на поиски", FunctionType.Departure);
			departureNode.Template = "{Agent} {action}.";
			departureNode.Actions.Add(talesNetwork.Actions.Add("Отправиться на поиски"));
			departureNode.Agents.Add(heroNode);

			FunctionNode donorMeeting = scenarioTaleNode.Functions.Add("Встреча с испытателем", FunctionType.DonorMeeting);
			donorMeeting.Template = "{Agent} {action} {recipient}.";
			donorMeeting.Actions.Add(talesNetwork.Actions.Add("Встретить"));
			donorMeeting.Agents.Add(heroNode);
			donorMeeting.Recipients.Add(donorNode);

			FunctionNode testNode = scenarioTaleNode.Functions.Add("Испытание", FunctionType.Test);
			testNode.Template = "{Agent} {action} {recipient}";
			testNode.Actions.Add(talesNetwork.Actions.Add("Испытать"));
			testNode.Agents.Add(donorNode);
			testNode.Recipients.Add(heroNode);

			FunctionNode testAttemptNode = scenarioTaleNode.Functions.Add("Попытка пройти испытание", FunctionType.TestAttempt);
			testAttemptNode.Template = "{Agent} {action} {recipient}.";
			testAttemptNode.Actions.Add(talesNetwork.Actions.Add("Отказать"));
			testAttemptNode.Agents.Add(heroNode);
			testAttemptNode.Recipients.Add(donorNode);

			FunctionNode testResultNode = scenarioTaleNode.Functions.Add("Результат испытания", FunctionType.TestResult);
			testResultNode.Template = "{Agent} {action} {recipient}.";
			testResultNode.Actions.Add(talesNetwork.Actions.Add("Сказать"));
			testResultNode.Agents.Add(donorNode);
			testResultNode.Recipients.Add(heroNode);

			//NetworkNode antagonistHomeNode = network.Nodes.Add("Жилище антагониста");
			//NetworkNode antagonistHomeTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			//NetworkNode antagonistHomeActionNode = network.Nodes.Add("Увидеть");
			//network.Edges.Add(antagonistHomeNode, antagonistHomeTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(antagonistHomeNode, antagonistHomeActionNode, NetworkEdgeType.Action);

			//NetworkNode antagonistMeetingNode = network.Nodes.Add("Облик антагониста");
			//NetworkNode antagonistMeetingTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			//NetworkNode antagonistMeetingActionNode = network.Nodes.Add("Увидеть");
			//network.Edges.Add(antagonistMeetingNode, antagonistMeetingTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(antagonistMeetingNode, antagonistMeetingActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(antagonistMeetingNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(antagonistMeetingNode, babaYagaNode, NetworkEdgeType.Recipient);

			//NetworkNode desiredCharacterAppearanceNode = network.Nodes.Add("Появление искомого персонажа");
			//NetworkNode desiredCharacterAppearanceTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			//NetworkNode desiredCharacterAppearanceActionNode = network.Nodes.Add("Увидеть");
			//network.Edges.Add(desiredCharacterAppearanceNode, desiredCharacterAppearanceTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(desiredCharacterAppearanceNode, desiredCharacterAppearanceActionNode, NetworkEdgeType.Action);

			//NetworkNode desiredCharacterLiberationNode = network.Nodes.Add("Добыча искомого персонажа с применением хитрости или силы");
			//NetworkNode desiredCharacterLiberationTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			//NetworkNode desiredCharacterLiberationActionNode = network.Nodes.Add("Схватить и унести");
			//network.Edges.Add(desiredCharacterLiberationNode, desiredCharacterLiberationTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(desiredCharacterLiberationNode, desiredCharacterLiberationActionNode, NetworkEdgeType.Action);

			//NetworkNode persecutionBeginningNode = network.Nodes.Add("Начало преследования");
			//NetworkNode persecutionBeginningTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			//NetworkNode persecutionBeginningActionNode = network.Nodes.Add("Начать преследовать");
			//network.Edges.Add(persecutionBeginningNode, persecutionBeginningTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(persecutionBeginningNode, persecutionBeginningActionNode, NetworkEdgeType.Action);
			#endregion

			#region Сказка "Гуси-лебеди"

			TaleNode geeseTaleNode = talesNetwork.Tales.Add("Сказка \"Гуси-лебеди\"", scenarioTaleNode);
			TaleItemNode grandMotherNode = talesNetwork.Persons.Add("Старушка", parentsNode);
			TaleItemNode grandFatherNode = talesNetwork.Persons.Add("Старичок", parentsNode);
			TaleItemNode girlNode = talesNetwork.Persons.Add("Девочка", heroNode);
			TaleItemNode boyNode = talesNetwork.Persons.Add("Мальчик", seekingPersonNode);
			TaleItemNode geeseNode = talesNetwork.Persons.Add("Гуси-лебеди", antagonistNode, Grammem.Plural);
			TaleItemNode stoveNode = talesNetwork.Persons.Add("Печка", donorNode);
			TaleItemNode appleTreeNode = talesNetwork.Persons.Add("Яблоня", donorNode);
			TaleItemNode riverNode = talesNetwork.Persons.Add("Река", donorNode);
			TaleItemNode babaYagaNode = talesNetwork.Persons.Add("Баба-Яга", antagonistNode);
			TaleItemNode cabinOnChickenLegsNode = talesNetwork.Persons.Add("Избушка на курьих ножках", negativePersonNode);

			FunctionNode gtInitialStateNode = geeseTaleNode.Functions.Add("Сказка \"Гуси-лебеди\". Начальная ситуация", initialStateNode);
			gtInitialStateNode.Template = "{Action} {agent0}, {agent1}; у них {~был:agent2} {agent2}, да {~маленький:agent3} {agent3}.";
			gtInitialStateNode.Agents.Add(grandMotherNode, grandFatherNode, girlNode, boyNode);

			FunctionNode gtInterdictionNode = geeseTaleNode.Functions.Add("Сказка \"Гуси-лебеди\". Запрет", interdictionNode);
			gtInterdictionNode.Template = "\"{Recipient}, {recipient}, - {~говорить:agent} {agent}, - мы пойдем на работу, принесем тебе булочку, сошьем платьице, купим платочек: только {action}.\"";
			// TODO: Ошибки генератора. Неправильно определяется время.
			gtInterdictionNode.Actions.Add(talesNetwork.Actions.Add("Не уходить со двора"));
			gtInterdictionNode.Agents.Add(grandMotherNode);
			gtInterdictionNode.Recipients.Add(girlNode);

			FunctionNode gtAbsentationNode = geeseTaleNode.Functions.Add("Сказка \"Гуси-лебеди\". Отлучка старших", absentationNode);
			gtAbsentationNode.Agents.Add(grandMotherNode, grandFatherNode);

			FunctionNode gtViolationOfInterdictionNode = geeseTaleNode.Functions.Add("Сказка \"Гуси-лебеди\". Нарушение запрета", violationOfInterdictionNode);
			gtViolationOfInterdictionNode.Template = "А {agent} {action} про запрет, {~посадил:agent} {recipient} на травку под окошко, а {~сам:agent} {~побежал:agent} на улицу.";
			gtViolationOfInterdictionNode.Actions.Add(talesNetwork.Actions.Add("Забыть"));
			gtViolationOfInterdictionNode.Agents.Add(girlNode);
			gtViolationOfInterdictionNode.Recipients.Add(boyNode);

			FunctionNode gtVillainyNode = geeseTaleNode.Functions.Add("Сказка \"Гуси-лебеди\". Вредительство", villainyNode);
			gtVillainyNode.Template = "{Agent} {action0}, {action1} {recipient}, {action2}.";
			gtVillainyNode.Actions.Add(talesNetwork.Actions.Add("Налететь"), talesNetwork.Actions.Add("Подхватить"), talesNetwork.Actions.Add("Унести"));
			gtVillainyNode.Agents.Add(geeseNode);
			gtVillainyNode.Recipients.Add(boyNode);

			FunctionNode gtMediationNode = geeseTaleNode.Functions.Add("Сказка \"Гуси-лебеди\". Сообщение беды", mediationNode);
			gtMediationNode.Template = "{Agent} {action}, глядь - {recipient} нету.";
			gtMediationNode.Actions.Add(talesNetwork.Actions.Add("Прийти"));
			gtMediationNode.Agents.Add(girlNode);
			gtMediationNode.Recipients.Add(boyNode);

			FunctionNode gtDepartureNode = geeseTaleNode.Functions.Add("Сказка \"Гуси-лебеди\". Отправка на поиски", departureNode);
			gtDepartureNode.Template = "Тогда {agent} {action} {recipient}.";
			gtDepartureNode.Actions.Add(talesNetwork.Actions.Add("Отправилась на поиски"));
			gtDepartureNode.Agents.Add(girlNode);
			gtDepartureNode.Recipients.Add(boyNode);

			FunctionNode gtFirstDonorMeetingNode = geeseTaleNode.Functions.Add("Сказка \"Гуси-лебеди\". Встреча с первым испытателем", donorMeeting);
			gtFirstDonorMeetingNode.Template = "{Agent} {action0}, {action0}. {Recipient0} стоит. {Agent} {~спросить:agent}: \"{Recipient0}, {recipient0}, куда {recipient1} {action1}?\"";
			gtFirstDonorMeetingNode.Actions.Add(talesNetwork.Actions.Add("Бежать"), talesNetwork.Actions.Add("Полететь"));
			gtFirstDonorMeetingNode.Agents.Add(girlNode);
			gtFirstDonorMeetingNode.Recipients.Add(stoveNode, geeseNode);

			FunctionNode gtFirstTestNode = geeseTaleNode.Functions.Add("Сказка \"Гуси-лебеди\". Первое испытание", testNode);
			gtFirstTestNode.Template = "{Agent} {action} {recipient}: \"Съешь моего ржаного пирожка - скажу.\"";
			gtFirstTestNode.Actions.Add(talesNetwork.Actions.Add("Сказать"));
			gtFirstTestNode.Agents.Add(stoveNode);
			gtFirstTestNode.Recipients.Add(girlNode);

			FunctionNode gtFirstTestAttempt = geeseTaleNode.Functions.Add("Сказка \"Гуси-лебеди\". Попытка пройти первое испытание", testAttemptNode);
			gtFirstTestAttempt.Template = "{Agent} {action} {recipient}: \"О, у моего батюшки пшеничные не едятся\".";
			gtFirstTestAttempt.Actions.Add(talesNetwork.Actions.Add("Ответить"));
			gtFirstTestAttempt.Agents.Add(girlNode);
			gtFirstTestAttempt.Recipients.Add(stoveNode);

			FunctionNode gtFirstTestResult = geeseTaleNode.Functions.Add("Сказка \"Гуси-лебеди\". Результат первого испытания", testResultNode);
			gtFirstTestResult.Actions.Add(talesNetwork.Actions.Add("Не сказать"));
			gtFirstTestResult.Agents.Add(stoveNode);
			gtFirstTestResult.Recipients.Add(girlNode);

			//FunctionNode gtFirstTesterMeetingNode = geeseTaleNode.FunctionNodes.Add("Сказка \"Гуси-лебеди\". Встреча с испытателем", testerMeetingNode);
			//network.Edges.Add(gtFirstTesterMeetingNode, testerMeetingNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtFirstTesterMeetingNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtFirstTesterMeetingNode, stoveNode, NetworkEdgeType.Recipient);
			//

			//NetworkNode gtFirstTestNode = network.Nodes.Add("Испытание");
			//network.Edges.Add(gtFirstTestNode, testNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtFirstTestNode, stoveNode, NetworkEdgeType.Agent);

			//NetworkNode gtFirstTestAttemptNode = network.Nodes.Add("Попытка пройти испытание");
			//network.Edges.Add(gtFirstTestAttemptNode, testAttemptNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtFirstTestAttemptNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtFirstTestAttemptNode, stoveNode, NetworkEdgeType.Recipient);

			//NetworkNode gtFirstTestResultNode = network.Nodes.Add("Результат испытания");
			//network.Edges.Add(gtFirstTestResultNode, testResultNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtFirstTestResultNode, stoveNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtFirstTestResultNode, girlNode, NetworkEdgeType.Recipient);

			//NetworkNode gtSecondTesterMeetingNode = network.Nodes.Add("Встреча с испытателем");
			//network.Edges.Add(gtSecondTesterMeetingNode, testerMeetingNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtSecondTesterMeetingNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtSecondTesterMeetingNode, appleTreeNode, NetworkEdgeType.Recipient);

			//NetworkNode gtSecondTestNode = network.Nodes.Add("Испытание");
			//network.Edges.Add(gtSecondTestNode, testNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtSecondTestNode, appleTreeNode, NetworkEdgeType.Agent);

			//NetworkNode gtSecondTestAttemptNode = network.Nodes.Add("Попытка пройти испытание");
			//network.Edges.Add(gtSecondTestAttemptNode, testAttemptNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtSecondTestAttemptNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtSecondTestAttemptNode, appleTreeNode, NetworkEdgeType.Recipient);

			//NetworkNode gtSecondTestFailNode = network.Nodes.Add("Результат испытания");
			//network.Edges.Add(gtSecondTestFailNode, testResultNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtSecondTestFailNode, appleTreeNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtSecondTestFailNode, girlNode, NetworkEdgeType.Recipient);

			//NetworkNode gtThirdTesterMeetingNode = network.Nodes.Add("Встреча с испытателем");
			//network.Edges.Add(gtThirdTesterMeetingNode, testerMeetingNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtThirdTesterMeetingNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtThirdTesterMeetingNode, riverNode, NetworkEdgeType.Recipient);

			//NetworkNode gtThirdTestNode = network.Nodes.Add("Испытание");
			//network.Edges.Add(gtThirdTestNode, testNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtThirdTestNode, riverNode, NetworkEdgeType.Agent);

			//NetworkNode gtThirdTestAttemptNode = network.Nodes.Add("Попытка пройти испытание");
			//network.Edges.Add(gtThirdTestAttemptNode, testAttemptNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtThirdTestAttemptNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtThirdTestAttemptNode, riverNode, NetworkEdgeType.Recipient);

			//NetworkNode gtThirdTestFailNode = network.Nodes.Add("Результат испытания");
			//network.Edges.Add(gtThirdTestFailNode, testResultNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtThirdTestFailNode, riverNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtThirdTestFailNode, girlNode, NetworkEdgeType.Recipient);

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

			//NetworkNode gtAntagonistHomeNode = network.Nodes.Add("Жилище антагониста");
			//network.Edges.Add(gtAntagonistHomeNode, antagonistHomeNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtAntagonistHomeNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtAntagonistHomeNode, cabinOnChickenLegsNode, NetworkEdgeType.Recipient);

			//NetworkNode gtAntagonistMeetingNode = network.Nodes.Add("Облик антагониста");
			//network.Edges.Add(gtAntagonistMeetingNode, antagonistMeetingNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtAntagonistMeetingNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtAntagonistMeetingNode, babaYagaNode, NetworkEdgeType.Recipient);

			//NetworkNode gtDesiredCharacterAppearanceNode = network.Nodes.Add("Появление искомого персонажа");
			//network.Edges.Add(gtDesiredCharacterAppearanceNode, desiredCharacterAppearanceNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtDesiredCharacterAppearanceNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtDesiredCharacterAppearanceNode, boyNode, NetworkEdgeType.Recipient);

			//NetworkNode gtDesiredCharacterLiberationNode = network.Nodes.Add("Добыча искомого персонажа с применением хитрости или силы");
			//network.Edges.Add(gtDesiredCharacterLiberationNode, desiredCharacterLiberationNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtDesiredCharacterLiberationNode, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtDesiredCharacterLiberationNode, boyNode, NetworkEdgeType.Recipient);

			//NetworkNode gtPersecutionBeginningNode = network.Nodes.Add("Начало преследования");
			//network.Edges.Add(gtPersecutionBeginningNode, persecutionBeginningNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtPersecutionBeginningNode, geeseNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtPersecutionBeginningNode, girlNode, NetworkEdgeType.Recipient);

			////TODO Необходимо дублировать данные вершины, в противном случае будет цикл.
			//NetworkNode gtThirdTesterMeeting2Node = network.Nodes.Add("Встреча с испытателем");
			//network.Edges.Add(gtThirdTesterMeeting2Node, testerMeetingNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtThirdTesterMeeting2Node, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtThirdTesterMeeting2Node, riverNode, NetworkEdgeType.Recipient);

			//NetworkNode gtThirdTest2Node = network.Nodes.Add("Испытание");
			//network.Edges.Add(gtThirdTest2Node, testNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtThirdTest2Node, riverNode, NetworkEdgeType.Agent);

			//NetworkNode gtThirdTestAttempt2Node = network.Nodes.Add("Попытка пройти испытание");
			//network.Edges.Add(gtThirdTestAttempt2Node, testAttemptNode, NetworkEdgeType.IsA);
			//network.Edges.Add(gtThirdTestAttempt2Node, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtThirdTestAttempt2Node, riverNode, NetworkEdgeType.Recipient);

			//NetworkNode gtThirdTestSuccessNode = network.Nodes.Add("Результат испытания");
			////TODO Непонятно, как представлять данное предложение.
			//NetworkNode thirdTestSuccessTemplateNode = network.Nodes.Add("{Agent} укрыла ее под кисельным бережком. Гуси-лебеди не увидали, пролетели мимо.");
			//NetworkNode thirdTestSuccessActionNode = network.Nodes.Add("Сказать");
			//network.Edges.Add(gtThirdTestSuccessNode, thirdTestSuccessTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(gtThirdTestSuccessNode, thirdTestSuccessActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(gtThirdTestSuccessNode, riverNode, NetworkEdgeType.Agent);
			//network.Edges.Add(gtThirdTestSuccessNode, girlNode, NetworkEdgeType.Recipient);

			//NetworkNode secondTesterMeeting2Node = network.Nodes.Add("Встреча с испытателем");
			//NetworkNode secondTesterMeeting2TemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			//NetworkNode secondTesterMeeting2ActionNode = network.Nodes.Add("Встретить");
			//network.Edges.Add(secondTesterMeeting2Node, secondTesterMeeting2TemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(secondTesterMeeting2Node, secondTesterMeeting2ActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(secondTesterMeeting2Node, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(secondTesterMeeting2Node, appleTreeNode, NetworkEdgeType.Recipient);

			//NetworkNode secondTest2Node = network.Nodes.Add("Испытание");
			//NetworkNode secondTest2TemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"Поешь моего лесного яблочка - скажу.\".");
			//NetworkNode secondTest2ActionNode = network.Nodes.Add("Встретить");
			//network.Edges.Add(secondTest2Node, secondTest2TemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(secondTest2Node, secondTest2ActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(secondTest2Node, appleTreeNode, NetworkEdgeType.Agent);

			//NetworkNode secondTestAttempt2Node = network.Nodes.Add("Попытка пройти испытание");
			////TODO В этом случае ответил не определяется как сказуемое.
			//NetworkNode secondTestAttempt2TemplateNode = network.Nodes.Add("{Agent} {~ответил:agent} {recipient}: \"У моего батюшки и садовые не едятся...\".");
			//NetworkNode secondTestAttempt2ActionNode = network.Nodes.Add("Встретить");
			//network.Edges.Add(secondTestAttempt2Node, secondTestAttempt2TemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(secondTestAttempt2Node, secondTestAttempt2ActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(secondTestAttempt2Node, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(secondTestAttempt2Node, appleTreeNode, NetworkEdgeType.Recipient);

			//NetworkNode secondTestSuccessNode = network.Nodes.Add("Результат испытания");
			////TODO В этом случае ответил не определяется как сказуемое.
			//NetworkNode secondTestSuccessTemplateNode = network.Nodes.Add("{Agent} не сказала {recipient}.");
			//NetworkNode secondTestSuccessActionNode = network.Nodes.Add("Сказать");
			//network.Edges.Add(secondTestSuccessNode, secondTestSuccessTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(secondTestSuccessNode, secondTestSuccessActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(secondTestSuccessNode, appleTreeNode, NetworkEdgeType.Agent);
			//network.Edges.Add(secondTestSuccessNode, girlNode, NetworkEdgeType.Recipient);

			//NetworkNode firstTesterMeeting2Node = network.Nodes.Add("Встреча с испытателем");
			//NetworkNode firstTesterMeeting2TemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			//NetworkNode firstTesterMeeting2ActionNode = network.Nodes.Add("Встретить");
			//network.Edges.Add(firstTesterMeeting2Node, firstTesterMeeting2TemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(firstTesterMeeting2Node, firstTesterMeeting2ActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(firstTesterMeeting2Node, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(firstTesterMeeting2Node, stoveNode, NetworkEdgeType.Recipient);

			//NetworkNode firstTest2Node = network.Nodes.Add("Испытание");
			//NetworkNode firstTest2TemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"Съешь моего ржаного пирожка - скажу\".");
			//NetworkNode firstTest2ActionNode = network.Nodes.Add("Встретить");
			//network.Edges.Add(firstTest2Node, firstTest2TemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(firstTest2Node, firstTest2ActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(firstTest2Node, stoveNode, NetworkEdgeType.Agent);

			//NetworkNode firstTestAttempt2Node = network.Nodes.Add("Попытка пройти испытание");
			////TODO В этом случае ответил не определяется как сказуемое.
			//NetworkNode firstTestAttempt2TemplateNode = network.Nodes.Add("{Agent} {~ответил:agent} {recipient}: \"О, у моего батюшки пшеничные не едятся\".");
			//NetworkNode firstTestAttempt2ActionNode = network.Nodes.Add("Встретить");
			//network.Edges.Add(firstTestAttempt2Node, firstTestAttempt2TemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(firstTestAttempt2Node, firstTestAttempt2ActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(firstTestAttempt2Node, girlNode, NetworkEdgeType.Agent);
			//network.Edges.Add(firstTestAttempt2Node, stoveNode, NetworkEdgeType.Recipient);

			//NetworkNode firstTestSuccessNode = network.Nodes.Add("Результат испытания");
			////TODO В этом случае ответил не определяется как сказуемое.
			//NetworkNode firstTestSuccessTemplateNode = network.Nodes.Add("{Agent} не сказала {recipient}.");
			//NetworkNode firstTestSuccessActionNode = network.Nodes.Add("Сказать");
			//network.Edges.Add(firstTestSuccessNode, firstTestSuccessTemplateNode, NetworkEdgeType.Template);
			//network.Edges.Add(firstTestSuccessNode, firstTestSuccessActionNode, NetworkEdgeType.Action);
			//network.Edges.Add(firstTestSuccessNode, stoveNode, NetworkEdgeType.Agent);
			//network.Edges.Add(firstTestSuccessNode, girlNode, NetworkEdgeType.Recipient);
			#endregion

			#region Сказка "Сестрица Аленушка и братец Иванушка"

			TaleNode alenushkaTaleNode = talesNetwork.Tales.Add("Сказка \"Сестрица Аленушка и братец Иванушка\"", scenarioTaleNode);

			FunctionNode atInitialStateNode = alenushkaTaleNode.Functions.Add("Сказка \"Сестрица Аленушка и братец Иванушка\". Начальная ситуация", initialStateNode);
			atInitialStateNode.Agents.Add(girlNode, boyNode);

			FunctionNode atProhibitionNode = alenushkaTaleNode.Functions.Add("Сказка \"Сестрица Аленушка и братец Иванушка\". Запрет", interdictionNode);
			atProhibitionNode.Actions.Add(talesNetwork.Actions.Add("Не пей из лужи"));
			atProhibitionNode.Agents.Add(girlNode);
			atProhibitionNode.Recipients.Add(boyNode);

			//FunctionNode atSeniorsDepartureNode = alenushkaTaleNode.FunctionNodes.Add("Уход старших", seniorsDepartureNode);
			//atSeniorsDepartureNode.Agents.Add(grandMotherNode, grandFatherNode);

			FunctionNode atProhibitionViolationNode = alenushkaTaleNode.Functions.Add("Сказка \"Сестрица Аленушка и братец Иванушка\". Нарушение запрета", violationOfInterdictionNode);
			atProhibitionViolationNode.Agents.Add(boyNode);

			//FunctionNode gtSabotageNode = geeseTaleNode.FunctionNodes.Add("Вредительство", sabotageNode);
			//gtSabotageNode.Agents.Add(geeseNode);
			//gtSabotageNode.Recipients.Add(boyNode);
			//gtSabotageNode.Action = "Похитить";

			//FunctionNode gtWoesPostNode = geeseTaleNode.FunctionNodes.Add("Сообщение беды", woesPostNode);
			//gtWoesPostNode.Agents.Add(girlNode);
			//gtWoesPostNode.Recipients.Add(boyNode);

			//FunctionNode gtSearchSubmittingNode = geeseTaleNode.FunctionNodes.Add("Отправка на поиски", searchSubmittingNode);
			//gtSearchSubmittingNode.Agents.Add(girlNode);
			#endregion

			#region Сказка "Братец"

			TaleNode brotherTaleNode = talesNetwork.Tales.Add("Сказка \"Братец\"", scenarioTaleNode);
			TaleItemNode ladyNode = talesNetwork.Persons.Add("Барыня", parentsNode);
			TaleItemNode sisterNode = talesNetwork.Persons.Add("Сестра", heroNode);
			TaleItemNode brotherNode = talesNetwork.Persons.Add("Брат", seekingPersonNode);
			TaleItemNode windNode = talesNetwork.Persons.Add("Ветер", antagonistNode);
			TaleItemNode birchNode = talesNetwork.Persons.Add("Береза", donorNode);

			FunctionNode btInitialStateNode = brotherTaleNode.Functions.Add("Сказка \"Братец\". Начальная ситуация", initialStateNode);
			btInitialStateNode.Agents.Add(ladyNode, sisterNode, brotherNode);

			FunctionNode btIterdictionNode = brotherTaleNode.Functions.Add("Сказка \"Братец\". Запрет", interdictionNode);
			btIterdictionNode.Actions.Add(talesNetwork.Actions.Add("Отпустить брата"));
			btIterdictionNode.Agents.Add(sisterNode);
			btIterdictionNode.Recipients.Add(ladyNode);

			FunctionNode btVillainyNode = brotherTaleNode.Functions.Add("Сказка \"Братец\". Вредительство", villainyNode);
			btVillainyNode.Actions.Add(talesNetwork.Actions.Add("Вырвать из рук няньки"));
			btVillainyNode.Agents.Add(windNode);
			btVillainyNode.Recipients.Add(brotherNode);

			FunctionNode btMediationNode = brotherTaleNode.Functions.Add("Сказка \"Братец\". Сообщение беды", mediationNode);
			//TODO Не учитывается отрицание (частица "не").
			btMediationNode.Actions.Add(talesNetwork.Actions.Add("Не нашла"));
			btMediationNode.Agents.Add(sisterNode);
			btMediationNode.Recipients.Add(brotherNode);

			FunctionNode btDepartureNode = brotherTaleNode.Functions.Add("Сказка \"Братец\". Отправка на поиски", departureNode);
			btDepartureNode.Agents.Add(sisterNode);
			btDepartureNode.Recipients.Add(brotherNode);

			FunctionNode btDonorMeeting = brotherTaleNode.Functions.Add("Сказка \"Братец\". Встреча с испытателем", donorMeeting);
			btDonorMeeting.Template = "\"{Recipient}, {recipient}! Скажи, где мой братец!\"";
			btDonorMeeting.Agents.Add(sisterNode);
			btDonorMeeting.Recipients.Add(birchNode);

			FunctionNode btTestNode = brotherTaleNode.Functions.Add("Сказка \"Братец\". Испытание", testNode);
			btTestNode.Template = "\"Сбери с меня листики, половину возьми себе, половину оставь мне. Я тебе на время пригожусь!\" - сказала {agent}.";
			btTestNode.Agents.Add(birchNode);
			btTestNode.Recipients.Add(sisterNode);

			FunctionNode btTestAttemptNode = brotherTaleNode.Functions.Add("Сказка \"Братец\". Попытка пройти испытание", testAttemptNode);
			btTestAttemptNode.Template = "{Agent} cобрала листики, половину взяла себе, половину оставила {recipient}.";
			btTestAttemptNode.Agents.Add(sisterNode);
			btTestAttemptNode.Recipients.Add(birchNode);
			#endregion

			return talesNetwork;
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

		private static void AnalyzeText(TalesNetwork taleNetwork, TextAnalyzer textAnalyzer, string path)
		{
			string text = File.ReadAllText(path);
			
			//if (foundFunctionNodes.Count != 0)
			//{
			//    TaleNode taleNode = foundFunctionNodes.First().Tale;
			//    bool areMembersOfTheSameTale = foundFunctionNodes.All(node => node.Tale == taleNode);

			//    if (areMembersOfTheSameTale)
			//    {
			//        TaleNode baseTale = (TaleNode)taleNode.BaseNode;
			//        var scenario = baseTale.Scenario;
			//        //TODO Сценарий необходимо хранить в сети.
			//        //List<FunctionType> scenario = new List<FunctionType>
			//        //{
			//        //    FunctionType.InitialState,
			//        //    FunctionType.Interdiction,
			//        //    FunctionType.Absentation,
			//        //    FunctionType.ViolationOfInterdiction,
			//        //    FunctionType.Sabotage,
			//        //    FunctionType.WoesPost
			//        //};
			//        Dictionary<FunctionType, FunctionNode> scenarioFunctions = new Dictionary<FunctionType, FunctionNode>();

			//        foreach (FunctionType functionType in scenario)
			//        {
			//            FunctionNode foundNode = foundFunctionNodes.SingleOrDefault(node => node.FunctionType == functionType);

			//            if (foundNode != null)
			//            {
			//                scenarioFunctions[functionType] = foundNode;
			//            }
			//            else
			//            {
			//                // TODO Необходимо рассмотреть два случая:
			//                // 1) Вершина не смогла полностью разрезолвиться;
			//                // 2) Вершины нет в сети.
			//                // Хотя, наверное, возможно ограничиться только 2-ым вариантом.



			//                TaleNode baseTaleNode = (TaleNode)taleNode.BaseNode;
			//                FunctionNode baseFunctionNode = baseTaleNode.FunctionNodes.SingleOrDefault(node => node.FunctionType == functionType);

			//                if (baseFunctionNode != null)
			//                {
			//                    // В случае, когда в базовой сказке нашлась функциональная вершина с искомым типом, необходимо сделать следующее:
			//                    // 1) Необходимо определить недостающий контекст, т.е. найти те контекстные вершины (агент, реципиент и т.п.),
			//                    //    которых не хватает для успешной генерации текста.
			//                    // 2) Найти дочерние вершины найденных вершин, относящиеся к текущей сказке.

			//                    TemplateParserResult baseNodeParserResult = resolvedNodes[baseFunctionNode];
			//                    TemplateParserDictionaryContext parserContext = new TemplateParserDictionaryContext();

			//                    if (baseNodeParserResult.UnresolvedContext.Any())
			//                    {
			//                        foreach (NetworkEdgeType unresolvedEdgeType in baseNodeParserResult.UnresolvedContext)
			//                        {
			//                            var baseNodes = baseFunctionNode.OutgoingEdges.GetEdges(unresolvedEdgeType, false).Select(edge => edge.EndNode);

			//                            foreach (NetworkNode baseNode in baseNodes)
			//                            {
			//                                NetworkEdge isAEdge = baseNode.IncomingEdges.GetEdge(NetworkEdgeType.IsA);

			//                                if (isAEdge != null)
			//                                {
			//                                    NetworkNode childNode = isAEdge.StartNode;

			//                                    if (childNode != null)
			//                                    {

			//                                    }
			//                                }
			//                            }
			//                        }
			//                    }
			//                    else
			//                    {
									


			//                        NetworkNode agentNode = baseFunctionNode.Agents.First();
			//                        //bool found = false;

			//                        NetworkEdge isAEdge = agentNode.IncomingEdges.GetEdge(NetworkEdgeType.IsA);
			//                        //NetworkEdge 

			//                        if (isAEdge != null)
			//                        {
			//                            NetworkNode childNode = isAEdge.StartNode;

			//                            if (taleAgents.Any(node => node.BaseNode == childNode))
			//                            {
			//                                parserContext.Add(NetworkEdgeType.Agent, childNode);
			//                            }

			//                            //parserContext.Add(NetworkEdgeType.Agent
			//                        }
			//                    }
			//                }
			//                else
			//                {
			//                    throw new NotImplementedException();
			//                }
			//            }
			//        }
			//    }
			//    else
			//    {
			//        throw new NotImplementedException();
			//    }
			//}
		}

		private static TextAnalyzer CreateTextAnalyzer()
		{
			TextAnalyzer textAnalyzer = new TextAnalyzer(AdapterKind.RussianCp1251Adapter);

			textAnalyzer.Load(
				Path.Combine(Environment.CurrentDirectory, "Dictionaries", "Russian", "Dictionary.auto"),
				Path.Combine(Environment.CurrentDirectory, "Dictionaries", "Russian", "Paradigms.bin"),
				Path.Combine(Environment.CurrentDirectory, "Dictionaries", "Russian", "PredictionDictionary.auto"));

			return textAnalyzer;
		}

		public static void Main(string[] args)
		{
			TalesNetwork talesNetwork = CreateNetwork();
			TextAnalyzer textAnalyzer = CreateTextAnalyzer();
			TextGenerator textGenerator = new TextGenerator(textAnalyzer);
			Console.OutputEncoding = Encoding.UTF8;
			bool generateByNetwork = false;

			talesNetwork.SaveToXml().Save(@"C:\Temp\TalesNetwork.xml");

			TalesNetwork loadedNetwork = new TalesNetwork();
			loadedNetwork.LoadFromXml(XDocument.Load(@"C:\Temp\TalesNetwork.xml"));

			if (generateByNetwork)
			{
				string text = textGenerator.GenerateText(loadedNetwork.Tales[1]);
				Console.WriteLine(text);
			}
			else
			{
				File.WriteAllText(@"Output.txt", string.Empty);

				while (true)
				{
					TextGeneratorContext result = textGenerator.GenerateText(loadedNetwork, File.ReadAllText(@"Input.txt"));

					if (result != null)
					{
						string text = result.OutputText;
						File.AppendAllText(@"Output.txt", text + Environment.NewLine);
						Console.WriteLine(text);
					}
					
					Console.WriteLine("Press 'r' to repeat, 'q' to exit.");

					string input = Console.ReadLine();

					if (input.ToLower() == "q")
					{
						break;
					}
				}
			}
		}
		#endregion
	}
}
