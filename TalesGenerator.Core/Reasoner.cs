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

		private readonly Dictionary<NetworkEdgeType, string> _dictionary = new Dictionary<NetworkEdgeType, string>()
		{
			{ NetworkEdgeType.IsA, "есть" },
			{ NetworkEdgeType.Agent, "агент" },
			{ NetworkEdgeType.Recipient, "реципиент" },
			{ NetworkEdgeType.Goal, "цель" },
			{ NetworkEdgeType.Locative, "локатив" }
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

		private NetworkNode FindNode(NetworkNode startNode, string name)
		{
			NetworkNode baseNode = null;

			if (startNode.Name.ToLower() == name)
			{
				baseNode = startNode;
			}
			else
			{
				startNode = startNode.BaseNode;

				while (startNode != null)
				{
					if (startNode.Name.ToLower() == name)
					{
						baseNode = startNode;
						break;
					}
				}
			}

			return baseNode;
		}

		private NetworkNode GetNode(NetworkNode baseNode, NetworkEdgeType edgeType)
		{
			NetworkEdge agentEdge = baseNode.OutgoingEdges.GetEdge(NetworkEdgeType.Agent);

			return agentEdge.EndNode;
		}

		public bool Confirm(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}

			text = text.ToLower();

			string[] words;
			if (text.Contains('\"'))
			{
				words = Regex.Split(text, @"""[а-я]+""");
			}
			else
			{
				words = text.Split(' ');
			}

			if (words.Length != 3)
			{
				throw new ArgumentException("text");
			}

			string agent = words[0].Trim();
			string caseFrame = words[1].Trim();
			string recipient = words[2].Trim();

			NetworkNode agentNode = _network.Nodes.GetNode(agent);
			NetworkNode caseFrameNode = _network.Nodes.GetNode(caseFrame);
			NetworkNode recipientNode = _network.Nodes.GetNode(recipient);

			if (agentNode == null ||
				caseFrameNode == null ||
				recipientNode == null)
			{
				throw new ArgumentException("text");
			}

			bool result = false;

			bool agentFound = false;
			bool recipientFound = false;

			do
			{
				NetworkEdge agentEdge = caseFrameNode.OutgoingEdges.GetEdge(NetworkEdgeType.Agent);
				NetworkEdge recipientEdge = caseFrameNode.OutgoingEdges.GetEdge(NetworkEdgeType.Recipient);

				if (!agentFound &&
					agentEdge != null &&
					agentEdge.EndNode == agentNode)
				{
					agentFound = true;
				}

				if (!recipientFound &&
					recipientEdge != null &&
					recipientEdge.EndNode == recipientNode)
				{
					recipientFound = true;
				}

				if (agentFound &&
					recipientFound)
				{
					result = true;
					break;
				}

				NetworkEdge isAEdge = caseFrameNode.IncomingEdges.GetEdge(NetworkEdgeType.IsA);
				caseFrameNode = isAEdge != null ? isAEdge.StartNode : null;
			} while (caseFrameNode != null);

			return result;
		}
		#endregion
	}
}
