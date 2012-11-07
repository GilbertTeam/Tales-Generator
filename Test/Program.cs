using System;
using System.Linq;
using System.Text;
using TalesGenerator.Core;
using TalesGenerator.Core.Collections;
using System.IO;
using System.Text.RegularExpressions;
using TalesGenerator.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Test
{
	public static class Program
	{
		#region Methods

		private static Network CreateNetwork()
		{
			Network network = new Network();
			NetworkNode grandMotherNode = network.Nodes.Add("Старушка");
			NetworkNode grandFatherNode = network.Nodes.Add("Старичок");
			NetworkNode girlNode = network.Nodes.Add("Девочка");
			NetworkNode boyNode = network.Nodes.Add("Мальчик");
			NetworkNode geeseNode = network.Nodes.Add("Гуси-ледеди");
			NetworkNode stoveNode = network.Nodes.Add("Печка");
			NetworkNode appleTreeNode = network.Nodes.Add("Яблоня");
			NetworkNode riverNode = network.Nodes.Add("Река");
			NetworkNode hedgehogNode = network.Nodes.Add("Ежик");
			NetworkNode babaYagaNode = network.Nodes.Add("Баба-Яга");
			NetworkNode cabinOnChickenLegsNode = network.Nodes.Add("Избушка на курьих ножках");

			//NetworkNode initialStateNode = network.Nodes.Add("Начальная ситуация");
			//NetworkNode gtInitialStateTemplateNode = network.Nodes.Add("{Action} {agents}.");
			//NetworkNode gtInitialStateActionNode = network.Nodes.Add("Жили-были");
			//NetworkNode prohibitionNode = network.Nodes.Add("Запрет");
			//NetworkNode seniorsDepartureNode = network.Nodes.Add("Уход старших");
			//NetworkNode prohibitionViolationNode = network.Nodes.Add("Нарушение запрета");
			//NetworkNode sabotageNode = network.Nodes.Add("Вредительство");
			//NetworkNode woesPostNode = network.Nodes.Add("Сообщение беды");
			//NetworkNode searchSubmittingNode = network.Nodes.Add("Отправка на поиски");
			//NetworkNode firstTesterMeetingNode = network.Nodes.Add("Встреча с испытателем");
			//NetworkNode firstTestNode = network.Nodes.Add("Испытание");

			NetworkNode gtInitialStateNode = network.Nodes.Add("Начальная ситуация");
			NetworkNode gtInitialStateTemplateNode = network.Nodes.Add("{Action} {agents}.");
			NetworkNode gtInitialStateActionNode = network.Nodes.Add("Жили-были");
			network.Edges.Add(gtInitialStateNode, gtInitialStateTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(gtInitialStateNode, gtInitialStateActionNode, NetworkEdgeType.Action);
			network.Edges.Add(gtInitialStateNode, grandMotherNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtInitialStateNode, grandFatherNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtInitialStateNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtInitialStateNode, boyNode, NetworkEdgeType.Agent);

			NetworkNode gtProhibitionNode = network.Nodes.Add("Запрет");
			NetworkNode gtProhibitionTemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"{Recipient}, {action}.\"");
			NetworkNode gtProhibitionActionNode = network.Nodes.Add("Не уходи со двора");
			network.Edges.Add(gtProhibitionNode, gtProhibitionTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(gtProhibitionNode, gtProhibitionActionNode, NetworkEdgeType.Action);
			network.Edges.Add(gtProhibitionNode, grandMotherNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtProhibitionNode, girlNode, NetworkEdgeType.Recipient);

			NetworkNode gtSeniorsDepartureNode = network.Nodes.Add("Уход старших");
			NetworkNode gtSeniorsDepartureTemplateNode = network.Nodes.Add("{Agents} {action}.");
			NetworkNode gtSeniorsDepartureActionNode = network.Nodes.Add("Уходить");
			network.Edges.Add(gtSeniorsDepartureNode, gtSeniorsDepartureTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(gtSeniorsDepartureNode, gtSeniorsDepartureActionNode, NetworkEdgeType.Action);
			network.Edges.Add(gtSeniorsDepartureNode, grandMotherNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtSeniorsDepartureNode, grandFatherNode, NetworkEdgeType.Agent);

			NetworkNode gtProhibitionViolationNode = network.Nodes.Add("Нарушение запрета");
			NetworkNode gtProhibitionViolationTemplateNode = network.Nodes.Add("{Agent} {action}.");
			NetworkNode gtProhibitionViolationActionNode = network.Nodes.Add("Нарушить запрет");
			network.Edges.Add(gtProhibitionViolationNode, gtProhibitionViolationTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(gtProhibitionViolationNode, gtProhibitionViolationActionNode, NetworkEdgeType.Action);
			network.Edges.Add(gtProhibitionViolationNode, girlNode, NetworkEdgeType.Agent);

			NetworkNode gtSabotageNode = network.Nodes.Add("Вредительство");
			NetworkNode gtSabotageTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode gtSabotageActionNode = network.Nodes.Add("Похитить");
			network.Edges.Add(gtSabotageNode, gtSabotageTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(gtSabotageNode, gtSabotageActionNode, NetworkEdgeType.Action);
			network.Edges.Add(gtSabotageNode, geeseNode, NetworkEdgeType.Agent);
			network.Edges.Add(gtSabotageNode, boyNode, NetworkEdgeType.Recipient);

			NetworkNode woesPostNode = network.Nodes.Add("Сообщение беды");
			NetworkNode woesPostTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode woesPostActionNode = network.Nodes.Add("Обнаружить пропажу");
			network.Edges.Add(woesPostNode, woesPostTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(woesPostNode, woesPostActionNode, NetworkEdgeType.Action);
			network.Edges.Add(woesPostNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(woesPostNode, boyNode, NetworkEdgeType.Recipient);

			NetworkNode searchSubmittingNode = network.Nodes.Add("Отправка на поиски");
			NetworkNode searchSubmittingTemplateNode = network.Nodes.Add("{Agent} {action}.");
			NetworkNode searchSubmittingActionNode = network.Nodes.Add("Отправилась на поиски");
			network.Edges.Add(searchSubmittingNode, searchSubmittingTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(searchSubmittingNode, searchSubmittingActionNode, NetworkEdgeType.Action);
			network.Edges.Add(searchSubmittingNode, girlNode, NetworkEdgeType.Agent);

			NetworkNode firstTesterMeetingNode = network.Nodes.Add("Встреча с испытателем");
			NetworkNode firstTesterMeetingTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode firstTesterMeetingActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(firstTesterMeetingNode, firstTesterMeetingTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(firstTesterMeetingNode, firstTesterMeetingActionNode, NetworkEdgeType.Action);
			network.Edges.Add(firstTesterMeetingNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(firstTesterMeetingNode, stoveNode, NetworkEdgeType.Recipient);

			NetworkNode firstTestNode = network.Nodes.Add("Испытание");
			NetworkNode firstTestTemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"Съешь моего ржаного пирожка - скажу\".");
			NetworkNode firstTestActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(firstTestNode, firstTestTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(firstTestNode, firstTestActionNode, NetworkEdgeType.Action);
			network.Edges.Add(firstTestNode, stoveNode, NetworkEdgeType.Agent);

			NetworkNode firstTestAttemptNode = network.Nodes.Add("Попытка пройти испытание");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode firstTestAttemptTemplateNode = network.Nodes.Add("{Agent} {~ответил:agent} {recipient}: \"О, у моего батюшки пшеничные не едятся\".");
			NetworkNode firstTestAttemptActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(firstTestAttemptNode, firstTestAttemptTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(firstTestAttemptNode, firstTestAttemptActionNode, NetworkEdgeType.Action);
			network.Edges.Add(firstTestAttemptNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(firstTestAttemptNode, stoveNode, NetworkEdgeType.Recipient);

			NetworkNode firstTestFailNode = network.Nodes.Add("Результат испытания");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode firstTestFailTemplateNode = network.Nodes.Add("{Agent} не сказала {recipient}.");
			NetworkNode firstTestFailActionNode = network.Nodes.Add("Сказать");
			network.Edges.Add(firstTestFailNode, firstTestFailTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(firstTestFailNode, firstTestFailActionNode, NetworkEdgeType.Action);
			network.Edges.Add(firstTestFailNode, stoveNode, NetworkEdgeType.Agent);
			network.Edges.Add(firstTestFailNode, girlNode, NetworkEdgeType.Recipient);

			NetworkNode secondTesterMeetingNode = network.Nodes.Add("Встреча с испытателем");
			NetworkNode secondTesterMeetingTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode secondTesterMeetingActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(secondTesterMeetingNode, secondTesterMeetingTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(secondTesterMeetingNode, secondTesterMeetingActionNode, NetworkEdgeType.Action);
			network.Edges.Add(secondTesterMeetingNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(secondTesterMeetingNode, appleTreeNode, NetworkEdgeType.Recipient);

			NetworkNode secondTestNode = network.Nodes.Add("Испытание");
			NetworkNode secondTestTemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"Поешь моего лесного яблочка - скажу.\".");
			NetworkNode secondTestActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(secondTestNode, secondTestTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(secondTestNode, secondTestActionNode, NetworkEdgeType.Action);
			network.Edges.Add(secondTestNode, appleTreeNode, NetworkEdgeType.Agent);

			NetworkNode secondTestAttemptNode = network.Nodes.Add("Попытка пройти испытание");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode secondTestAttemptTemplateNode = network.Nodes.Add("{Agent} {~ответил:agent} {recipient}: \"У моего батюшки и садовые не едятся...\".");
			NetworkNode secondTestAttemptActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(secondTestAttemptNode, secondTestAttemptTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(secondTestAttemptNode, secondTestAttemptActionNode, NetworkEdgeType.Action);
			network.Edges.Add(secondTestAttemptNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(secondTestAttemptNode, appleTreeNode, NetworkEdgeType.Recipient);

			NetworkNode secondTestFailNode = network.Nodes.Add("Результат испытания");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode secondTestFailTemplateNode = network.Nodes.Add("{Agent} не сказала {recipient}.");
			NetworkNode secondTestFailActionNode = network.Nodes.Add("Сказать");
			network.Edges.Add(secondTestFailNode, secondTestFailTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(secondTestFailNode, secondTestFailActionNode, NetworkEdgeType.Action);
			network.Edges.Add(secondTestFailNode, appleTreeNode, NetworkEdgeType.Agent);
			network.Edges.Add(secondTestFailNode, girlNode, NetworkEdgeType.Recipient);

			NetworkNode thirdTesterMeetingNode = network.Nodes.Add("Встреча с испытателем");
			NetworkNode thirdTesterMeetingTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode thirdTesterMeetingActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(thirdTesterMeetingNode, thirdTesterMeetingTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(thirdTesterMeetingNode, thirdTesterMeetingActionNode, NetworkEdgeType.Action);
			network.Edges.Add(thirdTesterMeetingNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(thirdTesterMeetingNode, riverNode, NetworkEdgeType.Recipient);

			NetworkNode thirdTestNode = network.Nodes.Add("Испытание");
			NetworkNode thirdTestTemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"Поешь моего простого киселька с молочком - скажу.\".");
			NetworkNode thirdTestActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(thirdTestNode, thirdTestTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(thirdTestNode, thirdTestActionNode, NetworkEdgeType.Action);
			network.Edges.Add(thirdTestNode, riverNode, NetworkEdgeType.Agent);

			NetworkNode thirdTestAttemptNode = network.Nodes.Add("Попытка пройти испытание");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode thirdTestAttemptTemplateNode = network.Nodes.Add("{Agent} {~ответил:agent} {recipient}: \"У моего батюшки и сливочки не едятся...\".");
			NetworkNode thirdTestAttemptActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(thirdTestAttemptNode, thirdTestAttemptTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(thirdTestAttemptNode, thirdTestAttemptActionNode, NetworkEdgeType.Action);
			network.Edges.Add(thirdTestAttemptNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(thirdTestAttemptNode, riverNode, NetworkEdgeType.Recipient);

			NetworkNode thirdTestFailNode = network.Nodes.Add("Результат испытания");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode thirdTestFailTemplateNode = network.Nodes.Add("{Agent} не {action} {recipient}.");
			NetworkNode thirdTestFailActionNode = network.Nodes.Add("Сказать");
			network.Edges.Add(thirdTestFailNode, thirdTestFailTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(thirdTestFailNode, thirdTestFailActionNode, NetworkEdgeType.Action);
			network.Edges.Add(thirdTestFailNode, riverNode, NetworkEdgeType.Agent);
			network.Edges.Add(thirdTestFailNode, girlNode, NetworkEdgeType.Recipient);

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

			NetworkNode antagonistHomeNode = network.Nodes.Add("Жилище антагониста");
			NetworkNode antagonistHomeTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode antagonistHomeActionNode = network.Nodes.Add("Увидеть");
			network.Edges.Add(antagonistHomeNode, antagonistHomeTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(antagonistHomeNode, antagonistHomeActionNode, NetworkEdgeType.Action);
			network.Edges.Add(antagonistHomeNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(antagonistHomeNode, cabinOnChickenLegsNode, NetworkEdgeType.Recipient);

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
			network.Edges.Add(desiredCharacterAppearanceNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(desiredCharacterAppearanceNode, boyNode, NetworkEdgeType.Recipient);

			NetworkNode desiredCharacterLiberationNode = network.Nodes.Add("Добыча искомого персонажа с применением хитрости или силы");
			NetworkNode desiredCharacterLiberationTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode desiredCharacterLiberationActionNode = network.Nodes.Add("Схватить и унести");
			network.Edges.Add(desiredCharacterLiberationNode, desiredCharacterLiberationTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(desiredCharacterLiberationNode, desiredCharacterLiberationActionNode, NetworkEdgeType.Action);
			network.Edges.Add(desiredCharacterLiberationNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(desiredCharacterLiberationNode, boyNode, NetworkEdgeType.Recipient);

			NetworkNode persecutionBeginningNode = network.Nodes.Add("Начало преследования");
			NetworkNode persecutionBeginningTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode persecutionBeginningActionNode = network.Nodes.Add("Начать преследовать");
			network.Edges.Add(persecutionBeginningNode, persecutionBeginningTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(persecutionBeginningNode, persecutionBeginningActionNode, NetworkEdgeType.Action);
			network.Edges.Add(persecutionBeginningNode, geeseNode, NetworkEdgeType.Agent);
			network.Edges.Add(persecutionBeginningNode, girlNode, NetworkEdgeType.Recipient);

			//TODO Необходимо дублировать данные вершины, в противном случае будет цикл.
			NetworkNode thirdTesterMeeting2Node = network.Nodes.Add("Встреча с испытателем");
			NetworkNode thirdTesterMeeting2TemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode thirdTesterMeeting2ActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(thirdTesterMeeting2Node, thirdTesterMeeting2TemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(thirdTesterMeeting2Node, thirdTesterMeeting2ActionNode, NetworkEdgeType.Action);
			network.Edges.Add(thirdTesterMeeting2Node, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(thirdTesterMeeting2Node, riverNode, NetworkEdgeType.Recipient);

			NetworkNode thirdTest2Node = network.Nodes.Add("Испытание");
			NetworkNode thirdTest2TemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"Поешь моего простого киселька.\".");
			NetworkNode thirdTest2ActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(thirdTest2Node, thirdTest2TemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(thirdTest2Node, thirdTest2ActionNode, NetworkEdgeType.Action);
			network.Edges.Add(thirdTest2Node, riverNode, NetworkEdgeType.Agent);

			NetworkNode thirdTestAttempt2Node = network.Nodes.Add("Попытка пройти испытание");
			//TODO В этом случае ответил не определяется как сказуемое.
			NetworkNode thirdTestAttempt2TemplateNode = network.Nodes.Add("{Agent} поела и спасибо сказала.");
			NetworkNode thirdTestAttempt2ActionNode = network.Nodes.Add("Встретить");
			network.Edges.Add(thirdTestAttempt2Node, thirdTestAttempt2TemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(thirdTestAttempt2Node, thirdTestAttempt2ActionNode, NetworkEdgeType.Action);
			network.Edges.Add(thirdTestAttempt2Node, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(thirdTestAttempt2Node, riverNode, NetworkEdgeType.Recipient);

			NetworkNode thirdTestSuccessNode = network.Nodes.Add("Результат испытания");
			//TODO Непонятно, как представлять данное предложение.
			NetworkNode thirdTestSuccessTemplateNode = network.Nodes.Add("{Agent} укрыла ее под кисельным бережком. Гуси-лебеди не увидали, пролетели мимо.");
			NetworkNode thirdTestSuccessActionNode = network.Nodes.Add("Сказать");
			network.Edges.Add(thirdTestSuccessNode, thirdTestSuccessTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(thirdTestSuccessNode, thirdTestSuccessActionNode, NetworkEdgeType.Action);
			network.Edges.Add(thirdTestSuccessNode, riverNode, NetworkEdgeType.Agent);
			network.Edges.Add(thirdTestSuccessNode, girlNode, NetworkEdgeType.Recipient);

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

			network.Edges.Add(gtInitialStateNode, gtProhibitionNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtProhibitionNode, gtSeniorsDepartureNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtSeniorsDepartureNode, gtProhibitionViolationNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtProhibitionViolationNode, gtSabotageNode, NetworkEdgeType.Follow);
			network.Edges.Add(gtSabotageNode, woesPostNode, NetworkEdgeType.Follow);
			network.Edges.Add(woesPostNode, searchSubmittingNode, NetworkEdgeType.Follow);
			network.Edges.Add(searchSubmittingNode, firstTesterMeetingNode, NetworkEdgeType.Follow);
			network.Edges.Add(firstTesterMeetingNode, firstTestNode, NetworkEdgeType.Follow);
			network.Edges.Add(firstTestNode, firstTestAttemptNode, NetworkEdgeType.Follow);
			network.Edges.Add(firstTestAttemptNode, firstTestFailNode, NetworkEdgeType.Follow);
			network.Edges.Add(firstTestFailNode, secondTesterMeetingNode, NetworkEdgeType.Follow);
			network.Edges.Add(secondTesterMeetingNode, secondTestNode, NetworkEdgeType.Follow);
			network.Edges.Add(secondTestNode, secondTestAttemptNode, NetworkEdgeType.Follow);
			network.Edges.Add(secondTestAttemptNode, secondTestFailNode, NetworkEdgeType.Follow);
			network.Edges.Add(secondTestFailNode, thirdTesterMeetingNode, NetworkEdgeType.Follow);
			network.Edges.Add(thirdTesterMeetingNode, thirdTestNode, NetworkEdgeType.Follow);
			network.Edges.Add(thirdTestNode, thirdTestAttemptNode, NetworkEdgeType.Follow);
			network.Edges.Add(thirdTestAttemptNode, thirdTestFailNode, NetworkEdgeType.Follow);
			network.Edges.Add(thirdTestFailNode, antagonistHomeNode, NetworkEdgeType.Follow);
			network.Edges.Add(antagonistHomeNode, antagonistMeetingNode, NetworkEdgeType.Follow);
			network.Edges.Add(antagonistMeetingNode, desiredCharacterAppearanceNode, NetworkEdgeType.Follow);
			network.Edges.Add(desiredCharacterAppearanceNode, desiredCharacterLiberationNode, NetworkEdgeType.Follow);
			network.Edges.Add(desiredCharacterLiberationNode, persecutionBeginningNode, NetworkEdgeType.Follow);
			network.Edges.Add(persecutionBeginningNode, thirdTesterMeeting2Node, NetworkEdgeType.Follow);
			network.Edges.Add(thirdTesterMeeting2Node, thirdTest2Node, NetworkEdgeType.Follow);
			network.Edges.Add(thirdTest2Node, thirdTestAttempt2Node, NetworkEdgeType.Follow);
			network.Edges.Add(thirdTestAttempt2Node, thirdTestSuccessNode, NetworkEdgeType.Follow);

			return network;
		}

		private static void AnalyzeText(string path, Network network)
		{
			TextAnalyzer textAnalyzer = new TextAnalyzer(AdapterKind.RussianCp1251Adapter);
			textAnalyzer.Load(
				@"C:\Projects\Git\libturglem\Dictionaries\dict_russian.auto",
				@"C:\Projects\Git\libturglem\Dictionaries\paradigms_russian.bin",
				@"C:\Projects\Git\libturglem\Dictionaries\prediction_russian.auto");

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

			using (TextAnalyzer textAnalyzer = new TextAnalyzer(AdapterKind.RussianCp1251Adapter))
			{
				textAnalyzer.Load(
					@"C:\Projects\Git\libturglem\Dictionaries\dict_russian.auto",
					@"C:\Projects\Git\libturglem\Dictionaries\paradigms_russian.bin",
					@"C:\Projects\Git\libturglem\Dictionaries\prediction_russian.auto");

				TextGenerator textGenerator = new TextGenerator(textAnalyzer);

				//TODO Необходимо определиться с тем, каким образом будет начинаться процесс генерации.
				NetworkNode startNode = network.Nodes.SingleOrDefault(node => node.Name == "Начальная ситуация");

				string text = textGenerator.GenerateText(startNode);
			}
		}
		#endregion
	}
}
