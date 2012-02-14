using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TalesGeneratorCore
{
	class SemanticNetworkImpl : ISemanticNetwork
	{
		bool _dirty;

		string _name;

		NodesImpl _nodes;

		EdgesImpl _edges;

		public bool Dirty
		{
			get { return _dirty; }
			set { _dirty = false; }
		}

		public INodes Nodes
		{
			get { return _nodes; }
		}

		public IEdges Edges
		{
			get { return _edges; }
		}

		public void Undo()
		{
		}

		public void Redo()
		{
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public void Load(string path)
		{
		}

		public void Load(Stream reader)
		{
		}

		public void Save(string path)
		{
		}

		public void Save(Stream writer)
		{
		}
		
	}

	class NodesImpl : INodes
	{
	}

	class NodeImpl : INode
	{
	}

	class EdgesImpl : IEdges
	{
	}

	class EdgeImpl : IEdge
	{
	}

}
