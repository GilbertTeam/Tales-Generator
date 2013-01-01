using TalesGenerator.Net;
using TalesGenerator.Net.Collections;
using TalesGenerator.TaleNet.Collections;
using System.Diagnostics.Contracts;
using System;
using System.Xml.Linq;
using TalesGenerator.Net.Serialization;

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

	public class FunctionNode : TaleBaseItemNode
	{
		#region Fields

		private readonly FunctionNodeContextNodeCollection _actionNodes;

		private readonly FunctionNodeContextNodeCollection _agentNodes;

		private readonly FunctionNodeContextNodeCollection _recipientNodes;

		private FunctionType _functionType;
		#endregion

		#region Properties

		internal override TaleNodeKind NodeKind
		{
			get { return TaleNodeKind.Function; }
		}

		public TaleNode Tale
		{
			get
			{
				Contract.Ensures(Contract.Result<TaleNode>() != null);

				NetworkEdge partOfEdge = OutgoingEdges.GetEdge(NetworkEdgeType.PartOf);
				TaleNode taleNode = partOfEdge.EndNode as TaleNode;

				return taleNode;
			}
		}

		public FunctionNodeContextNodeCollection Actions
		{
			get { return _actionNodes; }
		}

		public FunctionNodeContextNodeCollection Agents
		{
			get { return _agentNodes; }
		}

		public FunctionNodeContextNodeCollection Recipients
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
		#endregion

		#region Constructors

		internal FunctionNode(TalesNetwork talesNetwork)
			: base(talesNetwork)
		{
			_actionNodes = new FunctionNodeContextNodeCollection(this, NetworkEdgeType.Action);
			_agentNodes = new FunctionNodeContextNodeCollection(this, NetworkEdgeType.Agent);
			_recipientNodes = new FunctionNodeContextNodeCollection(this, NetworkEdgeType.Recipient);
		}

		internal FunctionNode(TaleNode taleNode, string name, FunctionType functionType)
			: base((TalesNetwork)taleNode.Network, name)
		{
			Network.Edges.Add(this, taleNode, Net.NetworkEdgeType.PartOf);

			_functionType = functionType;
			_actionNodes = new FunctionNodeContextNodeCollection(this, NetworkEdgeType.Action);
			_agentNodes = new FunctionNodeContextNodeCollection(this, NetworkEdgeType.Agent);
			_recipientNodes = new FunctionNodeContextNodeCollection(this, NetworkEdgeType.Recipient);
		}

		internal FunctionNode(TaleNode taleNode, string name, FunctionNode baseNode)
			: base((TalesNetwork)taleNode.Network, name, baseNode)
		{
			Network.Edges.Add(this, taleNode, Net.NetworkEdgeType.PartOf);

			_functionType = baseNode._functionType;
			_actionNodes = new FunctionNodeContextNodeCollection(this, NetworkEdgeType.Action);
			_agentNodes = new FunctionNodeContextNodeCollection(this, NetworkEdgeType.Agent);
			_recipientNodes = new FunctionNodeContextNodeCollection(this, NetworkEdgeType.Recipient);
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
					TaleItemNode node = new TaleItemNode((TalesNetwork)Network, value);

					Network.Nodes.Add(node);
					Network.Edges.Add(this, node, edgeType);

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

					Network.Edges.Add(node, baseNode, NetworkEdgeType.IsA);
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

		public override XElement GetXml()
		{
			XElement xElement = base.GetXml();

			xElement.Add(
				new XAttribute("functionType", _functionType));

			return xElement;
		}

		public override void LoadFromXml(XElement xElement)
		{
			Contract.Requires<ArgumentNullException>(xElement != null);

			base.LoadFromXml(xElement);

			_functionType = (FunctionType)Enum.Parse(typeof(FunctionType), xElement.Attribute("functionType").Value);
		}
		#endregion
	}
}
