using System.Windows.Input;

namespace Gt.Controls.Diagramming
{
	class Utils
	{
		public static Cursor ResizeDirectionToCursor(ResizeDirection direction)
		{
			Cursor result;

			switch (direction)
			{
				case ResizeDirection.None:
					result = Cursors.Arrow;
					break;
				case ResizeDirection.HorizontalLeft:
				case ResizeDirection.HorizontalRight:
					result = Cursors.SizeWE;
					break;
				case ResizeDirection.VerticalTop:
				case ResizeDirection.VerticalBottom:
					result = Cursors.SizeNS;
					break;
				case ResizeDirection.AscendingTopRight:
				case ResizeDirection.AscendingBottomLeft:
					result = Cursors.SizeNESW;
					break;
				case ResizeDirection.DescendingTopLeft:
				case ResizeDirection.DescendingBottomRight:
					result = Cursors.SizeNWSE;
					break;
				default:
					result = Cursors.SizeAll;
					break;
			}

			return result;
		}
	}
}
