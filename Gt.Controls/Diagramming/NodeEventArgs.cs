using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gt.Controls.Diagramming
{
	public class NodeEventArgs : DiagramEventArgs
	{
		#region Ctors

		public NodeEventArgs(Diagram diagram, DiagramNode node)
			: base(diagram)
		{
			Node = node;
		}

		#endregion

		#region Props

		public DiagramNode Node { get; set; }

		#endregion
	}
}
