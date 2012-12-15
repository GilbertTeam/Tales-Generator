using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gt.Controls.Diagramming
{
	public class EdgeEventArgs : DiagramEventArgs
	{
		#region Ctors

		public EdgeEventArgs(Diagram diagram, DiagramEdge edge)
			: base(diagram)
		{
			Edge = edge;
		}

		#endregion

		#region Props

		public DiagramEdge Edge { get; set; }

		#endregion
	}
}
