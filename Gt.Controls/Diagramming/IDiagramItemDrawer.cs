using System.Windows.Media;
using System.Windows;

namespace Gt.Controls.Diagramming
{
	public interface IDiagramItemDrawer
	{
		//		IShape CalculateShape(DiagramItem item);
		Geometry CalculateGeometry(DiagramItem item);
		DiagramSelectionBorder CalculateBorder(DiagramItem item);

		void Draw(DrawingContext dc, Rect viewport, DiagramItem item);
		//void DrawSelectionBorder(DrawingContext dc, Rect viewport, DiagramItem item);
	}
}
