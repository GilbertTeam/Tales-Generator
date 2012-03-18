using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TalesGenerator.Core.Collections;
using System.Text.RegularExpressions;

namespace TalesGenerator.Core
{
	public class Reasoner
	{
		#region Fields

		private Network _network;

		private readonly Dictionary<string, NetworkEdgeType> _dictionary = new Dictionary<string, NetworkEdgeType>()
		{
			{ "есть", NetworkEdgeType.IsA },
			{ "агент", NetworkEdgeType.Agent },
			{ "реципиент", NetworkEdgeType.Recipient },
			{ "цель", NetworkEdgeType.Goal },
			{ "локатив", NetworkEdgeType.Locative }
		};
		#endregion

		#region Properties

		public Network Network
		{
			get { return _network; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_network = value;
			}
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

		private string[] PrepareQuestion(string text)
		{
			text = text.ToLower();

			string[] words;
			if (text.Contains('\"'))
			{
				var matches = Regex.Matches(text, @"""((?:[^\\""]|\\.)*)""");

				if (matches.Count != 3)
				{
					throw new ArgumentException(Properties.Resources.InvalidFormatError, "text");
				}

				words = matches.Cast<Match>().Where(m => m.Success).Select(m => m.Value).ToArray();
			}
			else
			{
				words = text.Split(' ');
			}

			if (words.Length != 3)
			{
				throw new ArgumentException(Properties.Resources.InvalidFormatError, "text");
			}

			return words;
		}

		private bool FindUpcastNode(NetworkNode startCaseFrameNode, NetworkNode targetCaseFrameNode)
		{
			bool found = false;
			NetworkNode currentCaseFrameNode = startCaseFrameNode;

			while (!found &&
				currentCaseFrameNode != null)
			{
				if (currentCaseFrameNode == targetCaseFrameNode)
				{
					found = true;
				}

				currentCaseFrameNode = currentCaseFrameNode.BaseNode;
			}

			return found;
		}

		private bool FindDowncastNode(NetworkNode startCaseFrameNode, NetworkNode targetCaseFrameNode, NetworkEdgeType edgeType)
		{
			bool found = false;

			NetworkNode currentCaseFrameNode = startCaseFrameNode;
			if (currentCaseFrameNode != null)
			{
				if (currentCaseFrameNode == targetCaseFrameNode)
				{
					found = true;
				}

				if (found)
				{
					NetworkEdge tempEdge = currentCaseFrameNode.OutgoingEdges.GetEdge(edgeType);

					if (tempEdge != null)
					{
						NetworkNode tempNode = tempEdge.EndNode;
						NetworkNode targetNode = targetCaseFrameNode.OutgoingEdges.GetEdge(edgeType).EndNode;

						found = tempNode.IsInherit(targetNode);
					}
				}
				else
				{
					var isAEdges = currentCaseFrameNode.IncomingEdges.GetEdges(NetworkEdgeType.IsA);
					var enumerator = isAEdges.GetEnumerator();

					while (!found &&
						enumerator.MoveNext())
					{
						found = FindDowncastNode(enumerator.Current.StartNode, targetCaseFrameNode, edgeType);
					}
				}
			}

			return found;
		}

		private bool FindNode(NetworkNode targetNode, NetworkNode targetCaseFrameNode, NetworkEdgeType edgeType)
		{
			NetworkEdge targetEdge = targetNode.IncomingEdges.GetEdge(edgeType);
			NetworkNode startCaseFrameNode = targetEdge.StartNode;
			bool found = false;

			found = FindUpcastNode(startCaseFrameNode, targetCaseFrameNode);

			if (!found)
			{
				found = FindDowncastNode(startCaseFrameNode, targetCaseFrameNode, edgeType);
			}

			return found;
		}

		public bool Confirm(string question)
		{
			if (string.IsNullOrEmpty(question))
			{
				throw new ArgumentNullException("text");
			}

			string[] words = PrepareQuestion(question);

			string agent = words[0].Trim().Replace("\"", "");
			string caseFrame = words[1].Trim().Replace("\"", "");
			string recipient = words[2].Trim().Replace("\"", "");

			NetworkNode agentNode = _network.Nodes.GetNode(agent);
			NetworkNode caseFrameNode = _network.Nodes.GetNode(caseFrame);
			NetworkNode recipientNode = _network.Nodes.GetNode(recipient);

			if (agentNode == null ||
				caseFrameNode == null ||
				recipientNode == null)
			{
				throw new ArgumentException(Properties.Resources.InvalidFormatError, "text");
			}

			NetworkEdge agentEdge = agentNode.IncomingEdges.GetEdge(NetworkEdgeType.Agent);
			NetworkEdge recipientEdge = recipientNode.IncomingEdges.GetEdge(NetworkEdgeType.Recipient);

			if (agentEdge == null ||
				recipientEdge == null)
			{
				throw new ArgumentException(Properties.Resources.InvalidFormatError, "text");
			}

			NetworkNode agentCaseFrameNode = agentEdge.StartNode;
			NetworkNode recipientCaseFrameNode = recipientEdge.StartNode;

			bool agentFound = FindNode(agentNode, caseFrameNode, NetworkEdgeType.Agent);
			bool recipientFound = FindNode(recipientNode, caseFrameNode, NetworkEdgeType.Recipient);

			return agentFound && recipientFound;
		}
		#endregion
	}
}
