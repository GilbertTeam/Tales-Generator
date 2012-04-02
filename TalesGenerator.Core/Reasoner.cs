using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TalesGenerator.Core.Collections;
using System.Text.RegularExpressions;
using TalesGenerator.TextAnalyzer;

namespace TalesGenerator.Core
{
	public class Reasoner
	{
		#region Fields

		private const string WhoWord = "кто";

		private const string WhereWord = "где";

		private const string GoalWord = "цель";

		private const string IsWord = "это";

		private const string AgentWord = "агент";

		private const string RecipientWord = "реципиент";

		private readonly Network _network;
		#endregion

		#region Properties

		public Network Network
		{
			get { return _network; }
		}
		#endregion

		#region Constructors

		public Reasoner(Network network)
		{
			if (network == null)
			{
				throw new ArgumentNullException("network");
			}

			_network = network;
		}
		#endregion

		#region Methods

		private bool IsInherit(NetworkNode firstNode, NetworkNode secondNode)
		{
			return firstNode.IsInherit(secondNode, false) || secondNode.IsInherit(firstNode, false);
		}

		private NetworkNode GetNode(NetworkNode caseFrameNode, NetworkEdgeType edgeType)
		{
			NetworkNode networkNode = null;
			NetworkEdge networkEdge = caseFrameNode.OutgoingEdges.GetEdge(edgeType);

			if (networkEdge != null)
			{
				networkNode = networkEdge.EndNode;
			}

			return networkNode;
		}

		private bool FindEdge(NetworkNode caseFrameNode, NetworkEdgeType edgeType, NetworkNode targetNode, NetworkNode firstNode)
		{
			bool found = false;
			NetworkNode endNode = GetNode(caseFrameNode, edgeType);

			if (endNode != null)
			{
				found =
					IsInherit(targetNode, endNode) &&
					(firstNode != null && targetNode.IsInherit(firstNode, true));
			}

			return found;
		}

		private bool FindUpcastNode(NetworkNode startCaseFrameNode, NetworkEdgeType edgeType, NetworkNode targetNode)
		{
			bool found = false;
			NetworkNode currentCaseFrameNode = startCaseFrameNode;
			NetworkNode currentNode = GetNode(currentCaseFrameNode, edgeType);

			if (currentNode != null &&
				IsInherit(targetNode, currentNode))
			{
				found = true;
			}

			while (!found &&
				currentCaseFrameNode != null)
			{
				found = FindEdge(currentCaseFrameNode, edgeType, targetNode, currentNode);
				currentCaseFrameNode = currentCaseFrameNode.BaseNode;
			}

			return found;
		}

		private bool FindDowncastNode(NetworkNode startCaseFrameNode, NetworkEdgeType edgeType, NetworkNode targetNode)
		{
			bool found = false;
			NetworkNode currentCaseFrameNode = startCaseFrameNode;
			NetworkNode currentNode = GetNode(currentCaseFrameNode, edgeType);

			if (currentNode != null &&
				IsInherit(targetNode, currentNode))
			{
				found = true;
			}

			if (!found &&
				currentCaseFrameNode != null)
			{
				found = FindEdge(currentCaseFrameNode, edgeType, targetNode, currentNode);

				if (!found)
				{
					var isAEdges = currentCaseFrameNode.IncomingEdges.GetEdges(NetworkEdgeType.IsA);
					var enumerator = isAEdges.GetEnumerator();

					while (!found &&
						enumerator.MoveNext())
					{
						found = FindDowncastNode(enumerator.Current.StartNode, edgeType, targetNode);
					}
				}
			}

			return found;
		}

		private bool FindNode(NetworkNode startCaseFrameNode, NetworkEdgeType edgeType, NetworkNode targetNode)
		{
			bool found = FindUpcastNode(startCaseFrameNode, edgeType, targetNode);

			if (!found)
			{
				found = FindDowncastNode(startCaseFrameNode, edgeType, targetNode);
			}

			return found;
		}

		private string ProcessQueryWithKeyWordInTheBeginning(List<WordInfo> wordsInfo, NetworkEdgeType edgeType, string errorMessage)
		{
			if (wordsInfo.Count <= 1)
			{
				throw new ReasonerException(errorMessage);
			}

			string result = Properties.Resources.ReasonerDontKnowAnswer;
			string caseFrameNodeName = string.Empty;

			foreach (WordInfo wordInfo in wordsInfo.Skip(1))
			{
				caseFrameNodeName += wordInfo.Word + " ";
			}

			NetworkNode caseFrameNode = _network.Nodes.GetNode(caseFrameNodeName);

			if (caseFrameNode != null)
			{
				NetworkEdge locativeEdge = null;
				NetworkNode currentCaseFrameNode = caseFrameNode;

				while (locativeEdge == null && 
					currentCaseFrameNode != null)
				{
					locativeEdge = currentCaseFrameNode.OutgoingEdges.GetEdge(edgeType);
					currentCaseFrameNode = currentCaseFrameNode.BaseNode;
				}

				if (locativeEdge != null)
				{
					result = locativeEdge.EndNode.Name;
				}
			}

			return result;
		}

		private void ProcessQueryWithKeywordInTheMiddle(List<WordInfo> wordsInfo, string keyWord, string errorMessage, out NetworkNode firstNode, out NetworkNode secondNode)
		{
			string firstNodeName = string.Empty;
			string secondNodeName = string.Empty;
			var keyWords = wordsInfo.Where(wordInfo => wordInfo.Word == keyWord).ToList();

			if (keyWords.Count != 1)
			{
				throw new ReasonerException(errorMessage);
			}

			int isWordIndex = wordsInfo.IndexOf(keyWords.First());

			for (int i = 0; i < isWordIndex; i++)
			{
				firstNodeName += wordsInfo[i].Word + " ";
			}
			for (int i = isWordIndex + 1; i < wordsInfo.Count; i++)
			{
				secondNodeName += wordsInfo[i].Word + " ";
			}

			firstNode = _network.Nodes.GetNode(firstNodeName);
			secondNode = _network.Nodes.GetNode(secondNodeName);
		}

		private string ProcessQueryWithKeywordInTheMiddle(List<WordInfo> wordsInfo, string keyWord, string errorMessage, NetworkEdgeType edgeType)
		{
			string result = Properties.Resources.ReasonerDontKnowAnswer;
			NetworkNode firstNode;
			NetworkNode secondNode;

			ProcessQueryWithKeywordInTheMiddle(wordsInfo, keyWord, errorMessage, out firstNode, out secondNode);

			if (firstNode != null &&
				secondNode != null)
			{
				result =
					FindNode(secondNode, edgeType, firstNode)
					? Properties.Resources.ReasonerYesAnswer
					: Properties.Resources.ReasonerNoAnswer;
			}

			return result;
		}

		private string ProcessWhoQuery(List<WordInfo> wordsInfo)
		{
			string result = Properties.Resources.ReasonerDontKnowAnswer;
			string nodeName = string.Empty;

			foreach (WordInfo wordInfo in wordsInfo.Skip(1))
			{
				nodeName += wordInfo.Word + " ";
			}

			NetworkNode personNode = _network.Nodes.GetNode(nodeName);

			if (personNode != null)
			{
				if (personNode.BaseNode != null)
				{
					result = personNode.BaseNode.Name;
				}
				else if (personNode.InstanceNode != null)
				{
					result = personNode.InstanceNode.Name;
				}
				else
				{
					result = personNode.Name;
				}
			}

			return result;
		}

		private string ProcessIsQuery(List<WordInfo> wordsInfo)
		{
			string result = Properties.Resources.ReasonerDontKnowAnswer;
			NetworkNode firstNode;
			NetworkNode secondNode;

			ProcessQueryWithKeywordInTheMiddle(wordsInfo, IsWord, Properties.Resources.ReasonerIsQueryError, out firstNode, out secondNode);

			if (firstNode != null &&
				secondNode != null)
			{
				result =
					(firstNode.IsInherit(secondNode, true))
					? Properties.Resources.ReasonerYesAnswer
					: Properties.Resources.ReasonerNoAnswer;
			}

			return result;
		}

		private string ProcessWhereQuery(List<WordInfo> wordsInfo)
		{
			return ProcessQueryWithKeyWordInTheBeginning(wordsInfo, NetworkEdgeType.Locative, Properties.Resources.ReasonerWhereQueryError);
		}

		private string ProcessFirstGoalQuery(List<WordInfo> wordsInfo)
		{
			return ProcessQueryWithKeyWordInTheBeginning(wordsInfo, NetworkEdgeType.Goal, Properties.Resources.ReasonerGoalQueryError);
		}

		private string ProcessSecondGoalQuery(List<WordInfo> wordsInfo)
		{
			return ProcessQueryWithKeywordInTheMiddle(wordsInfo, GoalWord, Properties.Resources.ReasonerGoalQueryError, NetworkEdgeType.Goal);
		}

		private string ProcessFirstAgentQuery(List<WordInfo> wordsInfo)
		{
			return ProcessQueryWithKeyWordInTheBeginning(wordsInfo, NetworkEdgeType.Agent, Properties.Resources.ReasonerAgentQueryError);
		}

		private string ProcessSecondAgentQuery(List<WordInfo> wordsInfo)
		{
			return ProcessQueryWithKeywordInTheMiddle(wordsInfo, AgentWord, Properties.Resources.ReasonerAgentQueryError, NetworkEdgeType.Agent);
		}

		private string ProcessFirstRecipientQuery(List<WordInfo> wordsInfo)
		{
			return ProcessQueryWithKeyWordInTheBeginning(wordsInfo, NetworkEdgeType.Recipient, Properties.Resources.ReasonerRecipientQueryError);
		}

		private string ProcessSecondRecipientQuery(List<WordInfo> wordsInfo)
		{
			return ProcessQueryWithKeywordInTheMiddle(wordsInfo, RecipientWord, Properties.Resources.ReasonerRecipientQueryError, NetworkEdgeType.Recipient);
		}

		public string Confirm(string question)
		{
			if (string.IsNullOrEmpty(question))
			{
				throw new ReasonerException(Properties.Resources.ReasonerEmptyQueryError);
			}

			string result = Properties.Resources.ReasonerDontKnowAnswer;
			List<WordInfo> wordsInfo = null;

			try
			{
				wordsInfo = Mystem.Analyze(question).ToList();
			}
			catch { }

			if (wordsInfo != null
				&& wordsInfo.Count > 0)
			{
				WordInfo firstWord = wordsInfo.First();

				if (firstWord.Word == WhoWord)
				{
					result = ProcessWhoQuery(wordsInfo);
				}
				else if (firstWord.Word == WhereWord)
				{
					result = ProcessWhereQuery(wordsInfo);
				}
				else if (wordsInfo.Any(wordInfo => wordInfo.Word == IsWord))
				{
					result = ProcessIsQuery(wordsInfo);
				}
				else if (wordsInfo.Any(wordInfo => wordInfo.Word == GoalWord))
				{
					if (firstWord.NormalForm == GoalWord)
					{
						result = ProcessFirstGoalQuery(wordsInfo);
					}
					else
					{
						result = ProcessSecondGoalQuery(wordsInfo);
					}
				}
				else if (wordsInfo.Any(wordInfo => wordInfo.Word == AgentWord))
				{
					if (firstWord.NormalForm == AgentWord)
					{
						result = ProcessFirstAgentQuery(wordsInfo);
					}
					else
					{
						result = ProcessSecondAgentQuery(wordsInfo);
					}
				}
				else if (wordsInfo.Any(wordInfo => wordInfo.Word == RecipientWord))
				{
					if (firstWord.NormalForm == RecipientWord)
					{
						result = ProcessFirstRecipientQuery(wordsInfo);
					}
					else
					{
						result = ProcessSecondRecipientQuery(wordsInfo);
					}
				}
			}

			return result;
		}
		#endregion
	}
}
