using TalesGenerator.Net;
using TalesGenerator.Net.Collections;
using TalesGenerator.TaleNet.Collections;
using System.Diagnostics.Contracts;
using System;

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
		Interdiction,

		/// <summary>
		/// Отлучка старших.
		/// </summary>
		Absentation,

		/// <summary>
		/// Нарушение запрета.
		/// </summary>
		ViolationOfInterdiction,

		/// <summary>
		/// Вредительство.
		/// </summary>
		Villainy,

		/// <summary>
		/// Сообщение беды.
		/// </summary>
		Mediation,

		/// <summary>
		/// Отправка на поиски.
		/// </summary>
		Departure,

		/// <summary>
		/// Встреча с испытателем.
		/// </summary>
		DonorMeeting,

		/// <summary>
		/// Испытание
		/// </summary>
		Test,

		/// <summary>
		/// Попытка пройти испытание.
		/// </summary>
		TestAttempt,

		/// <summary>
		/// Результат испытания.
		/// </summary>
		TestResult
	}

	public class FunctionNode : NetworkNode
	{
		#region Fields

		private readonly TaleNode _taleNode;

		private readonly FunctionType _functionType;

		private readonly FunctionNodeActorCollection _agentNodes;

		private readonly FunctionNodeActorCollection _recipientNodes;

		//private NetworkNode _templateNode;

		//private NetworkNode _actionNode;
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
				NetworkEdge templateEdge = OutgoingEdges.GetEdge(NetworkEdgeType.Template, true);

				return templateEdge != null ? templateEdge.EndNode.Name : null;
			}
			set
			{
				UpdateContextNode(NetworkEdgeType.Template, value);

				OnPropertyChanged("Template");
			}
		}

		//public string Action
		//{
		//    get
		//    {
		//        NetworkEdge actionEdge = OutgoingEdges.GetEdge(NetworkEdgeType.Action, true);

		//        return actionEdge != null ? actionEdge.EndNode.Name : null;
		//    }
		//    set
		//    {
		//        UpdateContextNode(NetworkEdgeType.Action, value);

		//        OnPropertyChanged("Action");
		//    }
		//}

		public TaleItemNode Action
		{
			get
			{
				NetworkEdge actionEdge = OutgoingEdges.GetEdge(NetworkEdgeType.Action, true);

				return actionEdge != null ? (TaleItemNode)actionEdge.EndNode : null;
			}

			set
			{
				Contract.Requires<ArgumentNullException>(value != null);
				Contract.Assume(OutgoingEdges.GetEdge(NetworkEdgeType.Action, false) == null);

				Network.Edges.Add(this, value, NetworkEdgeType.Action);
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
		}
		#endregion

		#region Methods

		private void UpdateContextNode(NetworkEdgeType edgeType, string value)
		{
			NetworkEdge networkEdge = OutgoingEdges.GetEdge(edgeType);

			if (networkEdge == null)
			{
				if (value != null)
				{
					NetworkNode networkNode = Network.Nodes.Add(value);
					Network.Edges.Add(this, networkNode, edgeType);

					NetworkNode baseNode = null;

					switch (edgeType)
					{
						case NetworkEdgeType.Action:
							baseNode = ((TalesNetwork)Network).BaseAction;
							break;

						case NetworkEdgeType.Template:
							baseNode = ((TalesNetwork)Network).BaseTemplate;
							break;
					}

					Network.Edges.Add(networkNode, baseNode, NetworkEdgeType.IsA);
				}
			}
			else
			{
				if (value == null)
				{
					Network.Nodes.Remove(networkEdge.EndNode);
					Network.Edges.Remove(networkEdge);
				}
				else
				{
					networkEdge.EndNode.Name = value;
				}
			}
		}
		#endregion
	}
}
