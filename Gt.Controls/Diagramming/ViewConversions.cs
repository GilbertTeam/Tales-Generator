using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Gt.Controls.Diagramming
{
	public static class ViewConversions
	{
		public static Point ToViewPoint(this Point point, Diagram diagram)
		{
			return new Point(point.X + diagram.XViewOffset, point.Y + diagram.YViewOffset);
		}

		public static Point ToViewPoint(this Point point, Vector offset, double scale)
		{
			return new Point(point.X / scale - offset.X, point.Y / scale - offset.Y);
		}

		public static Point ToDisplayPoint(this Point point, Vector offset, double scale)
		{
			return new Point((point.X + offset.X) * scale, (point.Y + offset.Y) * scale);
		}

		public static Size ToViewSize(this Size size, Diagram diagram)
		{
			return new Size(size.Width, size.Height);
		}

		public static Rect ToViewRect(this Rect rect, Diagram diagram)
		{
			return new Rect(rect.X + diagram.XViewOffset, rect.Y + diagram.YViewOffset, rect.Width, rect.Height);
		}

		public static double Square(this Rect rect)
		{
			return rect.Width * rect.Height;
		}
	}
}
