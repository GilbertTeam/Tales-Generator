using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace Gt.Controls
{
	class GeometryTranslater
	{
		Geometry _targetGeometry;

		public GeometryTranslater(Geometry target)
		{
			_targetGeometry = target;
		}

		public Geometry Execute(Vector offset, double scale)
		{
			Geometry result = null;

			for (; ; )
			{
				if (_targetGeometry == null)
					break;

				RectangleGeometry rectGeometry = _targetGeometry as RectangleGeometry;
				if (rectGeometry != null)
				{
					result = TransalteRectGeometry(rectGeometry, offset, scale);
					break;
				}

				EllipseGeometry ellipseGeometry = _targetGeometry as EllipseGeometry;
				if (ellipseGeometry != null)
				{
					result = TranslateEllipseGeometry(ellipseGeometry, offset, scale);
				}

				PathGeometry pathGeometry = _targetGeometry as PathGeometry;
				if (pathGeometry != null)
				{
					result = TranslatePathGeometry(pathGeometry, offset, scale);
				}

				LineGeometry lineGeometry = _targetGeometry as LineGeometry;
				if (lineGeometry != null)
				{
					result = TranslateLineGeometry(lineGeometry, offset, scale);
					break;
				}

				break;
			}

			return result;
		}

		private Geometry TransalteRectGeometry(RectangleGeometry rectGeometry, Vector offset, double scale)
		{
			RectangleGeometry result = new RectangleGeometry();
			result.Rect = OffsetRect(rectGeometry.Rect, offset, scale);
			result.RadiusX = rectGeometry.RadiusX;
			result.RadiusY = rectGeometry.RadiusY;
			return result;
		}

		private Geometry TranslateEllipseGeometry(EllipseGeometry ellipseGeometry, Vector offset, double scale)
		{
			EllipseGeometry result = new EllipseGeometry();
			result.Center = OffsetPoint(ellipseGeometry.Center, offset, scale);
			result.RadiusX = ellipseGeometry.RadiusX * scale;
			result.RadiusY = ellipseGeometry.RadiusY * scale;
			return result;
		}

		private Geometry TranslatePathGeometry(PathGeometry pathGeometry, Vector offset, double scale)
		{
			PathGeometry result = new PathGeometry();

			for (int i = 0; i < pathGeometry.Figures.Count; i++)
			{
				PathFigure sourceFigure = pathGeometry.Figures[i];
				PathFigure destinationFigure = new PathFigure();
				destinationFigure.IsClosed = sourceFigure.IsClosed;
				destinationFigure.IsFilled = sourceFigure.IsFilled;
				destinationFigure.StartPoint = OffsetPoint(sourceFigure.StartPoint, offset, scale);
				for (int j = 0; j < sourceFigure.Segments.Count; j++)
				{
					PathSegment sourceSegment = sourceFigure.Segments[j];
					PathSegment destinationSegment = null;

					for (; ; )
					{
						ArcSegment arcSegment = sourceSegment as ArcSegment;
						if (arcSegment != null)
						{
							destinationSegment = TranslateArcSegment(arcSegment, offset, scale);
							break;
						}

						LineSegment lineSegment = sourceSegment as LineSegment;
						if (lineSegment != null)
						{
							destinationSegment = TranslateLineSegment(lineSegment, offset, scale);
							break;
						}

						break;
					}

					if (destinationSegment != null)
					{
						destinationFigure.Segments.Add(destinationSegment);
					}
				}
				result.Figures.Add(destinationFigure);
			}

			return result;
		}

		private PathSegment TranslateLineSegment(LineSegment lineSegment, Vector offset, double scale)
		{
			LineSegment result = new LineSegment();
			result.Point = OffsetPoint(lineSegment.Point, offset, scale);
			result.IsSmoothJoin = lineSegment.IsSmoothJoin;
			result.IsStroked = lineSegment.IsStroked;
			return result;
		}

		private PathSegment TranslateArcSegment(ArcSegment arcSegment, Vector offset, double scale)
		{
			ArcSegment result = new ArcSegment();
			result.Point = OffsetPoint(arcSegment.Point, offset, scale);
			result.IsLargeArc = arcSegment.IsLargeArc;
			result.IsSmoothJoin = arcSegment.IsSmoothJoin;
			result.IsStroked = arcSegment.IsStroked;
			result.RotationAngle = arcSegment.RotationAngle;
			result.Size = new Size(arcSegment.Size.Width * scale, arcSegment.Size.Height * scale);
			result.SweepDirection = arcSegment.SweepDirection;
			return result;
		}

		private Geometry TranslateLineGeometry(LineGeometry lineGeometry, Vector offset, double scale)
		{
			LineGeometry result = new LineGeometry();
			lineGeometry.StartPoint = OffsetPoint(lineGeometry.StartPoint, offset, scale);
			lineGeometry.EndPoint = OffsetPoint(lineGeometry.EndPoint, offset, scale);
			return result;
		}

		public static Point OffsetPoint(Point point, Vector offset, double scale)
		{
			return new Point((point.X + offset.X) * scale, (point.Y + offset.Y) * scale);
		}

		public static Rect OffsetRect(Rect rect, Vector offset, double scale)
		{
			Rect result = new Rect(rect.TopLeft, rect.BottomRight);
			result.Offset(offset);
			result.Scale(scale, scale);
			return result;
		}
	}
}
