using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Gt.Controls.Diagramming.NodeDrawers
{
	public class RoundedRectangleItemDrawer : BaseNodeDrawer
	{
		#region Method

		protected override Geometry CalculateNodeGeometry(DiagramNode node)
		{
			return new RectangleGeometry(node.Bounds, node.RadiusX, node.RadiusY);
		}

		#endregion
	}
}
