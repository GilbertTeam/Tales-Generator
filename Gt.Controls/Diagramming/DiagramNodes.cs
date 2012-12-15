using System.Linq;

namespace Gt.Controls.Diagramming
{
	public class DiagramNodes : DiagramItemCollection<DiagramNode>
	{
		#region Constructors

		internal DiagramNodes(Diagram diagram)
			: base(diagram)
		{
		}

		#endregion

		#region Methods

		protected override void SetItem(int index, DiagramNode item)
		{
			base.SetItem(index, item);
		}

		protected override void InsertItem(int index, DiagramNode item)
		{
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			var node = this[index];
			node.Edges.Clear();

			for (int i = 0; i < Diagram.Edges.Count; i++)
			{
				var edge = Diagram.Edges[i];

				if (edge.SourceNode == node || edge.DestinationNode == node)
				{
					if (edge.SourceNode != null && edge.SourceNode == node)
						edge.SourceNode = null;
					if (edge.DestinationNode != null && edge.DestinationNode == node)
						edge.DestinationNode = null;
				}
			}

			base.RemoveItem(index);
		}

		protected override void ClearItems()
		{
			base.ClearItems();
		}

		#endregion
	}
}
