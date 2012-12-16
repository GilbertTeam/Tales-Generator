using System;
using System.Linq;
using System.Text;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;
using TalesGenerator.TaleNet;
using System.Collections.Generic;

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
			string[] sentences = text.Split('\n');
			SentenceComparer sentenceComparer = new SentenceComparer();
			parserResults = new Dictionary<FunctionNode, TemplateParserResult>();
			resolvedFunctionNodes = new List<FunctionNode>();

			foreach (string sentence in sentences)
			{
				string trimSentence = sentence.Trim('.', '\r', '\n');

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

						if (sentenceComparer.Compare(parserResult, trimSentence))
						{
							resolvedFunctionNodes.Add(functionNode);
						}

						//if (parserResult.Text != null &&
						//    parserResult.Text.Trim('.') == trimSentence.Trim('.', '\r'))
						//{
						//    resolvedFunctionNodes.Add(functionNode);
						//}
					}
				}
			}
		}

		private void GetChildContextNode(
			IDictionary<FunctionNode, TemplateParserResult> parserResults,
			TaleNode currentTaleNode,
			FunctionNode baseFunctionNode,
			NetworkEdgeType edgeType,
			TemplateParserDictionaryContext newParserContext)
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
				throw new NotImplementedException();
			}

			TaleNode baseTaleNode = (TaleNode)taleNode.BaseNode;

			if (baseTaleNode == null)
			{
				throw new InvalidOperationException();
			}

			var scenario = baseTaleNode.Scenario;
			//Dictionary<FunctionType, FunctionNode> scenarioFunctions = new Dictionary<FunctionType, FunctionNode>();
			List<FunctionNode> scenarioFunctions = new List<FunctionNode>();

			foreach (FunctionType functionType in scenario)
			{
				//TODO Функций конкретного типа в общем случае может быть несколько.
				FunctionNode resolvedNode = resolvedFunctionNodes.SingleOrDefault(node => node.FunctionType == functionType);

				if (resolvedNode != null)
				{
					//scenarioFunctions.Add(resolvedNode);
					textBuilder.AppendFormat("{0}{1}", _templateParser.Parse(resolvedNode).Text, Environment.NewLine);
				}
				else
				{
					//TODO Функций конкретного типа в общем случае может быть несколько.
					FunctionNode currentTaleNode = taleNode.Functions.SingleOrDefault(node => node.FunctionType == functionType);

					if (currentTaleNode != null)
					{
						//scenarioFunctions.Add(currentTaleNode);
						textBuilder.AppendFormat("{0}{1}", _templateParser.Parse(currentTaleNode).Text, Environment.NewLine);
					}
					else
					{
						//TODO Функций конкретного типа в общем случае может быть несколько.
						FunctionNode baseFunctionNode = baseTaleNode.Functions.SingleOrDefault(node => node.FunctionType == functionType);

						if (baseFunctionNode == null)
						{
							throw new InvalidOperationException();
						}

						TemplateParserDictionaryContext newParserContext = new TemplateParserDictionaryContext();
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
