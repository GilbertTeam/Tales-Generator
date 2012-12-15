using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Gt.Controls.Diagramming.NodeDrawers
{
	public class EllipseNodeDrawer : BaseNodeDrawer
	{
		#region Methods

		protected override System.Windows.Media.Geometry CalculateNodeGeometry(DiagramNode node)
		{
			return new EllipseGeometry(node.Bounds);
		}

		#endregion
	}
}
