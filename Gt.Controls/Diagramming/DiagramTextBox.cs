using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gt.Controls.Diagramming
{
	public class DiagramTextBox : TextBox, IDiagramPlacedItem
	{
		#region Props

		public Point TopLeft { get; set; }

		public Size Size { get; set; }

		public Rect Rect
		{
			get { return new Rect(TopLeft, Size); }
		}

		public new UIElement UIElement
		{
			get { return (this as UIElement); }
		}

		#endregion

		#region Methods

		protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Escape || e.Key == Key.Return)
			{
				UIElement parentElement = Parent as UIElement;
				if (parentElement != null)
				{
					parentElement.Focus();
					return;
				}
			}

			base.OnKeyUp(e);
		}

		#endregion
	}
}
