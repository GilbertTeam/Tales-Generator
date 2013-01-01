using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TalesGenerator.Net.Collections;
using TalesGenerator.Net;
using System.Diagnostics.Contracts;

namespace TalesGenerator.TaleNet.Collections
{
	public class TaleFunctionNodeCollection : BaseTaleNodeCollection<FunctionNode>, IEnumerable, IEnumerable<FunctionNode>
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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<FunctionNode>)this).GetEnumerator();
		}

		IEnumerator<FunctionNode> IEnumerable<FunctionNode>.GetEnumerator()
		{
			var f = Network.Nodes.OfType<FunctionNode>().ToList();
			var functionNodes = Network.Nodes.OfType<FunctionNode>().Where(
				node =>
				{
					var partOfEdges = node.OutgoingEdges.GetEdges(NetworkEdgeType.PartOf);

					return partOfEdges.Any(edge => edge.EndNode == _taleNode);
				}
			);

			return functionNodes.GetEnumerator();
		}

		public override void Add(FunctionNode functionNode)
		{
			Contract.Requires<ArgumentNullException>(functionNode != null);

			

			base.Add(functionNode);
		}

		public FunctionNode Add(string name, FunctionType functionType)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}

			// TODO: Необходимо определиться, т.к. функций одного типа в общем случае может быть несколько.
			if (this.Count(node => node.FunctionType == functionType) != 0)
			{
				throw new InvalidOperationException();
			}

			FunctionNode functionNode = new FunctionNode(_taleNode, name, functionType);

			Network.Edges.Add(functionNode, ((TalesNetwork)_taleNode.Network).BaseFunction, Net.NetworkEdgeType.IsA);

			if (Count > 0)
			{
				Network.Edges.Add(this.Last(), functionNode, Net.NetworkEdgeType.Follow);
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

		public override bool Remove(FunctionNode item)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
