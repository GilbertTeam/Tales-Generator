using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Gt.Controls.Diagramming.LabelDrawers
{
	public class BaseLabelDrawer : BaseDrawer, IDiagramItemDrawer
	{
		#region Constructors

		#endregion

		#region Properties

		protected FontFamily FontFamily { get; set; }

		protected FontStyle FontStyle { get; set; }

		protected FontStretch FontStretch { get; set; }

		protected FontWeight FontWeight { get; set; }

		protected double FontSize { get; set; }

		protected Brush Foreground { get; set; }

		protected string Text { get; set; }

		#endregion

		#region Methods

		private FormattedText CreateFormattedText(string text)
		{
			var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
			return new FormattedText(text, CultureInfo.InvariantCulture,
				FlowDirection.LeftToRight, typeface, FontSize * (96.0 / 72.0), Foreground);
		}

		public Geometry CalculateGeometry(DiagramItem item)
		{
			var label = item as DiagramLabel;
			if (label == null)
				throw new DiagramException("");

			return CalculateLabelGeometry(label);
		}

		private Geometry CalculateLabelGeometry(DiagramLabel label)
		{
			FontFamily = label.FontFamily;
			FontSize = label.FontSize;
			FontStretch = label.FontStretch;
			FontStyle = label.FontStyle;
			FontWeight = label.FontWeight;
			Foreground = label.Foreground;
			Text = label.Text;

			var node = label.Owner as DiagramNode;
			if (node != null)
			{
				return CalculateLabelOnNode(label, node);
			}

			var edge = label.Owner as DiagramEdge;
			if (edge != null)
			{
				return CalculateLabelOnEdge(label, edge);
			}

			return null;
		}

		//public IShape CalculateShape(DiagramItem item)
		//{
		//    var label = item as DiagramLabel;
		//    if (label == null)
		//        throw new DiagramException("Тип объекта не соответствует типу отрисовщика!");

		//    return CalculateLabelShape(label);
		//}

		//protected IShape CalculateLabelShape(DiagramLabel label)
		//{
		//    FontFamily = label.FontFamily;
		//    FontSize = label.FontSize;
		//    FontStretch = label.FontStretch;
		//    FontStyle = label.FontStyle;
		//    FontWeight = label.FontWeight;
		//    Foreground = label.Foreground;

		//    var node = label.Owner as DiagramNode;
		//    if (node != null)
		//    {
		//        return CalculateLabelOnNode(label, node);
		//    }

		//    var edge = label.Owner as DiagramEdge;
		//    if (edge != null)
		//    {
		//        return CalculateLabelOnEdge(label, edge);
		//    }

		//    return null;
		//}

		protected virtual Geometry CalculateLabelOnNode(DiagramLabel label, DiagramNode node)
		{
			var textSize = GetTextSize(label.Text);
			var nodeRect = node.Bounds;
			var origin = new Point((nodeRect.Right + nodeRect.Left) / 2, (nodeRect.Bottom + nodeRect.Top) / 2);
			origin.Offset(label.RelativePosition.X, label.RelativePosition.Y);

			origin.Offset(-textSize.Width / 2, -textSize.Height / 2);

			return new RectangleGeometry(new Rect(origin.X, origin.Y, textSize.Width,
				textSize.Height));
		}

		protected virtual Geometry CalculateLabelOnEdge(DiagramLabel label, DiagramEdge edge)
		{
			var geometry = edge.Geometry;
			if (geometry == null)
				return null;

			Rect bounds = geometry.Bounds;

			var textSize = GetTextSize(label.Text);
			var origin = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
			origin.Offset(label.RelativePosition.X, label.RelativePosition.Y);
			origin.Offset(-textSize.Width / 2, -textSize.Height / 2);

			return new RectangleGeometry(new Rect(origin.X, origin.Y, textSize.Width,
				textSize.Height));
		}

		public DiagramSelectionBorder CalculateBorder(DiagramItem item)
		{
			var label = item as DiagramLabel;
			if (label == null)
				throw new DiagramException("Тип объекта не соответствует типу отрисовщика!");


			return CalculateLabelBorder(label);
		}

		protected virtual DiagramSelectionBorder CalculateLabelBorder(DiagramLabel label)
		{
			var textGeometry = label.Geometry as Geometry;
			if (textGeometry == null)
				return null;

			var rectGeometry = textGeometry as RectangleGeometry;
			if (rectGeometry == null)
				return null;

			var border = new DiagramSelectionBorder();

			border.ResizeInfos.Add(new ResizeInfo(rectGeometry.Bounds.TopLeft, ResizeDirection.None));
			border.ResizeInfos.Add(new ResizeInfo(rectGeometry.Bounds.TopRight, ResizeDirection.None));
			border.ResizeInfos.Add(new ResizeInfo(rectGeometry.Bounds.BottomRight, ResizeDirection.None));
			border.ResizeInfos.Add(new ResizeInfo(rectGeometry.Bounds.BottomLeft, ResizeDirection.None));

			return border;
		}

		private Size GetTextSize(string text)
		{
			var fText = CreateFormattedText(text);
			return new Size(fText.Width, fText.Height);

		}

		public void Draw(DrawingContext dc, Rect viewport, DiagramItem item)
		{
			var label = item as DiagramLabel;
			if (label == null)
				throw new DiagramException("Тип объекта не соответствует типу отрисовщика!");

			DrawLabel(dc, label);

			if (label.Diagram != null && label.Diagram.Selection.Contains(item))
			{
				DrawSelectionBorder(item.Diagram, dc, item.Border);
			}
		}

		public void DrawSelectionBorder(DrawingContext dc, DiagramItem item)
		{
			if (item == null)
				return;

			DrawSelectionBorder(item.Diagram, dc, item.Border);
		}

		protected virtual void DrawLabel(DrawingContext dc, DiagramLabel label)
		{
			var geometry = label.Geometry;
			if (geometry == null)
				return;

			DrawGeometry(dc, label.Geometry, label.Background, label.BorderPen, label.Diagram.Offset, label.Diagram.Scale);
			//dc.DrawGeometry(label.Background, label.BorderPen, geometry);

			var origin = geometry.Bounds.TopLeft;
			var fText = CreateFormattedText(label.Text);

			

			DrawFormattedText(dc, CreateFormattedText(label.Text), geometry.Bounds.TopLeft, label.Diagram.Offset, label.Diagram.Scale);
			//dc.DrawText(fText, origin);
		}

		//protected override void DrawNonPolylineShape(DrawingContext dc, IShape shape, Brush brush, Pen pen, Diagram diagram)
		//{
		//    var textShape = shape as TextShape;
		//    if (textShape == null)
		//        return;

		//    DrawPolylineShape(dc, textShape.Polygon, brush, pen, diagram);
		//    var origin = textShape.Polygon.Points[0];

		//    var fText = CreateFormattedText(textShape.Text);
		//    dc.DrawText(fText, new Point(origin.X, origin.Y).ToViewPoint(diagram));
		//}

		#endregion
	}
}
