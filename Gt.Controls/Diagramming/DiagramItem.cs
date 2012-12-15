using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

namespace Gt.Controls.Diagramming
{
	public abstract class DiagramItem : DependencyObject
	{
		#region Fields

		#region WPF

		public static readonly DependencyProperty BackgroundProperty;

		public static readonly DependencyProperty BorderPenProperty;

		public static readonly DependencyProperty DataProperty;

		public static readonly DependencyProperty UserDataProperty;

		#endregion

		private Diagram _diagram;

		protected DiagramLabel _label;

		protected IDiagramItemDrawer _drawer;

		protected Geometry _geometry;

		protected DiagramSelectionBorder _border;

		#endregion

		#region Constructors

		static DiagramItem()
		{
			var backgroundMd = new PropertyMetadata(new SolidColorBrush(Colors.Transparent), OnBackgroundChanged);
			BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(DiagramItem), backgroundMd);

			var borderPenMd = new PropertyMetadata(new Pen(Brushes.Black, 1), OnBorderPenChanged);
			BorderPenProperty = DependencyProperty.Register("BorderPen", typeof(Pen), typeof(DiagramItem), borderPenMd);

			var dataMd = new PropertyMetadata(null, OnDataChanged);
			DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(DiagramItem), dataMd);

			var userDataMd = new PropertyMetadata(null, OnUserDataChanged);
			UserDataProperty = DependencyProperty.Register("UserData", typeof(object), typeof(DiagramItem), userDataMd);
		}

		internal DiagramItem(Diagram diagram)
		{
			_diagram = diagram;
			_label = null;
			_drawer = null;
			_geometry = null;
			_border = null;
		}

		#endregion

		#region Properties

		[Bindable(true)]
		public Brush Background
		{
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}

		[Bindable(true)]
		public Pen BorderPen
		{
			get { return (Pen)GetValue(BorderPenProperty); }
			set { SetValue(BorderPenProperty, value); }
		}

		[Bindable(true)]
		public object Data
		{
			get { return GetValue(DataProperty); }
			set { SetValue(DataProperty, value); }
		}

		[Bindable(true)]
		public string UserData
		{
			get { return (string)GetValue(UserDataProperty); }
			set { SetValue(UserDataProperty, value); }
		}

		public DiagramLabel Label
		{
			get { return _label; }
		}

		public Diagram Diagram
		{
			get { return _diagram; }
		}

		public IDiagramItemDrawer Drawer
		{
			get { return _drawer; }
			set { _drawer = value; }
		}

		internal DiagramSelectionBorder Border
		{
			get { return _border; }
		}

		public Geometry Geometry
		{
			get { return _geometry; }
		}

		#endregion

		#region Methods

		protected static void OnBackgroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnVisualPropertyChanged(obj);
		}

		protected static void OnBorderPenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnVisualPropertyChanged(obj);
		}

		protected static void OnDataChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			//Не влияет ни на расположение, ни на внешний вид
		}

		protected static void OnUserDataChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
		}

		protected static void OnVisualPropertyChanged(DependencyObject obj)
		{
			var item = obj as DiagramItem;
			if (item == null)
				return;

			if (item.Diagram != null)
				item.Diagram.InvalidateVisual();
		}

		protected static void OnLayoutPropertyChanged(DependencyObject obj)
		{
			var item = obj as DiagramItem;
			if (item == null)
				return;

			if (item.Diagram != null)
				item.Diagram.InvalidateDiagram();
		}

		protected virtual void SetDiagram(Diagram value)
		{
			_diagram = value;
		}

		public abstract void CalculateGeometry();

		protected void CalculateGeometry(IDiagramItemDrawer drawer)
		{
			if (drawer == null)
				return;

			_geometry = drawer.CalculateGeometry(this);
			_border = drawer.CalculateBorder(this);
		}

		public abstract void Draw(DrawingContext dc);

		//public abstract void DrawSelectionBorder(DrawingContext dc);

		public bool HitTest(Point hitPoint)
		{
			if (_geometry != null)
				return _geometry.HitTest(hitPoint, BorderPen);

			return false;
		}

		public void GetBoundaries(ref double topX, ref double topY, ref double bottomX, ref double bottomY)
		{
			Geometry itemGeometry = this.Geometry;
			if (itemGeometry == null)
				return;

			var bounds = itemGeometry.Bounds;

			if (topX > bounds.Left)
			{
				topX = bounds.Left;
			}

			if (topY > bounds.Top)
			{
				topY = bounds.Top;
			}

			if (bottomX < bounds.Right)
			{
				bottomX = bounds.Right;
			}

			if (bottomY < bounds.Bottom)
			{
				bottomY = bounds.Bottom;
			}

			if (_label != null)
			{
				_label.GetBoundaries(ref topX, ref topY, ref bottomX, ref bottomY);
			}
		}

		#endregion
	}
}
