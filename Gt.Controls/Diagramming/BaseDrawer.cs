using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace Gt.Controls.Diagramming
{
	public abstract class BaseDrawer
	{
		#region Methods

		protected void DrawSelectionBorder(Diagram diagram, DrawingContext dc, DiagramSelectionBorder border)
		{
			if (border == null)
				return;

			for (var i = 0; i < border.ResizeInfos.Count; i++)
			{
				var curInfo = border.ResizeInfos[i];
				var prevPointIndex = i != 0 ? i - 1 : border.ResizeInfos.Count - 1;
				var prevInfo = border.ResizeInfos[prevPointIndex];
				if (!curInfo.Point.HasValue || !prevInfo.Point.HasValue)
					continue;

				if (border.IsLinked)
					dc.DrawLine(GlobalData.BorderPen, curInfo.Point.Value, prevInfo.Point.Value);//GeometryTranslater.OffsetPoint(, diagram.Offset, diagram.Scale), GeometryTranslater.OffsetPoint(prevInfo.Point.Value, diagram.Offset, diagram.Scale));

				var resizeRect = curInfo.Rect;
				if (!resizeRect.HasValue)
					continue;

				if (curInfo.ResizeDirection != ResizeDirection.None)
					dc.DrawRectangle(GlobalData.BorderBrush, GlobalData.BorderPen, resizeRect.Value);//GeometryTranslater.OffsetRect(resizeRect.Value, diagram.Offset, diagram.Scale));
			}
		}

		protected void DrawBUC(Diagram diagram, DrawingContext dc, Rect rect)
		{
			//rect = GeometryTranslater.OffsetRect(rect, diagram.Offset, diagram.Scale);

			var topLeft = rect.TopLeft;
			topLeft.Offset(-GlobalData.BUCOffset, -GlobalData.BUCOffset);
			dc.DrawLine(GlobalData.BUCPen, topLeft, new Point(topLeft.X, topLeft.Y + GlobalData.BUCWingRelativeLength * rect.Height));
			dc.DrawLine(GlobalData.BUCPen, topLeft, new Point(topLeft.X + GlobalData.BUCWingRelativeLength * rect.Width, topLeft.Y));

			var topRight = rect.TopRight;
			topRight.Offset(GlobalData.BUCOffset, -GlobalData.BUCOffset);
			dc.DrawLine(GlobalData.BUCPen, new Point(topRight.X - GlobalData.BUCWingRelativeLength * rect.Width, topRight.Y), topRight);
			dc.DrawLine(GlobalData.BUCPen, topRight, new Point(topRight.X, topRight.Y + GlobalData.BUCWingRelativeLength * rect.Height));

			var bottomRight = rect.BottomRight;
			bottomRight.Offset(GlobalData.BUCOffset, GlobalData.BUCOffset);
			dc.DrawLine(GlobalData.BUCPen, new Point(bottomRight.X, bottomRight.Y - GlobalData.BUCWingRelativeLength * rect.Height), bottomRight);
			dc.DrawLine(GlobalData.BUCPen, bottomRight, new Point(bottomRight.X - GlobalData.BUCWingRelativeLength * rect.Width, bottomRight.Y));

			var bottomLeft = rect.BottomLeft;
			bottomLeft.Offset(-GlobalData.BUCOffset, GlobalData.BUCOffset);
			dc.DrawLine(GlobalData.BUCPen, new Point(bottomLeft.X + GlobalData.BUCWingRelativeLength * rect.Width, bottomLeft.Y), bottomLeft);
			dc.DrawLine(GlobalData.BUCPen, bottomLeft, new Point(bottomLeft.X, bottomLeft.Y - GlobalData.BUCWingRelativeLength * rect.Height));
		}

		protected void DrawGeometry(DrawingContext dc, Geometry geometry, Brush brush, Pen pen, Vector offset, double scale)
		{
			//GeometryTranslater translater = new GeometryTranslater(geometry);
			dc.DrawGeometry(brush, pen, geometry);//translater.Execute(offset, scale));
		}

		protected void DrawFormattedText(DrawingContext dc, FormattedText text, Point origin, Vector offset, double scale)
		{
			//ScaleTransform scaleTransform = new ScaleTransform(scale, scale);
			//TranslateTransform translateTransform = new TranslateTransform(offset.X, offset.Y);

			//dc.PushTransform(scaleTransform);
			//dc.PushTransform(translateTransform);

			dc.DrawText(text, origin);

			//dc.Pop();
			//dc.Pop();
		}

		//protected void DrawGeometry(DrawingContext dc, Geometry geometry, Brush brush, Pen pen)
		//{
		//    dc.DrawGeometry(brush, pen, geometry);
		//    //RectangleGeometry rg = new RectangleGeometry(viewport);
		//    //if (GeometryUtils.IsIntersects(rg, GlobalData.BorderPen, geometry, pen))
				
		//}

		//protected void DrawShape(DrawingContext dc, IShape shape, Brush brush, Pen pen, Diagram diagram)
		//{
		//    if (shape == null)
		//        return;

		//    var polylineShape = shape as IPolylineShape;
		//    if (polylineShape != null)
		//        DrawPolylineShape(dc, polylineShape, brush, pen, diagram);
		//    else DrawNonPolylineShape(dc, shape, brush, pen, diagram);
		//}

		//protected void DrawPolylineShape(DrawingContext dc, IPolylineShape polylineShape, Brush brush, Pen pen, Diagram diagram)
		//{
		//    if (polylineShape == null)
		//        return;

		//    var polygon = polylineShape as PolygonEx;
		//    if (polygon != null)
		//    {
		//        DrawPolygon(dc, polygon, brush, pen, diagram);
		//        return;
		//    }

		//    var lines = new List<LineEx>(polylineShape.GenerateLines());

		//    foreach (var line in lines)
		//    {
		//        dc.DrawLine(pen, line.StartPoint.ToWpfPoint().ToViewPoint(diagram),
		//            line.EndPoint.ToWpfPoint().ToViewPoint(diagram));
		//    }
		//}

		//private void DrawPolygon(DrawingContext dc, PolygonEx polygon, Brush brush, Pen pen, Diagram diagram)
		//{
		//    if (polygon.Points.Count < 3)
		//        return;

		//    //TODO 1ый поинт дважды в списке
		//    var streamGeometry = new StreamGeometry();
		//    using (var geometryContext = streamGeometry.Open())
		//    {
		//        geometryContext.BeginFigure(polygon.Points[0].ToWpfPoint().ToViewPoint(diagram), true, true);
		//        var pointCollection = polygon.Points.ToWpfPoints();
		//        for (int i = 1; i < polygon.Points.Count; i++)
		//        {
		//            geometryContext.LineTo(polygon.Points[i].ToWpfPoint().ToViewPoint(diagram), true, true);
		//        }
		//    }
		//    dc.DrawGeometry(brush, pen, streamGeometry);
		//}

		//protected virtual void DrawNonPolylineShape(DrawingContext dc, IShape shape, Brush brush, Pen pen, Diagram diagram)
		//{
		//    if (shape == null)
		//        return;
		//    // всякие эллипсы и прочая ерунда
		//    throw new NotImplementedException();
		//}

		#endregion
	}
}
