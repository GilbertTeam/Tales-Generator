using System.Windows;
using System.Windows.Media;

namespace Gt.Controls.Diagramming.NodeDrawers
{
	public abstract class BaseNodeDrawer : BaseDrawer, IDiagramItemDrawer
	{
		#region Constructors

		#endregion

		#region Methods

		public Geometry CalculateGeometry(DiagramItem item)
		{
			var node = item as DiagramNode;
			if (node == null)
				throw new DiagramException("Тип итема не соответствует типу отрисовщика");

			return CalculateNodeGeometry(node);
		}

		protected abstract Geometry CalculateNodeGeometry(DiagramNode node);

		public DiagramSelectionBorder CalculateBorder(DiagramItem item)
		{
			var node = item as DiagramNode;
			if (node == null)
				throw new DiagramException("Тип итема не соответствует типу отрисовщика");

			return CalculateNodeBorder(node);
		}

		protected DiagramSelectionBorder CalculateNodeBorder(DiagramNode node)
		{
			var border = new DiagramSelectionBorder();

			var bounds = node.Bounds;

			border.ResizeInfos.Add(new ResizeInfo(bounds.TopLeft, ResizeDirection.DescendingTopLeft));
			border.ResizeInfos.Add(new ResizeInfo(new Point((bounds.Left + bounds.Right) / 2, bounds.Top), ResizeDirection.VerticalTop));
			border.ResizeInfos.Add(new ResizeInfo(bounds.TopRight, ResizeDirection.AscendingTopRight));
			border.ResizeInfos.Add(new ResizeInfo(new Point(bounds.Right, (bounds.Top + bounds.Bottom) / 2), ResizeDirection.HorizontalRight));
			border.ResizeInfos.Add(new ResizeInfo(bounds.BottomRight, ResizeDirection.DescendingBottomRight));
			border.ResizeInfos.Add(new ResizeInfo(new Point((bounds.Left + bounds.Right) / 2, bounds.Bottom), ResizeDirection.VerticalBottom));
			border.ResizeInfos.Add(new ResizeInfo(bounds.BottomLeft, ResizeDirection.AscendingBottomLeft));
			border.ResizeInfos.Add(new ResizeInfo(new Point(bounds.Left, (bounds.Top + bounds.Bottom) / 2), ResizeDirection.HorizontalLeft));

			return border;
		}

		public void Draw(DrawingContext dc, Rect viewport, DiagramItem item)
		{
			var node = item as DiagramNode;
			if (node == null)
				throw new DiagramException("Тип итема не соответствует типу отрисовщика");

			DrawGeometry(dc, node.Geometry, node.Background, node.BorderPen, node.Diagram.Offset, node.Diagram.Scale);
			//DrawNode(dc, node);
			if (node.Diagram.Selection.Contains(node))
			{
				DrawSelectionBorder(item.Diagram, dc, item.Border);
			}
			if (node.IsUnderCursor)
			{
				DrawBUC(item.Diagram, dc, node.Bounds);
			}
		}

		//protected void DrawNode(DrawingContext dc, DiagramNode node)
		//{
		//    var diagram = node.Diagram;
		//    if (diagram == null)
		//        return;

		//    GeometryTranslater translater = new GeometryTranslater(node.Geometry);
		//    dc.DrawGeometry(node.Background, node.BorderPen, translater.Execute(new Vector(diagram.XViewOffset, diagram.YViewOffset), diagram.Zoom));
		//}

		#endregion
	}
}
