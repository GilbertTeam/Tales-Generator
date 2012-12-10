using TalesGenerator.Net;
using TalesGenerator.Net.Collections;
using TalesGenerator.TaleNet.Collections;

namespace TalesGenerator.TaleNet
{
	public enum FunctionType
	{
		/// <summary>
		/// Начальная ситуация.
		/// </summary>
		InitialState,

		/// <summary>
		/// Запрет.
		/// </summary>
		Prohibition,

		/// <summary>
		/// Отлучка старших.
		/// </summary>
		SeniorsDeparture,

		/// <summary>
		/// Нарушение запрета.
		/// </summary>
		ProhibitionViolation,

		/// <summary>
		/// Вредительство.
		/// </summary>
		Sabotage,

		/// <summary>
		/// Сообщение беды.
		/// </summary>
		WoesPost
	}

	public class FunctionNode : NetworkNode
	{
		#region Fields

		private readonly TaleNode _taleNode;

		private readonly FunctionType _functionType;

		private readonly FunctionNodeActorCollection _agentNodes;

		private readonly FunctionNodeActorCollection _recipientNodes;

		private NetworkNode _templateNode;

		private NetworkNode _actionNode;
		#endregion

		#region Properties

		public TaleNode Tale
		{
			get { return _taleNode; }
		}

		public FunctionNodeActorCollection Agents
		{
			get { return _agentNodes; }
		}

		public FunctionNodeActorCollection Recipients
		{
			get { return _recipientNodes; }
		}

		public FunctionType FunctionType
		{
			get { return _functionType; }
		}

		public string Template
		{
			get
			{
				return _templateNode != null ? _templateNode.Name : null;
			}
			set
			{
				UpdateContextNode(ref _templateNode, NetworkEdgeType.Template, value);

				OnPropertyChanged("Template");
			}
		}

		public string Action
		{
			get
			{
				return _actionNode != null ? _actionNode.Name : null;
			}
			set
			{
				UpdateContextNode(ref _actionNode, NetworkEdgeType.Action, value);

				OnPropertyChanged("Action");
			}
		}
		#endregion

		#region Constructors

		internal FunctionNode(TaleNode taleNode, string name, FunctionType functionType)
			: base(taleNode.Network, name)
		{
			_taleNode = taleNode;
			_functionType = functionType;
			_agentNodes = new FunctionNodeActorCollection(this, NetworkEdgeType.Agent);
			_recipientNodes = new FunctionNodeActorCollection(this, NetworkEdgeType.Recipient);
		}

		public FunctionNode(TaleNode taleNode, string name, FunctionNode baseNode)
			: base(taleNode.Network, name, baseNode)
		{
			_taleNode = taleNode;
			_functionType = baseNode._functionType;
			_agentNodes = new FunctionNodeActorCollection(this, NetworkEdgeType.Agent);
			_recipientNodes = new FunctionNodeActorCollection(this, NetworkEdgeType.Recipient);
			_templateNode = baseNode._templateNode;
			_actionNode = baseNode._actionNode;
		}
		#endregion

		#region Methods

		private void UpdateContextNode(ref NetworkNode node, NetworkEdgeType edgeType, string value)
		{
			if (value == null)
			{
				NetworkEdge edge = OutgoingEdges.GetEdge(edgeType);

				if (edge != null)
				{
					Network.Edges.Remove(edge);
				}

				Network.Nodes.Remove(node);
				node = null;
			}
			else
			{
				if (node == null)
				{
					node = Network.Nodes.Add(value);
					Network.Edges.Add(this, node, edgeType);
				}
				else
				{
					node.Name = value;
				}
			}
		}
		#endregion
	}
}
