using System;
using System.Linq;
using TalesGenerator.Net.Collections;

namespace TalesGenerator.TaleNet.Collections
{
	public class TaleFunctionNodeCollection : BaseTaleNodeCollection<FunctionNode>
	{
		#region Fields

		private readonly TaleNode _taleNode;
		#endregion

		#region Constructors

		public TaleFunctionNodeCollection(TaleNode taleNode)
			: base((TalesNetwork)taleNode.Network)
		{
			_taleNode = taleNode;
		}
		#endregion

		#region Methods

		public override void Add(FunctionNode functionNode)
		{
			if (functionNode == null)
			{
				throw new ArgumentNullException("functionNode");
			}

			base.Add(functionNode);
		}

		public FunctionNode Add(string name, FunctionType functionType)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}

			if (this.Count(node => node.FunctionType == functionType) != 0)
			{
				throw new InvalidOperationException();
			}

			FunctionNode functionNode = new FunctionNode(_taleNode, name, functionType);

			Network.Edges.Add(functionNode, ((TalesNetwork)_taleNode.Network).BaseFunction, Net.NetworkEdgeType.IsA);

			if (Count == 0)
			{
				Network.Edges.Add(_taleNode, functionNode, Net.NetworkEdgeType.Follow);
			}
			else
			{
				Network.Edges.Add(this[Count - 1], functionNode, Net.NetworkEdgeType.Follow);
			}

			Add(functionNode);

			return functionNode;
		}

		public FunctionNode Add(string name, FunctionNode baseNode)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}
			if (baseNode == null)
			{
				throw new ArgumentNullException("baseNode");
			}

			if (this.Count(node => node.FunctionType == baseNode.FunctionType) != 0)
			{
				throw new InvalidOperationException();
			}

			FunctionNode functionNode = new FunctionNode(_taleNode, name, baseNode);

			Add(functionNode);

			return functionNode;
		}
		#endregion
	}
}
