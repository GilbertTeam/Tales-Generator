using System.Linq;

namespace Gt.Controls.Diagramming
{
	public class DiagramEdges : DiagramItemCollection<DiagramEdge>
	{
		#region Constructors

		internal DiagramEdges(Diagram diagram)
			: base(diagram)
		{
		}

		#endregion

		#region Methods

		protected override void SetItem(int index, DiagramEdge item)
		{
			base.SetItem(index, item);
		}

		protected override void InsertItem(int index, DiagramEdge item)
		{
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			var edge = this[index];

			var nodes = Diagram.Nodes.Where(item => item.Edges.Contains(edge));
			foreach (var node in nodes)
			{
				node.Edges.Remove(edge);
			}

			base.RemoveItem(index);
		}

		protected override void ClearItems()
		{
			foreach (var node in Diagram.Nodes)
			{
				node.Edges.Clear();
			}

			base.ClearItems();
		}

		#endregion
	}
}
