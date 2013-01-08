using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace Gt.Controls.Diagramming
{
	public class DiagramViewer : Control
	{
		#region Fields

		protected Diagram _diagram;

		protected bool _isLeftButtonDown;

		#endregion

		#region Ctor

		public DiagramViewer()
		{

		}

		#endregion

		#region Props

		public Diagram Diagram
		{
			get { return _diagram; }
			set
			{
				if (_diagram != value)
				{
					if (_diagram != null)
					{
						_diagram.DiagramRender -= new DiagramNotifyDelegate(InvalidateVisual);
					}

					_diagram = value;

					if (_diagram != null)
					{
						_diagram.DiagramRender += new DiagramNotifyDelegate(InvalidateVisual);
					}

					InvalidateVisual();
				}
			}
		}

		#endregion

		#region Methods

		#region Render

		protected override void OnRender(DrawingContext dc)
		{
			//warning : много хаков!
			if (_diagram == null)
				return;

			Rect actualViewport = new Rect(0, 0, ActualWidth, ActualHeight);

			dc.DrawRectangle(Brushes.White, null, actualViewport);

			Rect boundaries = _diagram.Boundaries;
			Rect viewport = _diagram.Viewport;

			double scale;

			double scaleX = actualViewport.Width / boundaries.Width;
			double scaleY = actualViewport.Height / boundaries.Height;

			scale = scaleX > scaleY ? scaleY : scaleX;

			//прямоугольник с элементами
			Rect viewerBoundaries = new Rect(0, 0, boundaries.Width * scale, boundaries.Height * scale);
			dc.DrawRectangle(Brushes.White, GlobalData.BorderPen, viewerBoundaries);

			//вьюпорт
			Vector offset = new Vector(-boundaries.Left, -boundaries.Top);
			Rect viewerViewport = new Rect(viewport.TopLeft, viewport.BottomRight);
			viewerViewport.Offset(offset.X , offset.Y);
			viewerViewport.Scale(scale, scale);

			//пересекаем
			RectangleGeometry geometryBoundaries = new RectangleGeometry(viewerBoundaries);
			RectangleGeometry geometryViewport = new RectangleGeometry(viewerViewport);
			CombinedGeometry geometryCombined = new CombinedGeometry(GeometryCombineMode.Exclude,
				geometryBoundaries, geometryViewport);

			SolidColorBrush brush = new SolidColorBrush(Colors.LightGray);
			dc.PushOpacity(0.3);

			dc.DrawGeometry(brush, null, geometryCombined);

			dc.Pop();

			dc.PushTransform(new ScaleTransform(scale, scale));
			dc.PushTransform(new TranslateTransform(offset.X, offset.Y));

			_diagram.DrawItems(dc);

			dc.Pop();
			dc.Pop();
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			CaptureMouse();

			if (e.ChangedButton != MouseButton.Left)
				return;

			_isLeftButtonDown = true;

			Point point = e.GetPosition(this);
			GoToPoint(point);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				_isLeftButtonDown = false;

			 ReleaseMouseCapture();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!_isLeftButtonDown)
				return;

			Point point = e.GetPosition(this);

			GoToPoint(point);
		}

		protected void GoToPoint(Point point)
		{
			Rect actualViewport = new Rect(0, 0, ActualWidth, ActualHeight);

			Rect boundaries = _diagram.Boundaries;
			Rect viewport = _diagram.Viewport;

			double scale;

			double scaleX = actualViewport.Width / boundaries.Width;
			double scaleY = actualViewport.Height / boundaries.Height;

			scale = scaleX > scaleY ? scaleY : scaleX;

			Vector offset = new Vector(-boundaries.Left, -boundaries.Top);

			Rect viewerBoundaries = new Rect(0, 0, boundaries.Width * scale, boundaries.Height * scale);

			if (point.Y > viewerBoundaries.Height || point.X > viewerBoundaries.Width)
				return;

			Point diagramPoint = new Point(point.X / scale + boundaries.Left, point.Y / scale + boundaries.Top);

			Vector newViewOffset = new Vector();
			newViewOffset.X = diagramPoint.X - viewport.Width / 2;
			newViewOffset.Y = diagramPoint.Y - viewport.Height / 2;

			using (DiagramUpdateLock locker = new DiagramUpdateLock(_diagram))
			{
				_diagram.XViewOffset = -newViewOffset.X;
				_diagram.YViewOffset = -newViewOffset.Y;
			}
		}

		#endregion

		#endregion
	}
}
