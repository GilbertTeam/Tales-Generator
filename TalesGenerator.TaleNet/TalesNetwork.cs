using TalesGenerator.Net;
using TalesGenerator.TaleNet.Collections;

namespace TalesGenerator.TaleNet
{
	public class TalesNetwork : Network
	{
		#region Fields

		private readonly TaleItemNode _baseActionNode;

		private readonly TaleItemNode _baseLocativeNode;

		private readonly TaleItemNode _basePersonNode;

		private readonly NetworkNode _baseFunctionNode;

		private readonly NetworkNode _baseTaleNode;

		private readonly NetworkNode _baseTemplateNode;

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

			_baseFunctionNode = Nodes.Add(Properties.Resources.BaseFunctionNodeName);
			_baseTaleNode = Nodes.Add(Properties.Resources.BaseTaleNodeName);
			_baseTemplateNode = Nodes.Add(Properties.Resources.BaseTemplateNodeName);

			_actionNodes = new TaleItemNodeCollection(this, _baseActionNode);
			_locativeNodes = new TaleItemNodeCollection(this, _baseLocativeNode);
			_personNodes = new TaleItemNodeCollection(this, _basePersonNode);
			_taleNodes = new TaleNodeCollection(this);
		}
		#endregion
	}
}
