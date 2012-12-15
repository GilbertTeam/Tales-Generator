using System.Windows;
using System.Windows.Media;

using System.Collections.Generic;

namespace Gt.Controls.Diagramming.EdgeDrawers
{
	public class LineEdgeDrawer : BaseEdgeDrawer
	{
		#region Methods

		protected override Geometry CalculateEdgeGeometry(DiagramEdge edge)
		{
			PathGeometry result = new PathGeometry();
			PathFigure figure = new PathFigure();
			Point? start;
			Point? end;
			FindBoundaries(edge, out start, out end);
			if (start != null && end != null)
			{
				figure.StartPoint = start.Value;
				PathSegment segment = null;
				if (edge.AnchoringMode == EdgeAnchoringMode.NodeToNode && edge.SourceNode != null && edge.SourceNode == edge.DestinationNode)
				{
					double a = edge.SourceNode.Bounds.Width / 2
						+ GlobalData.SelfArcWingAmplification * edge.SourceNode.Bounds.Width / 2;
					double b = edge.SourceNode.Bounds.Height / 2;
					segment = new ArcSegment(end.Value, new Size(a, b), 0, true, SweepDirection.Clockwise, true);
				}
				else
				{
					segment = new LineSegment(end.Value, true);
				}
				figure.Segments.Add(segment);
			}
			result.Figures.Add(figure);

			return result;
		}

		protected override DiagramSelectionBorder CalculateEdgeBorder(DiagramEdge edge)
		{
			var geometry = edge.Geometry as PathGeometry;
			if (geometry == null)
				return null;

			var border = new DiagramSelectionBorder(false);

			for (int i = 0; i < geometry.Figures.Count; i++)
			{
				var currentFigure = geometry.Figures[i];
				border.ResizeInfos.Add(new ResizeInfo(currentFigure.StartPoint, i == 0 ? ResizeDirection.AllRoundSource : ResizeDirection.AllRound));
				for (int j = 0; j < currentFigure.Segments.Count; j++)
				{
					PathSegment currentSegment = currentFigure.Segments[j];
					ResizeDirection? direction = ResizeDirection.AllRound;
					if (i == geometry.Figures.Count - 1 && j == currentFigure.Segments.Count - 1)
						direction = ResizeDirection.AllRoundDestination;
					LineSegment lineSegment = currentSegment as LineSegment;
					if (lineSegment != null)
					{
						border.ResizeInfos.Add(new ResizeInfo(lineSegment.Point, direction));
						continue;
					}
					ArcSegment arcSegment = currentSegment as ArcSegment;
					if (arcSegment != null)
					{
						border.ResizeInfos.Add(new ResizeInfo(arcSegment.Point, direction));
						continue;
					}
				}
			}

			return border;
		}

		//protected override void DrawEdge(DrawingContext dc, DiagramEdge edge)
		//{
		//    if (edge.Geometry == null)
		//        return;

		//    dc.DrawGeometry(edge.Background, edge.BorderPen, edge.Geometry);
		//}

		//protected override void DrawNonPolylineShape(DrawingContext dc, IShape shape, Brush brush, Pen pen, Diagram diagram)
		//{
		//    ArcEx arc = shape as ArcEx;
		//    if (arc == null)
		//        return;

		//    PathGeometry geometry = new PathGeometry();

		//    PathFigure path = new PathFigure();
		//    path.StartPoint = arc.StartPoint.ToWpfPoint().ToViewPoint(diagram);
		//    path.Segments.Add(new ArcSegment(arc.EndPoint.ToWpfPoint().ToViewPoint(diagram), new Size(arc.A, arc.B).ToViewSize(diagram), 0, arc.IsLargeArc, SweepDirection.Clockwise, true));

		//    geometry.Figures.Add(path);
		//    dc.DrawGeometry(brush, pen, geometry);

		//}

		#endregion
	}
}
