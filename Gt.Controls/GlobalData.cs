using System.Windows;
using System.Windows.Media;

namespace Gt.Controls
{
	class GlobalData
	{
		public static readonly double ResizeRectEdgeLength = 5;

		public static readonly Size ResizeRectSize = new Size(ResizeRectEdgeLength, ResizeRectEdgeLength);

		public static readonly SolidColorBrush TransparentBrush = new SolidColorBrush(Colors.Transparent);

		public static readonly SolidColorBrush BorderBrush = new SolidColorBrush(Colors.Black);

		public static readonly Pen BorderPen = new Pen(BorderBrush, 1);

		public static readonly double DefaulNodeEdgeLength = 25;

		public static readonly Size DefaultNodeSize = new Size(DefaulNodeEdgeLength, DefaulNodeEdgeLength);

		public static readonly double SelfArcWingAmplification = 0.2;

		public static readonly double MinTextBoxHeight = 20;

		//BorderUnderCursor
		public static readonly double BUCOffset = 20;

		public static readonly double BUCWingRelativeLength = 0.33; // как грубо!

		public static readonly double BUCThickness = 2;

		public static readonly SolidColorBrush BUCBrush = new SolidColorBrush(Colors.DarkGray);

		public static readonly Pen BUCPen = new Pen(BUCBrush, BUCThickness);

		//Константы
		public static readonly double Precision = 0.000001;

		public static readonly double PointPrecision = 3;
	}
}
