using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Collections.Generic;

namespace Gt.Controls.Diagramming
{
	public class DiagramNode : DiagramItem
	{
		#region Fields

		#region WPF

		public static readonly DependencyProperty BoundsProperty;

		public static readonly DependencyProperty RadiusXProperty;

		public static readonly DependencyProperty RadiusYProperty;

		#endregion

		private DiagramItemCollection<DiagramEdge> _edges;

		#endregion

		#region Costructors

		static DiagramNode()
		{
			var boundsMd = new PropertyMetadata(new Rect(0, 0, 0, 0), OnBoundsChanged);
			BoundsProperty = DependencyProperty.Register("Bounds", typeof(Rect), typeof(DiagramNode), boundsMd);

			var radiusXMd = new PropertyMetadata(0.0, OnRadiusXChanged);
			RadiusXProperty = DependencyProperty.Register("RadiusX", typeof(double), typeof(DiagramNode), radiusXMd);

			var radiusYMd = new PropertyMetadata(0.0, OnRadiusYChanged);
			RadiusYProperty = DependencyProperty.Register("RadiusY", typeof(double), typeof(DiagramNode), radiusYMd);
		}

		public DiagramNode(Diagram diagram)
			: base(diagram)
		{
			_label = new DiagramLabel(diagram, this);

			_edges = new DiagramItemCollection<DiagramEdge>(diagram);

			IsUnderCursor = false;

			if (!diagram.Nodes.Contains(this))
				diagram.Nodes.Add(this);
		}

		#endregion

		#region Properties

		[Bindable(true)]
		public Rect Bounds
		{
			get { return (Rect)GetValue(BoundsProperty); }
			set { SetValue(BoundsProperty, value); }
		}

		[Bindable(true)]
		public double RadiusX
		{
			get { return (double)GetValue(RadiusXProperty); }
			set { SetValue(RadiusXProperty, value); }
		}

		[Bindable(true)]
		public double RadiusY
		{
			get { return (double)GetValue(RadiusYProperty); }
			set { SetValue(RadiusYProperty, value); }
		}

		internal bool IsUnderCursor { get; set; }

		public DiagramItemCollection<DiagramEdge> Edges
		{
			get { return _edges; }
		}

		#endregion

		#region Methods

		protected static void OnBoundsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnLayoutPropertyChanged(obj);
		}

		protected static void OnRadiusXChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnLayoutPropertyChanged(obj);
		}

		protected static void OnRadiusYChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnLayoutPropertyChanged(obj);
		}

		public override void CalculateGeometry()
		{
			if (Drawer != null)
				CalculateGeometry(Drawer);
			else
			{
				var diagramDrawer = Diagram.DefaultNodeDrawer;
				if (diagramDrawer != null)
					CalculateGeometry(diagramDrawer);
			}
		}

		public override void Draw(DrawingContext dc)
		{
			if (Drawer != null)
				Drawer.Draw(dc, new Rect(), this);
			else
			{
				var diagramDrawer = Diagram.DefaultNodeDrawer;
				if (diagramDrawer != null)
					diagramDrawer.Draw(dc, new Rect(), this);
			}
		}

		//public override void DrawSelectionBorder(DrawingContext dc)
		//{
		//    if (Drawer != null)
		//        Drawer.DrawSelectionBorder(dc, this);
		//    else
		//    {
		//        var diagramDrawer = Diagram.DefaultNodeDrawer;
		//        if (diagramDrawer != null)
		//            diagramDrawer.DrawSelectionBorder(dc, this);
		//    }
		//}

		#endregion
	}
}
