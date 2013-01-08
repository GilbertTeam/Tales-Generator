using System.Windows.Media;
using System.Windows;
using System;

namespace Gt.Controls
{
	public static class GeometryUtils
	{
		public static bool HitTest(this Geometry geometry, Point hitPoint, Pen pen)
		{
			return geometry.FillContains(hitPoint) || geometry.StrokeContains(pen, hitPoint, GlobalData.PointPrecision, ToleranceType.Absolute);
		}

		public static double Distance(Point p1, Point p2)
		{
			return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
		}

		public static Point[] Intersect(Geometry geometry1, Pen pen1, Geometry geometry2, Pen pen2)
		{
			Geometry og1 = geometry1.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));
			Geometry og2 = geometry2.GetWidenedPathGeometry(new Pen(Brushes.Black, 1.0));

			CombinedGeometry cg = new CombinedGeometry(GeometryCombineMode.Intersect, og1, og2);

			PathGeometry pg = cg.GetFlattenedPathGeometry();
			Point[] result = new Point[pg.Figures.Count];
			for (int i = 0; i < pg.Figures.Count; i++)
			{
				Rect fig = new PathGeometry(new PathFigure[] { pg.Figures[i] }).Bounds;
				result[i] = new Point(fig.Left + fig.Width / 2.0, fig.Top + fig.Height / 2.0);
			}
			return result;
		}

		public static bool AreIntersectOrContain(Geometry geometry1, Pen pen1, Geometry geometry2, Pen pen2)
		{
			PathGeometry og1 = geometry1.GetWidenedPathGeometry(pen1);
			PathGeometry og2 = geometry2.GetWidenedPathGeometry(pen2);

			CombinedGeometry cg = new CombinedGeometry(GeometryCombineMode.Intersect,og1, og2);
			PathGeometry pg = cg.GetFlattenedPathGeometry();

			return pg.Figures.Count > 0 || geometry1.FillContains(geometry2) || geometry2.FillContains(geometry1);
		}
	}
}
