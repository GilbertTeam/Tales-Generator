using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TalesGenerator.Text;
using TalesGenerator.Core;
using TalesGenerator.Core.Collections;
using System.Text.RegularExpressions;

namespace Test
{
	public static class Program
	{
		#region Fields

		private static TextAnalyzer _textAnalyzer = new TextAnalyzer(AdapterKind.RussianCp1251Adapter);
		#endregion

		#region Methods

		private static Network CreateNetwork()
		{
			Network network = new Network();
			NetworkNode grandMotherNode = network.Nodes.Add("Старушка");
			NetworkNode grandFatherNode = network.Nodes.Add("Старичок");
			NetworkNode girlNode = network.Nodes.Add("Девочка");
			NetworkNode boyNode = network.Nodes.Add("Мальчик");
			NetworkNode geeseNode = network.Nodes.Add("Гуси-ледеди");

			NetworkNode initialStateNode = network.Nodes.Add("Начальная ситуация");
			NetworkNode initialStateTemplateNode = network.Nodes.Add("{Action} {agents}.");
			NetworkNode initialStateActionNode = network.Nodes.Add("Жили-были");
			network.Edges.Add(initialStateNode, initialStateTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(initialStateNode, initialStateActionNode, NetworkEdgeType.Action);
			network.Edges.Add(initialStateNode, grandMotherNode, NetworkEdgeType.Agent);
			network.Edges.Add(initialStateNode, grandFatherNode, NetworkEdgeType.Agent);
			network.Edges.Add(initialStateNode, girlNode, NetworkEdgeType.Agent);
			network.Edges.Add(initialStateNode, boyNode, NetworkEdgeType.Agent);

			NetworkNode prohibitionNode = network.Nodes.Add("Запрет");
			NetworkNode prohibitionTemplateNode = network.Nodes.Add("{Agent} {~сказал:agent}: \"{Recipient}, {action}.\"");
			NetworkNode prohibitionActionNode = network.Nodes.Add("Не уходи со двора");
			network.Edges.Add(prohibitionNode, prohibitionTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(prohibitionNode, prohibitionActionNode, NetworkEdgeType.Action);
			network.Edges.Add(prohibitionNode, grandMotherNode, NetworkEdgeType.Agent);
			network.Edges.Add(prohibitionNode, girlNode, NetworkEdgeType.Recipient);

			NetworkNode prohibitionViolationNode = network.Nodes.Add("Нарушение запрета");
			NetworkNode prohibitionViolationTemplateNode = network.Nodes.Add("{Agent} {action}.");
			NetworkNode prohibitionViolationActionNode = network.Nodes.Add("Нарушить запрет");
			network.Edges.Add(prohibitionViolationNode, prohibitionViolationTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(prohibitionViolationNode, prohibitionViolationActionNode, NetworkEdgeType.Action);
			network.Edges.Add(prohibitionViolationNode, girlNode, NetworkEdgeType.Agent);

			NetworkNode sabotageNode = network.Nodes.Add("Вредительство");
			NetworkNode sabotageNodeTemplateNode = network.Nodes.Add("{Agent} {action} {recipient}.");
			NetworkNode sabotageNodeActionNode = network.Nodes.Add("Похитить");
			network.Edges.Add(sabotageNode, sabotageNodeTemplateNode, NetworkEdgeType.Template);
			network.Edges.Add(sabotageNode, sabotageNodeActionNode, NetworkEdgeType.Action);
			network.Edges.Add(sabotageNode, geeseNode, NetworkEdgeType.Agent);
			network.Edges.Add(sabotageNode, boyNode, NetworkEdgeType.Recipient);

			network.Edges.Add(initialStateNode, prohibitionNode, NetworkEdgeType.Follow);
			network.Edges.Add(prohibitionNode, prohibitionViolationNode, NetworkEdgeType.Follow);
			network.Edges.Add(prohibitionViolationNode, sabotageNode, NetworkEdgeType.Follow);

			return network;
		}

		private static string ParseGrammemTemplate(string value, string grammem)
		{
			var results = _textAnalyzer.Lemmatize(value, true);

			if (results == null)
			{
				throw new InvalidOperationException();
			}

			LemmatizeResult result = results.First();
			ulong grammemValue = ulong.Parse(grammem);

			var texts = result.GetTextByGrammem((Grammem)grammemValue);

			if (texts == null)
			{
				throw new InvalidOperationException();
			}

			return texts.FirstOrDefault();
		}

		private static string ParseGrammemMatch(Match match, string value, bool ignoreCase = false)
		{
			string text = string.Empty;

			if (match.Groups[1].Success)
			{
				text = ParseGrammemTemplate(value, match.Groups[1].Value.Substring(1));
			}
			else
			{
				text = value;
			}

			if (!ignoreCase)
			{
				if (char.IsUpper(match.Groups[0].Value, 1))
				{
					text = char.ToUpper(text[0]) + text.Substring(1);
				}
				else
				{
					text = char.ToLower(text[0]) + text.Substring(1);
				}
			}

			return text;
		}

		private static string GenerateText(NetworkNode node)
		{
			NetworkNode templateNode = node.OutgoingEdges.GetEdge(NetworkEdgeType.Template).EndNode;
			NetworkNode actionNode = node.OutgoingEdges.GetEdge(NetworkEdgeType.Action).EndNode;
			string template = templateNode.Name;
			string action = actionNode.Name;

			template = Regex.Replace(template, @"{[Aa]ction(:\d+?)?}",
				(match) =>
				{
					return ParseGrammemMatch(match, node.OutgoingEdges.GetEdge(NetworkEdgeType.Action).EndNode.Name);
				}
			);

			template = Regex.Replace(template, @"{[Aa]gent(:\d+?)?}",
				(match) =>
				{
					return ParseGrammemMatch(match, node.OutgoingEdges.GetEdge(NetworkEdgeType.Agent).EndNode.Name);
				}
			);

			template = Regex.Replace(template, @"{[Aa]gents(:\d+?)?}",
				(match) =>
				{
					var agents = node.OutgoingEdges.GetEdges(NetworkEdgeType.Agent).Select(edge => edge.EndNode.Name);
					StringBuilder stringBuilder = new StringBuilder();

					if (agents.Count() == 0)
					{
						throw new InvalidOperationException();
					}

					stringBuilder.Append(ParseGrammemMatch(match, agents.First()));

					foreach (var agent in agents.Skip(1))
					{
						stringBuilder.AppendFormat(", {0}", ParseGrammemMatch(match, agent.ToLower(), true));
					}

					return stringBuilder.ToString();
				}
			);

			template = Regex.Replace(template, @"{[Rr]ecipient(:\d+?)?}",
				(match) =>
				{
					return ParseGrammemMatch(match, node.OutgoingEdges.GetEdge(NetworkEdgeType.Recipient).EndNode.Name);
				}
			);

			template = Regex.Replace(template, @"{[Rr]ecipients(:\d+?)?}",
				(match) =>
				{
					var recipients = node.OutgoingEdges.GetEdges(NetworkEdgeType.Recipient).Select(edge => edge.EndNode.Name);
					StringBuilder stringBuilder = new StringBuilder();

					if (recipients.Count() == 0)
					{
						throw new InvalidOperationException();
					}

					stringBuilder.Append(ParseGrammemMatch(match, recipients.First()));

					foreach (var recipient in recipients.Skip(1))
					{
						stringBuilder.AppendFormat(", {0}", ParseGrammemMatch(match, recipient.ToLower(), true));
					}

					return stringBuilder.ToString();
				}
			);

			template = Regex.Replace(template, @"{~(\w+?):(\w+?)}",
				(match) =>
				{
					string wordSample = match.Groups[2].Value;

					switch (wordSample)
					{
						case "agent":
							wordSample = node.OutgoingEdges.GetEdge(NetworkEdgeType.Agent).EndNode.Name;
							break;

						default:
							throw new InvalidOperationException();
					}

					var results = _textAnalyzer.Lemmatize(wordSample, true);

					if (results == null)
					{
						throw new InvalidOperationException();
					}

					LemmatizeResult result = results.First();
					Grammem sampleGrammem = result.GetGrammem();
					Grammem resultGrammem = sampleGrammem & Grammem.Plural;

					resultGrammem |= sampleGrammem & Grammem.Singular;
					resultGrammem |= sampleGrammem & Grammem.Masculinum;
					resultGrammem |= sampleGrammem & Grammem.Feminum;
					resultGrammem |= sampleGrammem & Grammem.Neutrum;
					resultGrammem |= sampleGrammem & Grammem.MascFem;

					results = _textAnalyzer.Lemmatize(match.Groups[1].Value.Trim('~', ':'), true);

					if (results == null)
					{
						throw new InvalidOperationException();
					}

					result = results.First();
					var texts = result.GetTextByGrammem(resultGrammem);

					if (texts == null)
					{
						throw new InvalidOperationException();
					}

					//TODO Игнорируется регистр первой буквы
					return texts.First().ToLower();
				}
			);

			return template;
		}

		public static void Main(string[] args)
		{
			_textAnalyzer.Load(
				@"C:\Projects\Git\libturglem\Dictionaries\dict_russian.auto",
				@"C:\Projects\Git\libturglem\Dictionaries\paradigms_russian.bin",
				@"C:\Projects\Git\libturglem\Dictionaries\prediction_russian.auto");

			Network network = CreateNetwork();
			//TODO Необходимо определиться с тем, каким образом будет начинаться процесс генерации.
			NetworkNode currentNode = network.Nodes.SingleOrDefault(node => node.Name == "Начальная ситуация");
			StringBuilder textBuilder = new StringBuilder();

			while (currentNode != null)
			{
				textBuilder.AppendFormat("{0} ", GenerateText(currentNode));
				NetworkEdge followEdge = currentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Follow);
				currentNode = followEdge != null ? followEdge.EndNode : null;
			}

			Console.WriteLine(textBuilder.ToString().TrimEnd(' '));
		}
		#endregion
	}
}
