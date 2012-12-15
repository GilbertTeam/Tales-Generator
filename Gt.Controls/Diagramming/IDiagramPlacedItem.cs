using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Gt.Controls.Diagramming
{
	public interface IDiagramPlacedItem
	{
		#region Props

		Point TopLeft { get; set; }

		Size Size { get; set; }

		Rect Rect { get; }

		UIElement UIElement { get; }

		#endregion
	}
}
