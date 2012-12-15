using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace Gt.Controls.Diagramming
{
	public abstract class DiagramConstrainedItem : DiagramItem
	{
#region Fields

#region WPF

		public static readonly DependencyProperty BoundsProperty;

#endregion

#endregion

#region Constructors

		static DiagramConstrainedItem()
		{
			PropertyMetadata BoundsMd = new PropertyMetadata(new Rect(0, 0, 0, 0));
			BoundsProperty = DependencyProperty.Register("Bounds", typeof(Rect), typeof(DiagramConstrainedItem), BoundsMd);
		}

		internal DiagramConstrainedItem(Diagram diagram) : base(diagram)
		{
		}

#endregion

#region Properties

		[Bindable(true)]
		public Rect Bounds
		{
			get { return (Rect)GetValue(BoundsProperty); }
			set { SetValue(BoundsProperty, value); }
		}

#endregion

#region Methods
#endregion
	}
}
