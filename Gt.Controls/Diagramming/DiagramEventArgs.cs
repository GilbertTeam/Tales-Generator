using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gt.Controls.Diagramming
{
	public class DiagramEventArgs : EventArgs
	{
		#region Ctors

		public DiagramEventArgs(Diagram diagram)
		{
			Diagram = diagram;
		}

		#endregion

		#region Props

		public Diagram Diagram { get; set; }

		#endregion
	}
}
