using System;
using System.Windows;

namespace Gt.Controls
{
	public class MathUtils
	{
		public static int Compare(double a, double b, double epsilon)
		{
			double delta = a - b;
			if (Math.Abs(delta) <= epsilon) return 0;
			return (delta < 0 ? -1 : 1);
		}

		public static double GetAngle(Point point)
		{
			double absoluteAngle = Math.Atan(Math.Abs(point.X) / Math.Abs(point.Y));
			if (point.X < 0 && point.Y >= 0)
			{
				absoluteAngle = Math.PI - absoluteAngle;
			}
			if (point.X < 0 && point.Y < 0)
			{
				absoluteAngle = Math.PI + absoluteAngle;
			}
			if (point.X > 0 && point.Y < 0)
			{
				absoluteAngle = 2 * Math.PI - absoluteAngle;
			}
			return absoluteAngle;
		}

		public static double RadiansToGraduses(double angle)
		{
			return angle / Math.PI * 180;
		}

		public static double SimplifyAngle(double angle)
		{
			while (angle < 0)
				angle += 360;
			while (angle > 360)
				angle -= 360;
			return angle;
		}
	}
}
