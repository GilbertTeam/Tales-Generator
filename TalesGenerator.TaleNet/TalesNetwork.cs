using TalesGenerator.Net;
using TalesGenerator.TaleNet.Collections;
using System.Xml.Linq;
using System.Diagnostics.Contracts;
using System;
using System.Linq;
using TalesGenerator.Net.Serialization;

namespace TalesGenerator.TaleNet
{
	public class TalesNetwork : Network
	{
		#region Fields

		private TaleItemNode _baseActionNode;

		private TaleItemNode _baseLocativeNode;

		private TaleItemNode _basePersonNode;

		private NetworkNode _baseFunctionNode;

		private NetworkNode _baseTaleNode;

		private NetworkNode _baseTemplateNode;

		private readonly TaleItemNodeCollection _actionNodes;

		private readonly TaleItemNodeCollection _locativeNodes;

		private readonly TaleItemNodeCollection _personNodes;

		private readonly TaleNodeCollection _taleNodes;
		#endregion

		#region Properties

		internal TaleItemNode BaseAction
		{
			get { return _baseActionNode; }
		}

		internal TaleItemNode BaseLocative
		{
			get { return _baseLocativeNode; }
		}

		internal TaleItemNode BasePerson
		{
			get { return _basePersonNode; }
		}

		internal NetworkNode BaseFunction
		{
			get { return _baseFunctionNode; }
		}

		internal NetworkNode BaseTale
		{
			get { return _baseTaleNode; }
		}

		internal NetworkNode BaseTemplate
		{
			get { return _baseTemplateNode; }
		}

		public TaleItemNodeCollection Actions
		{
			get { return _actionNodes; }
		}

		public TaleItemNodeCollection Locatives
		{
			get { return _locativeNodes; }
		}

		public TaleItemNodeCollection Persons
		{
			get { return _personNodes; }
		}

		public TaleNodeCollection Tales
		{
			get { return _taleNodes; }
		}
		#endregion

		#region Constructors

		public TalesNetwork()
		{
			_baseActionNode = new TaleItemNode(this, Properties.Resources.BaseActionNodeName);
			Nodes.Add(_baseActionNode);

			_baseLocativeNode = new TaleItemNode(this, Properties.Resources.BaseLocativeNodeName);
			Nodes.Add(_baseLocativeNode);

			_basePersonNode = new TaleItemNode(this, Properties.Resources.BasePersonNodeName);
			Nodes.Add(_basePersonNode);

			_baseFunctionNode = new TaleItemNode(this, Properties.Resources.BaseFunctionNodeName);
			Nodes.Add(_baseFunctionNode);

			_baseTaleNode = new TaleItemNode(this, Properties.Resources.BaseTaleNodeName);
			Nodes.Add(_baseTaleNode);

			_baseTemplateNode = new TaleItemNode(this, Properties.Resources.BaseTemplateNodeName);
			Nodes.Add(_baseTemplateNode);

			_actionNodes = new TaleItemNodeCollection(this, _baseActionNode);
			_locativeNodes = new TaleItemNodeCollection(this, _baseLocativeNode);
			_personNodes = new TaleItemNodeCollection(this, _basePersonNode);
			_taleNodes = new TaleNodeCollection(this);
		}
		#endregion

		#region Methods

		protected override void LoadFromXElement(XElement xNetwork)
		{
			Contract.Requires<ArgumentNullException>(xNetwork != null);

			XNamespace xNamespace = SerializableObject.XNamespace;

			XElement xNodesBase = xNetwork.Element(xNamespace + "Nodes");
			var xNodes = xNodesBase.Elements(xNamespace + "Node");
			foreach (XElement xNode in xNodes)
			{
				XAttribute xNodeKindAttribute = xNode.Attribute("nodeKind");
				NetworkNode networkNode = null;

				if (xNodeKindAttribute == null)
				{
					//networkNode = Nodes.Add();
					throw new SerializationException();
				}
				else
				{
					TaleNodeKind nodeKind = (TaleNodeKind)Enum.Parse(typeof(TaleNodeKind), xNodeKindAttribute.Value);

					switch (nodeKind)
					{
						case TaleNodeKind.Tale:
							networkNode = new TaleNode(this);
							break;

						case TaleNodeKind.TaleItem:
							networkNode = new TaleItemNode(this);
							break;

						case TaleNodeKind.Function:
							networkNode = new FunctionNode(this);
							break;
					}
				}

				networkNode.LoadFromXml(xNode);

				// TODO: Необходимо избавиться от этого костыля.
				if (!Nodes.Where(node => node.Id == networkNode.Id).Any())
				{
					Nodes.Add(networkNode);
				}
			}

			XElement xEdgesBase = xNetwork.Element(xNamespace + "Edges");
			var xEdges = xEdgesBase.Elements(xNamespace + "Edge");
			foreach (XElement xEdge in xEdges)
			{
				NetworkEdge networkEdge = new TaleItemEdge(this);

				networkEdge.LoadFromXml(xEdge);

				// TODO: Необходимо избавиться от этого костыля.
				if (!Edges.Where(edge => edge.Id == networkEdge.Id).Any())
				{
					Edges.Add(networkEdge);
				}
			}

			SetId();

			_isDirty = false;

			_baseActionNode = (TaleItemNode)Nodes[0];
			_baseLocativeNode = (TaleItemNode)Nodes[1];
			_basePersonNode = (TaleItemNode)Nodes[2];
			_baseFunctionNode = Nodes[3];
			_baseTaleNode = Nodes[4];
			_baseTemplateNode = Nodes[5];
		}
		#endregion
	}
}
