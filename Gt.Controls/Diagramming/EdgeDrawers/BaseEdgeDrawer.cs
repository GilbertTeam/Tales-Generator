using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Gt.Controls.Diagramming.EdgeDrawers
{
	public abstract class BaseEdgeDrawer : BaseDrawer, IDiagramItemDrawer
	{
		#region Methods

		public Geometry CalculateGeometry(DiagramItem item)
		{
			var edge = item as DiagramEdge;
			if (edge == null)
				throw new DiagramException("");

			return CalculateEdgeGeometry(edge);
		}

		protected abstract Geometry CalculateEdgeGeometry(DiagramEdge edge);

		public DiagramSelectionBorder CalculateBorder(DiagramItem item)
		{
			var edge = item as DiagramEdge;
			if (edge == null)
				throw new DiagramException("Тип отрисовщика не соответствует типу итема");

			return CalculateEdgeBorder(edge);
		}

		protected abstract DiagramSelectionBorder CalculateEdgeBorder(DiagramEdge edge);

		public void Draw(DrawingContext dc, Rect viewport, DiagramItem item)
		{
			var edge = item as DiagramEdge;
			if (edge == null)
				throw new DiagramException("Тип отрисовщика не соответствует типу итема");

			//DrawEdge(dc, edge);
			DrawGeometry(dc, edge.Geometry, edge.Background, edge.BorderPen, edge.Diagram.Offset, edge.Diagram.Scale);
			if (edge.Diagram.Selection.Contains(edge))
			{
				DrawSelectionBorder(edge.Diagram, dc, edge.Border);
			}
		}

		public void DrawSelectionBorder(DrawingContext dc, DiagramItem item)
		{
			if (item == null)
				return;

			DrawSelectionBorder(item.Diagram, dc, item.Border);
		}

		//protected abstract void DrawEdge(DrawingContext dc, DiagramEdge edge);

		protected void FindBoundaries(DiagramEdge edge, out Point? start, out Point? end)
		{
			start = null;
			end = null;

			Point? tempStartPoint;
			Point? tempEndPoint;
			Geometry sourceNodeShape = null;
			Geometry destinationNodeShape = null;

			FindTempBoundaries(edge, out tempStartPoint, out tempEndPoint);

			switch (edge.AnchoringMode)
			{
				case EdgeAnchoringMode.PointToPoint:
					start = tempStartPoint;
					end = tempEndPoint;
					break;
				case EdgeAnchoringMode.PointToNode:
					start = tempStartPoint;
					destinationNodeShape = edge.DestinationNode != null ? edge.DestinationNode.Geometry : null;
					break;
				case EdgeAnchoringMode.NodeToPoint:
					sourceNodeShape = edge.SourceNode != null ? edge.SourceNode.Geometry : null;
					end = tempEndPoint;
					break;
				case EdgeAnchoringMode.NodeToNode:
					sourceNodeShape = edge.SourceNode != null ? edge.SourceNode.Geometry : null;
					destinationNodeShape = edge.DestinationNode != null ? edge.DestinationNode.Geometry : null;
					break;
			}

			if (tempStartPoint != null && tempEndPoint != null && sourceNodeShape != null)
			{
				var line = new LineGeometry(tempStartPoint.Value, tempEndPoint.Value);
				var points = new List<Point>(GeometryUtils.Intersect(sourceNodeShape, edge.SourceNode.BorderPen, 
					line, edge.BorderPen));

				var min = Double.MaxValue;
				Point? minPoint = null;
				foreach (var pointItem in points)
				{
					if (MathUtils.Compare(GeometryUtils.Distance(pointItem, tempEndPoint.Value), 0, GlobalData.PointPrecision) == 0)
						continue;

					var distance = GeometryUtils.Distance(pointItem, tempEndPoint.Value);
					if (distance < min)
					{
						min = distance;
						minPoint = pointItem;
					}
				}

				if (minPoint != null)
					start = new Point(minPoint.Value.X, minPoint.Value.Y);
				else start = tempStartPoint;
			}

			if (tempStartPoint != null && tempEndPoint != null && destinationNodeShape != null)
			{
				var line = new LineGeometry(tempStartPoint.Value, tempEndPoint.Value);
				var points = new List<Point>(GeometryUtils.Intersect(destinationNodeShape, edge.DestinationNode.BorderPen, 
					line, edge.BorderPen));

				var min = Double.MaxValue;
				Point? minPoint = null;
				foreach (var pointItem in points)
				{
					if (MathUtils.Compare(GeometryUtils.Distance(tempStartPoint.Value, pointItem), 0, GlobalData.PointPrecision) == 0)
						continue;

					var distance = GeometryUtils.Distance(tempStartPoint.Value, pointItem);
					if (distance < min)
					{
						min = distance;
						minPoint = pointItem;
					}
				}

				if (minPoint != null)
					end = new Point(minPoint.Value.X, minPoint.Value.Y);
				else end = tempEndPoint;
			}

			if ((edge.AnchoringMode == EdgeAnchoringMode.NodeToPoint || edge.AnchoringMode == EdgeAnchoringMode.NodeToNode) && start != null)
			{
				edge.SourcePoint = start.Value;
			}

			if ((edge.AnchoringMode == EdgeAnchoringMode.PointToNode || edge.AnchoringMode == EdgeAnchoringMode.NodeToNode) && end != null)
			{
				edge.DestinationPoint = end.Value;
			}
		}

		protected void FindTempBoundaries(DiagramEdge edge, out Point? start, out Point? end)
		{
			start = null;
			end = null;

			Rect? sourceNodeBounds = null;
			if (edge.SourceNode != null)
			{
				sourceNodeBounds = edge.SourceNode.Bounds;
			}
			Rect? destNodeBounds = null;
			if (edge.DestinationNode != null)
			{
				destNodeBounds = edge.DestinationNode.Bounds;
			}

			switch (edge.AnchoringMode)
			{
				case EdgeAnchoringMode.PointToPoint:
					start = new Point(edge.SourcePoint.X, edge.SourcePoint.Y);
					end = new Point(edge.DestinationPoint.X, edge.DestinationPoint.Y);
					break;
				case EdgeAnchoringMode.PointToNode:
					start = new Point(edge.SourcePoint.X, edge.SourcePoint.Y);
					if (destNodeBounds.HasValue)
					{
						end = new Point((destNodeBounds.Value.Left + destNodeBounds.Value.Right) / 2, (destNodeBounds.Value.Top + destNodeBounds.Value.Bottom) / 2);
					}
					break;
				case EdgeAnchoringMode.NodeToPoint:
					if (sourceNodeBounds.HasValue)
					{
						start = new Point((sourceNodeBounds.Value.Left + sourceNodeBounds.Value.Right) / 2, (sourceNodeBounds.Value.Top + sourceNodeBounds.Value.Bottom) / 2);
					}
					end = new Point(edge.DestinationPoint.X, edge.DestinationPoint.Y);
					break;
				case EdgeAnchoringMode.NodeToNode:
					if (edge.SourceNode != null && edge.DestinationNode != null && edge.SourceNode == edge.DestinationNode && sourceNodeBounds.HasValue)
					{
						double y = sourceNodeBounds.Value.Top + 0.25 * sourceNodeBounds.Value.Height;
						start = new Point(sourceNodeBounds.Value.Left, y);
						end = new Point(sourceNodeBounds.Value.Right, y);
					}
					else
					{
						if (sourceNodeBounds.HasValue)
						{
							start = new Point((sourceNodeBounds.Value.Left + sourceNodeBounds.Value.Right) / 2, (sourceNodeBounds.Value.Top + sourceNodeBounds.Value.Bottom) / 2);
						}
						if (destNodeBounds.HasValue)
						{
							end = new Point((destNodeBounds.Value.Left + destNodeBounds.Value.Right) / 2, (destNodeBounds.Value.Top + destNodeBounds.Value.Bottom) / 2);
						}
					}
					break;
			}
		}

		#endregion
	}
}
