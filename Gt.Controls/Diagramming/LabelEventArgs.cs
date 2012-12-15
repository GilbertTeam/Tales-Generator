using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gt.Controls.Diagramming
{
	public class LabelEventArgs : DiagramEventArgs
	{
		#region Ctors

		public LabelEventArgs(Diagram diagram, DiagramLabel label)
			: base(diagram)
		{
			Label = label;
		}
		#endregion

		#region Props

		public DiagramLabel Label { get; set; }

		#endregion
	}
}
