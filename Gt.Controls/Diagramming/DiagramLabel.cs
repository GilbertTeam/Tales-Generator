using System;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

using Gt.Controls.Diagramming.LabelDrawers;
using System.Windows.Controls;

namespace Gt.Controls.Diagramming
{
	public class DiagramLabel : DiagramItem
	{

		#region Fields

		#region WPF

		public static readonly DependencyProperty FontFamilyProperty;

		public static readonly DependencyProperty FontWeightProperty;

		public static readonly DependencyProperty FontSizeProperty;

		public static readonly DependencyProperty FontStyleProperty;

		public static readonly DependencyProperty FontStretchProperty;

		public static readonly DependencyProperty ForegroundProperty;

		public static readonly DependencyProperty RelativePositionProperty;

		public static readonly DependencyProperty TextProperty;

		#endregion

		private DiagramItem _owner;

		public event LabelEventHandler TextChanged;

		#endregion

		#region Constructors

		static DiagramLabel()
		{
			var fontFamilyMd = new PropertyMetadata(new FontFamily("Arial"), OnFontFamilyChanged);
			FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof (FontFamily), typeof (DiagramLabel), fontFamilyMd);

			var fontWeightMd = new PropertyMetadata(FontWeights.Normal, OnFontWeightChanged);
			FontWeightProperty = DependencyProperty.Register("FontWeight", typeof (FontWeight), typeof (DiagramLabel), fontWeightMd);

			const double defaultSize = 11;
			var fontSizeMd = new PropertyMetadata(defaultSize, OnFontSizeChanged);
			FontSizeProperty = DependencyProperty.Register("FontSize", typeof (double), typeof (DiagramLabel), fontSizeMd);

			var fontStyleMd = new PropertyMetadata(FontStyles.Normal, OnFontStyleChanged);
			FontStyleProperty = DependencyProperty.Register("FontStyle", typeof (FontStyle), typeof (DiagramLabel), fontStyleMd);

			var fontStretchMd = new PropertyMetadata(FontStretches.Normal, OnFontStretchChanged);
			FontStretchProperty = DependencyProperty.Register("FontStretch", typeof (FontStretch), typeof (DiagramLabel),
			                                                  fontStretchMd);

			var foregroundMd = new PropertyMetadata(new SolidColorBrush(Colors.Black), OnForegroundChanged);
			ForegroundProperty = DependencyProperty.Register("Foreground", typeof (Brush), typeof (DiagramLabel), foregroundMd);

			var relativePosMd = new PropertyMetadata(new Point(0, 0), OnRelativePositionChanged);
			RelativePositionProperty = DependencyProperty.Register("RelativePosition", typeof (Point), typeof (DiagramLabel), relativePosMd);

			var textMd = new PropertyMetadata("", OnTextChanged);
			TextProperty = DependencyProperty.Register("Text", typeof (String), typeof (DiagramLabel), textMd);
		}

		public DiagramLabel(Diagram diagram, DiagramItem owner)
			: base(diagram)
		{
			Drawer = new BaseLabelDrawer();

			_owner = owner;

			BorderPen = new Pen(new SolidColorBrush(Colors.Transparent), 0);
			AllowInPlaceEdit = true;
		}

		#endregion

		#region Properties

		[Bindable(true)]
		public FontFamily FontFamily
		{
			get { return (FontFamily) GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}

		[Bindable(true)]
		public FontWeight FontWeight
		{
			get { return (FontWeight) GetValue(FontWeightProperty); }
			set { SetValue(FontWeightProperty, value); }
		}

		[Bindable(true)]
		public double FontSize
		{
			get { return (double) GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		[Bindable(true)]
		public FontStyle FontStyle
		{
			get { return (FontStyle) GetValue(FontStyleProperty); }
			set { SetValue(FontStyleProperty, value); }
		}

		[Bindable(true)]
		public FontStretch FontStretch
		{
			get { return (FontStretch) GetValue(FontStretchProperty); }
			set { SetValue(FontStretchProperty, value); }
		}

		[Bindable(true)]
		public Brush Foreground
		{
			get { return (Brush) GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}

		[Bindable(true)]
		public Point RelativePosition
		{
			get { return (Point) GetValue(RelativePositionProperty); }
			set { SetValue(RelativePositionProperty, value); }
		}

		[Bindable(true)]
		public string Text
		{
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public DiagramItem Owner
		{
			get { return _owner; }
		}

		public bool AllowInPlaceEdit { get; set; }

		#endregion

		#region Methods

		protected static void OnFontFamilyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnLayoutPropertyChanged(obj);
		}

		protected static void OnFontWeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnLayoutPropertyChanged(obj);
		}

		protected static void OnFontStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnLayoutPropertyChanged(obj);
		}

		protected static void OnFontStretchChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnLayoutPropertyChanged(obj);
		}

		protected static void OnFontSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnLayoutPropertyChanged(obj);
		}

		protected static void OnForegroundChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnLayoutPropertyChanged(obj);
		}

		protected static void OnRelativePositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			OnLayoutPropertyChanged(obj);
		}

		protected static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var label = obj as DiagramLabel;
			if (label != null)
			{
				label.RaiseTextChanged();
			}
			OnLayoutPropertyChanged(obj);
		}

		public override void  CalculateGeometry()
		{
			if (Drawer != null)
				CalculateGeometry(Drawer);
		}

		public override void Draw(DrawingContext dc)
		{
			if (Drawer != null)
				Drawer.Draw(dc, new Rect(), this);
		}

		public void BeginEdit()
		{
			if (!AllowInPlaceEdit)
				return;

			DiagramTextBox textBox = new DiagramTextBox();

			//ScaleTransform scaleTransform = new ScaleTransform(Diagram.Scale, Diagram.Scale);
			//textBox.RenderTransform = scaleTransform;

			textBox.FontFamily = FontFamily;
			textBox.FontSize = FontSize * Diagram.Scale;
			textBox.FontStretch = FontStretch;
			textBox.FontStyle = FontStyle;
			textBox.FontWeight = FontWeight;

			Geometry thisGeometry = Geometry;
			if (thisGeometry.Bounds.Size.Width == 0 || thisGeometry.Bounds.Size.Height == 0)
			{
				thisGeometry = Owner.Geometry;
			}
			textBox.TopLeft = thisGeometry.Bounds.TopLeft.ToDisplayPoint(Diagram.Offset, Diagram.Scale);
			Size size = thisGeometry.Bounds.Size;
			textBox.Size = new Size(size.Width * Diagram.Scale, size.Height * Diagram.Scale + 5 > GlobalData.MinTextBoxHeight ?
				size.Height * Diagram.Scale : GlobalData.MinTextBoxHeight);

			textBox.MinHeight = textBox.Size.Height;
			textBox.MinWidth = textBox.Size.Width;
			//textBox.MaxHeight = textBox.Size.Height;
			//textBox.MaxWidth = textBox.Size.Width;

			textBox.Text = this.Text;

			textBox.LostFocus += new RoutedEventHandler(EditLostFocus);

			Diagram.PlacedItems.Add(textBox);
		}

		void EditLostFocus(object sender, RoutedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				this.Text = textBox.Text;
			}

			Diagram.PlacedItems.Remove(sender as IDiagramPlacedItem);
		}

		//public override void DrawSelectionBorder(DrawingContext dc)
		//{
		//    if (Drawer != null)
		//        Drawer.DrawSelectionBorder(dc, this);
		//}

		void RaiseTextChanged()
		{
			if (TextChanged != null)
			{
				LabelEventArgs args = new LabelEventArgs(this.Diagram, this);
				TextChanged(args);
			}
		}

		#endregion
	}
}
