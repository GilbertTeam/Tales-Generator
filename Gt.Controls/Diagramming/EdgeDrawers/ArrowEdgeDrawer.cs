using System;
using System.Windows;
using System.Windows.Media;

using System.Collections.Generic;

namespace Gt.Controls.Diagramming.EdgeDrawers
{
	public class ArrowEdgeDrawer : BaseEdgeDrawer
	{
		protected const double _defaultArrowLength = 10;

		protected const double _defaultArrowWingLength = 3;

		#region Methods

		protected PathFigure CalculateArrowShape(Point startPoint, Point endPoint, out Point arrowStartPoint)
		{
			PathFigure result = new PathFigure();
			result.IsClosed = true;
			result.IsFilled = true;

			arrowStartPoint = new Point();
			Point arrowUpPoint;
			Point arrowDownPoint;

			if (startPoint.X == endPoint.X)
			{
				bool isBackward = endPoint.Y < startPoint.Y;
				arrowStartPoint.X = endPoint.X;
				arrowStartPoint.Y = isBackward ? Math.Abs(endPoint.Y + _defaultArrowLength) : Math.Abs(endPoint.Y - _defaultArrowLength);
				arrowUpPoint = new Point(arrowStartPoint.X + _defaultArrowWingLength, arrowStartPoint.Y);
				arrowDownPoint = new Point(arrowStartPoint.X - _defaultArrowWingLength, arrowStartPoint.Y);
			}
			else if (startPoint.Y == endPoint.Y)
			{
				bool isBackward = endPoint.X < startPoint.X;
				arrowStartPoint.X = isBackward ? Math.Abs(endPoint.X + _defaultArrowLength) : Math.Abs(endPoint.X - _defaultArrowLength);
				arrowStartPoint.Y = endPoint.Y;
				arrowUpPoint = new Point(arrowStartPoint.X, arrowStartPoint.Y + _defaultArrowWingLength);
				arrowDownPoint = new Point(arrowStartPoint.X, arrowStartPoint.Y - _defaultArrowWingLength);
			}
			else
			{
				double a = Math.Abs(startPoint.Y - endPoint.Y); // катет напротив угла
				double length = Math.Sqrt(Math.Pow(startPoint.X - endPoint.X, 2) + Math.Pow(startPoint.Y - endPoint.Y, 2));

				double a1 = a / length * _defaultArrowLength;

				//координаты искомой точки
				arrowStartPoint.X = startPoint.X > endPoint.X ? endPoint.X + Math.Sqrt(_defaultArrowLength * _defaultArrowLength - a1 * a1) : endPoint.X - Math.Sqrt(_defaultArrowLength
					* _defaultArrowLength - a1 * a1);
				arrowStartPoint.Y = startPoint.Y > endPoint.Y ? endPoint.Y + a1 : endPoint.Y - a1;

				double xAdd = _defaultArrowWingLength / Math.Sqrt(2);
				double yAdd = xAdd;

				xAdd = a / length * _defaultArrowWingLength;
				yAdd = Math.Sqrt(_defaultArrowWingLength * _defaultArrowWingLength - xAdd * xAdd);


				arrowDownPoint = new Point(arrowStartPoint.X - xAdd, arrowStartPoint.Y + yAdd);
				arrowUpPoint = new Point(arrowStartPoint.X + xAdd, arrowStartPoint.Y - yAdd);
			}

			result.StartPoint = arrowStartPoint;
			result.Segments.Add(new LineSegment(arrowDownPoint, true));
			result.Segments.Add(new LineSegment(endPoint, true));
			result.Segments.Add(new LineSegment(arrowUpPoint, true));
			result.Segments.Add(new LineSegment(arrowStartPoint, true));

			return result;
		}

		protected override Geometry CalculateEdgeGeometry(DiagramEdge edge)
		{
			PathGeometry result = new PathGeometry();
			PathFigure figure = new PathFigure();
			PathFigure arrowFigure = null;
			Point? start;
			Point? end;
			FindBoundaries(edge, out start, out end);
			if (start != null && end != null)
			{
				figure.StartPoint = start.Value;

				Point arrowStartPoint;
				arrowFigure = CalculateArrowShape(start.Value, end.Value, out arrowStartPoint);

				PathSegment mainSegment;
				if (edge.AnchoringMode == EdgeAnchoringMode.NodeToNode && edge.SourceNode != null && edge.SourceNode == edge.DestinationNode)
				{
					double a = edge.SourceNode.Bounds.Width / 2
						+ GlobalData.SelfArcWingAmplification * edge.SourceNode.Bounds.Width / 2;
					double b = edge.SourceNode.Bounds.Height / 2;
					mainSegment = new ArcSegment(arrowStartPoint, new Size(a, b), 0, true, SweepDirection.Clockwise, true);
				}
				else
				{
					mainSegment = new LineSegment(arrowStartPoint, true);
				}
				figure.Segments.Add(mainSegment);
			}
			result.Figures.Add(figure);
			if (arrowFigure != null)
			{
				result.Figures.Add(arrowFigure);
			}

			return result;
		}

		protected override DiagramSelectionBorder CalculateEdgeBorder(DiagramEdge edge)
		{
			var geometry = edge.Geometry as PathGeometry;
			if (geometry == null)
				return null;

			if (geometry.Figures.Count != 2)
				return null;

			var border = new DiagramSelectionBorder(false);

			var mainFigure = geometry.Figures[0];
			var arrowFigure = geometry.Figures[1];
			border.ResizeInfos.Add(new ResizeInfo(mainFigure.StartPoint, ResizeDirection.AllRoundSource));
			border.ResizeInfos.Add(new ResizeInfo((arrowFigure.Segments[1] as LineSegment).Point, ResizeDirection.AllRoundDestination));

			return border;
		}

		#endregion
	}
}
