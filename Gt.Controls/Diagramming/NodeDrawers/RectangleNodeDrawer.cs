using System.Windows;
using System.Windows.Media;

namespace Gt.Controls.Diagramming.NodeDrawers
{
	public class RectangleNodeDrawer : BaseNodeDrawer
	{
		#region Constructors

		#endregion

		#region Methods

		protected override Geometry CalculateNodeGeometry(DiagramNode node)
		{
			return new RectangleGeometry(node.Bounds);
		}

		#endregion
	}
}
