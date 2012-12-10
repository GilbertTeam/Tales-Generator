using System;
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

		public FunctionNode Add(string name, FunctionType functionType)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}

			FunctionNode functionNode = new FunctionNode(_taleNode, name, functionType);

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

			FunctionNode functionNode = new FunctionNode(_taleNode, name, baseNode);

			Add(functionNode);

			return functionNode;
		}
		#endregion
	}
}
