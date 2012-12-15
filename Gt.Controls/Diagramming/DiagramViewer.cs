using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Gt.Controls.Diagramming
{
	public class DiagramViewer : Control
	{
		#region Fields

		protected Diagram _diagram;

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
		#endregion

		#endregion
	}
}
