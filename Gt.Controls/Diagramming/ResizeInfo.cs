using System.Windows;

namespace Gt.Controls.Diagramming
{
	public struct ResizeInfo
	{
		#region Fields

		Point? _point;

		Rect? _rect;

		ResizeDirection? _direction;

		#endregion

		#region Constructors

		public ResizeInfo(Point? point = null, ResizeDirection? direction = null)
		{
			_point = point;
			_direction = direction;
			_rect = null;
		}

		#endregion

		#region Properties

		public Point? Point
		{
			get { return _point; }
			set
			{
				if (_point != value)
				{
					_point = value;

					CalculateRect();
				}
			}
		}

		public ResizeDirection? ResizeDirection
		{
			get { return _direction; }
			set { _direction = value; }
		}

		public Rect? Rect
		{
			get
			{
				if (_rect == null)
					CalculateRect();
				return _rect;
			}
		}

		#endregion

		#region Methods

		private void CalculateRect()
		{
			if (!_point.HasValue)
			{
				_rect = null;

				return;
			}

			var offsetX = -GlobalData.ResizeRectEdgeLength / 2;
			var offsetY = -GlobalData.ResizeRectEdgeLength / 2;

			var point = _point.Value;

			point.Offset(offsetX, offsetY);

			_rect = new Rect(point, GlobalData.ResizeRectSize);
		}

#endregion
	}
}
