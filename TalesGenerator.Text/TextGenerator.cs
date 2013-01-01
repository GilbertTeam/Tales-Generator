using System;
using System.Linq;
using System.Text;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;
using TalesGenerator.TaleNet;
using System.Collections.Generic;
using MoreLinq;

namespace TalesGenerator.Text
{
	public class TextGenerator
	{
		#region Fields

		private readonly TextAnalyzer _textAnalyzer;

		private readonly TemplateParser _templateParser;
		#endregion

		#region Constructors

		public TextGenerator(TextAnalyzer textAnalyzer)
		{
			if (textAnalyzer == null)
			{
				throw new ArgumentNullException("textAnalyzer");
			}

			_textAnalyzer = textAnalyzer;
			_templateParser = new TemplateParser(_textAnalyzer);
		}
		#endregion

		#region Methods

		private void ResolveText(
			TalesNetwork talesNetwork,
			string text,
			out IList<FunctionNode> resolvedFunctionNodes,
			out IDictionary<FunctionNode, TemplateParserResult> parserResults)
		{
			string[] sentences = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			ExactMatchSentenceComparer sentenceComparer = new ExactMatchSentenceComparer();
			parserResults = new Dictionary<FunctionNode, TemplateParserResult>();
			resolvedFunctionNodes = new List<FunctionNode>();

			TextParser textParser = new TextParser(_textAnalyzer);
			IEnumerable<IEnumerable<SentenceToken>> textTokens = textParser.Parse(text);

			//foreach (string sentence in sentences)
			//{
			//    string trimSentence = sentence.Trim('.', '\r', '\n');

			//    foreach (TaleNode taleNode in talesNetwork.Tales)
			//    {
			//        foreach (FunctionNode functionNode in taleNode.Functions)
			//        {
			//            TemplateParserResult parserResult;

			//            if (!parserResults.TryGetValue(functionNode, out parserResult))
			//            {
			//                parserResult = _templateParser.Parse(functionNode);
			//                parserResults[functionNode] = parserResult;
			//            }

			//            if (sentenceComparer.Compare(parserResult.Text, trimSentence))
			//            {
			//                resolvedFunctionNodes.Add(functionNode);
			//            }
			//        }
			//    }
			//}

			// TODO: Возможно стоит предварительно проверять на то, не содержит ли входной текст просто имена героев.

			IEqualityComparer<SentenceToken> sentenceTokenEqualityComparer = new SentenceTokenDefaultEqualityComparer();

			foreach (var sentenceTokens in textTokens)
			{
				foreach (TaleNode taleNode in talesNetwork.Tales)
				{
					foreach (FunctionNode functionNode in taleNode.Functions)
					{
						TemplateParserResult parserResult;

						if (!parserResults.TryGetValue(functionNode, out parserResult))
						{
							parserResult = _templateParser.Parse(functionNode);
							parserResults[functionNode] = parserResult;
						}

						var parserTemplateTokens = parserResult.TemplateTokens.Aggregate<IEnumerable<SentenceToken>>((result, next) => result.Concat(next));

						if (sentenceTokens.SequenceEqual(parserTemplateTokens, sentenceTokenEqualityComparer))
						{
							resolvedFunctionNodes.Add(functionNode);
						}

						if (sentenceTokens.All(token => parserTemplateTokens.Contains(token, sentenceTokenEqualityComparer)))
						{
							resolvedFunctionNodes.Add(functionNode);
						}
					}
				}
			}
		}

		private void GetChildContextNode(
			IDictionary<FunctionNode, TemplateParserResult> parserResults,
			TaleNode currentTaleNode,
			FunctionNode baseFunctionNode,
			NetworkEdgeType edgeType,
			TemplateParserNetworkDictionaryContext newParserContext)
		{
			TemplateParserResult baseFunctionNodeParserResult = parserResults[baseFunctionNode];
			var currentTaleContextNodes = new List<NetworkNode>();
			var baseTaleContextNodes = baseFunctionNode.OutgoingEdges.GetEdges(edgeType).Select(edge => edge.EndNode);
			bool found = false;

			foreach (FunctionNode functionNode in currentTaleNode.Functions)
			{
				currentTaleContextNodes.AddRange(functionNode.OutgoingEdges.GetEdges(edgeType).Select(edge => edge.EndNode));
			}

			foreach (NetworkNode baseTaleContextNode in baseTaleContextNodes)
			{
				if (currentTaleContextNodes.Any(node => node.IsInherit(baseTaleContextNode, false)))
				{
					NetworkNode currentNode = null;

					while (true)
					{
						NetworkEdge isAEdge = baseTaleContextNode.OutgoingEdges.GetEdge(NetworkEdgeType.IsA);

						if (isAEdge != null)
						{
							currentNode = isAEdge.StartNode;

							NetworkNode childNode = currentTaleContextNodes.FirstOrDefault(node => node == currentNode || node.BaseNode == currentNode);

							if (childNode != null)
							{
								found = true;
								newParserContext.Add(edgeType, childNode);
								break;
							}
						}

						if (isAEdge == null &&
							currentNode != null)
						{
							found = true;
							newParserContext.Add(edgeType, currentNode);
							break;
						}
					}
				}
			}

			if (!found)
			{
				newParserContext.Add(edgeType, baseTaleContextNodes);
			}
		}

		private string MergeTales(IEnumerable<FunctionNode> resolvedFunctions)
		{
			var tales = resolvedFunctions.Select(function => function.Tale).Distinct();

			// 1. Проверим, имеют ли все сказки одинаковый сценарий.
			TaleNode baseTale = (TaleNode)resolvedFunctions.First().BaseNode;
			bool allTalesHaveSameScenario = resolvedFunctions.All(function => function.BaseNode == baseTale);

			if (allTalesHaveSameScenario)
			{
				TaleNode scenarioTale = (TaleNode)tales.First().BaseNode;
				var scenario = scenarioTale.Scenario;
				
				// Выберем из всех сказок ту, которая содержит наибольшое число функций.
				TaleNode mostRelevantTale = tales.MaxBy(tale => tale.Functions.Count());

				// TODO: Нужно ли делать что-то еще?
			}
			else
			{

			}

			throw new NotImplementedException();
		}

		private string GenerateText(IEnumerable<FunctionNode> functionNodes)
		{
			StringBuilder textBuilder = new StringBuilder();

			foreach (FunctionNode functionNode in functionNodes)
			{
				textBuilder.AppendFormat("{0}{1}", _templateParser.Parse(functionNode).Text, Environment.NewLine);
			}

			return textBuilder.ToString();
		}

		public string GenerateText(TaleNode taleNode)
		{
			if (taleNode == null)
			{
				throw new ArgumentNullException("taleNode");
			}

			return GenerateText(taleNode.Functions);
		}

		public string GenerateText(TalesNetwork talesNetwork, string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentException("text");
			}

			IList<FunctionNode> resolvedFunctionNodes;
			IDictionary<FunctionNode, TemplateParserResult> parserResults;
			StringBuilder textBuilder = new StringBuilder();

			ResolveText(talesNetwork, text, out resolvedFunctionNodes, out parserResults);

			if (resolvedFunctionNodes.Count == 0)
			{
				return string.Empty;
			}

			TaleNode taleNode = resolvedFunctionNodes.First().Tale;
			bool areMembersOfTheSameTale = resolvedFunctionNodes.All(node => node.Tale == taleNode);

			if (!areMembersOfTheSameTale)
			{
				//throw new NotImplementedException();

				var tales = resolvedFunctionNodes.Select(function => function.Tale);
				int maxFunctionCount = 0;

				foreach (TaleNode tale in tales)
				{
					if (tale.BaseNode is TaleNode &&
						tale.Functions.Count() > maxFunctionCount)
					{
						maxFunctionCount = tale.Functions.Count();
						taleNode = tale;
					}
				}

				//taleNode = resolvedFunctionNodes.Select(function => function.Tale).MaxBy(tale => tale.Functions.Count());
			}

			TaleNode baseTaleNode = (TaleNode)taleNode.BaseNode;

			if (baseTaleNode == null)
			{
				throw new InvalidOperationException();
			}

			var scenario = baseTaleNode.Scenario;
			List<FunctionNode> scenarioFunctions = new List<FunctionNode>();

			foreach (FunctionType functionType in scenario)
			{
				FunctionNode resolvedNode = resolvedFunctionNodes.FirstOrDefault(node => node.FunctionType == functionType);

				if (resolvedNode != null)
				{
					//scenarioFunctions.Add(resolvedNode);
					textBuilder.AppendFormat("{0}{1}", _templateParser.Parse(resolvedNode).Text, Environment.NewLine);
				}
				else
				{
					FunctionNode currentTaleNode = taleNode.Functions.FirstOrDefault(node => node.FunctionType == functionType);

					if (currentTaleNode != null)
					{
						//scenarioFunctions.Add(currentTaleNode);
						textBuilder.AppendFormat("{0}{1}", _templateParser.Parse(currentTaleNode).Text, Environment.NewLine);
					}
					else
					{
						FunctionNode baseFunctionNode = baseTaleNode.Functions.FirstOrDefault(node => node.FunctionType == functionType);

						if (baseFunctionNode == null)
						{
							throw new InvalidOperationException();
						}

						TemplateParserNetworkDictionaryContext newParserContext = new TemplateParserNetworkDictionaryContext();
						GetChildContextNode(parserResults, taleNode, baseFunctionNode, NetworkEdgeType.Agent, newParserContext);
						GetChildContextNode(parserResults, taleNode, baseFunctionNode, NetworkEdgeType.Recipient, newParserContext);
						GetChildContextNode(parserResults, taleNode, baseFunctionNode, NetworkEdgeType.Action, newParserContext);

						textBuilder.AppendFormat("{0}{1}", _templateParser.Parse(baseFunctionNode, newParserContext).Text, Environment.NewLine);
					}
				}
			}

			//return GenerateText(scenarioFunctions);
			return textBuilder.ToString();
		}
		#endregion
	}
}
