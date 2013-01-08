using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Gt.Controls.Diagramming
{
	class DiagramSelector
	{
		#region fields

		#endregion

		#region ctors

		public DiagramSelector()
		{
			StartPoint = new Point();
			EndPoint = new Point();
		}

		#endregion

		#region props

		public Point StartPoint { get; set; }

		public Point EndPoint { get; set; }

		public Point TopLeft
		{
			get { return GetTopLeft(); }
		}

		public Point BottomRight
		{
			get { return GetBottomRight(); }
		}

		public Geometry Geometry
		{
			get { return GetGeometry(); }
		}

		#endregion

		#region methods

		protected Point GetTopLeft()
		{
			return new Point(StartPoint.X < EndPoint.X ? StartPoint.X : EndPoint.X,
				StartPoint.Y < EndPoint.Y ? StartPoint.Y : EndPoint.Y);
		}

		protected Point GetBottomRight()
		{
			return new Point(StartPoint.X > EndPoint.X ? StartPoint.X : EndPoint.X,
				StartPoint.Y > EndPoint.Y ? StartPoint.Y : EndPoint.Y);
		}

		protected Geometry GetGeometry()
		{
			return new RectangleGeometry(new Rect(TopLeft, BottomRight));
		}

		#endregion


	}
}
