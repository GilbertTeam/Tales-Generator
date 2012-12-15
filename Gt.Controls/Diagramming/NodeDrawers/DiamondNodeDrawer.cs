using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Gt.Controls.Diagramming.NodeDrawers
{
	public class DiamondNodeDrawer : BaseNodeDrawer
	{
		#region Constructors
		#endregion

		#region Methods

		protected override System.Windows.Media.Geometry CalculateNodeGeometry(DiagramNode node)
		{
			Rect rect = node.Bounds;
			List<Point> points  = new List<Point>();
			points.Add(new Point((rect.Left + rect.Right) / 2, rect.Top));
			points.Add(new Point(rect.Right, (rect.Top + rect.Bottom) / 2));
			points.Add(new Point((rect.Left + rect.Right) / 2, rect.Bottom));
			points.Add(new Point(rect.Left, (rect.Top + rect.Bottom) / 2));

			PathGeometry pathGeometry = new PathGeometry();

			PathFigure figure = new PathFigure();
			figure.IsClosed = true;
			figure.IsFilled = true;
			figure.StartPoint = points[0];
			for (int i = 1; i < 4; i++)
			{
				LineSegment lineSegment = new LineSegment(points[i], true);
				figure.Segments.Add(lineSegment);
			}
			pathGeometry.Figures.Add(figure);
			
			return pathGeometry;
		}

		#endregion
	}
}
