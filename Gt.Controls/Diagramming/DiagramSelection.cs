using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gt.Controls.Diagramming
{
	public class DiagramSelection : DiagramItemCollection<DiagramItem>
	{

		public DiagramSelection(Diagram diagram)
			: base(diagram)
		{
		}

		protected override void InsertItem(int index, DiagramItem item)
		{
			if (item != null && item.IsSelectable == false)
				return;

			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, DiagramItem item)
		{
			if (item != null && item.IsSelectable == false)
				return;

			base.SetItem(index, item);
		}
	}
}
