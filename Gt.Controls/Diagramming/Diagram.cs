﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;
using System.IO;

using Gt.Controls.Diagramming.EdgeDrawers;
using Gt.Controls.Diagramming.NodeDrawers;

using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace Gt.Controls.Diagramming
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Gt.Controls.Diagramming"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Gt.Controls.Diagramming;assembly=Gt.Controls.Diagramming"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <MyNamespace:Diagram/>
	///
	/// </summary>
	public class Diagram : Control
	{
		#region Fields

		#region wpf

		public static readonly DependencyProperty XViewOffsetProperty;

		public static readonly DependencyProperty YViewOffsetProperty;

		public static readonly DependencyProperty ScaleProperty;

		public static readonly DependencyProperty ScaleFactorProperty;

		#endregion wpf

		private DiagramNodes _nodes;

		private DiagramEdges _edges;

		private DiagramItemCollection<DiagramItem> _selection;

		private DiagramItemCollection<DiagramItem> _itemsInDrawingOrder;

		private DiagramMouseManager _mouseManager;

		protected Rect? _viewport;

		protected Rect? _boundaries;

		protected IDiagramItemDrawer _defaultNodeDrawer;

		protected IDiagramItemDrawer _defaultEdgeDrawer;

		protected ObservableCollection<IDiagramPlacedItem> _placedItems;

		public event DiagramNotifyDelegate DiagramRecalc;

		public event DiagramNotifyDelegate DiagramRender;

		#endregion

		#region Constructors

		static Diagram()
		{
			var xViewOffsetMd = new PropertyMetadata(0.0, OnXViewOffsetChanged);
			XViewOffsetProperty = DependencyProperty.Register("XViewOffset", typeof(double), typeof(Diagram), xViewOffsetMd);

			var yViewOffsetMd = new PropertyMetadata(0.0, OnYViewOffsetChanged);
			YViewOffsetProperty = DependencyProperty.Register("YViewOffset", typeof(double), typeof(Diagram), yViewOffsetMd);

			var scaleMd = new PropertyMetadata(1.0, OnZoomChanged);
			ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(Diagram), scaleMd);

			var scaleFactorMd = new PropertyMetadata(1.5);
			ScaleFactorProperty = DependencyProperty.Register("ScaleFactor", typeof(double), typeof(Diagram), scaleFactorMd);
		}

		public Diagram()
		{
			_nodes = new DiagramNodes(this);
			_edges = new DiagramEdges(this);
			_selection = new DiagramItemCollection<DiagramItem>(this);

			_itemsInDrawingOrder = new DiagramItemCollection<DiagramItem>(this);

			_nodes.CollectionChanged += NodeCollectionChanged;
			_edges.CollectionChanged += EdgeCollectionChanged;
			_selection.CollectionChanged += SelectionChanged;

			Background = new SolidColorBrush(Colors.Transparent);

			_defaultNodeDrawer = new RectangleNodeDrawer();
			_defaultEdgeDrawer = new LineEdgeDrawer();
			//DefaultLabelDrawer = new BaseLabelDrawer();

			_placedItems = new ObservableCollection<IDiagramPlacedItem>();
			_placedItems.CollectionChanged += new NotifyCollectionChangedEventHandler(CustomChilren_CollectionChanged);

			_mouseManager = new DiagramMouseManager(this);

			_mouseManager.LabelLButtonDblClick += new LabelEventHandler(OnLabelLButtonDblClick);
			_mouseManager.NodeLButtonDblClick += new NodeEventHandler(OnNodeLButtonDblClick);
			_mouseManager.EdgeLButtonDblClick += new EdgeEventHandler(OnEdgeLButtonDblClick);

			_viewport = null;
			_boundaries = null;

			LockRender = false;
			LockRecalc = false;
		}

		#endregion

		#region Properties

		[Bindable(true)]
		public double XViewOffset
		{
			get { return (double)GetValue(XViewOffsetProperty); }
			set { SetValue(XViewOffsetProperty, value); }
		}

		[Bindable(true)]
		public double YViewOffset
		{
			get { return (double)GetValue(YViewOffsetProperty); }
			set { SetValue(YViewOffsetProperty, value); }
		}

		[Bindable(true)]
		public double Scale
		{
			get { return (double)GetValue(ScaleProperty); }
			set { SetValue(ScaleProperty, value); }
		}

		[Bindable(true)]
		public double ScaleFactor
		{
			get { return (double)GetValue(ScaleFactorProperty); }
			set { SetValue(ScaleFactorProperty, value); }
		}


		public DiagramNodes Nodes
		{
			get { return _nodes; }
		}

		public DiagramEdges Edges
		{
			get { return _edges; }
		}

		public DiagramItemCollection<DiagramItem> Selection
		{
			get { return _selection; }
		}

		public IDiagramItemDrawer DefaultNodeDrawer
		{
			get { return _defaultNodeDrawer; }
			set
			{
				_defaultNodeDrawer = value;

				InvalidateDiagram();
			}
		}

		public IDiagramItemDrawer DefaultEdgeDrawer
		{
			get { return _defaultEdgeDrawer; }
			set
			{
				_defaultEdgeDrawer = value;

				InvalidateDiagram();
			}
		}

		internal bool LockRender { get; set; }

		internal bool LockRecalc { get; set; }

		internal DiagramItemCollection<DiagramItem> ItemsInDrawingOrder
		{
			get { return _itemsInDrawingOrder; }
		}

		internal DiagramMouseManager MouseManager
		{
			get { return _mouseManager; }
		}

		public Rect Viewport
		{
			get
			{
				if (!_viewport.HasValue)
					CalculateViewport();
				return _viewport.Value;
			}
		}

		public Rect Boundaries
		{
			get
			{
				CalculateBoundaries();
				return _boundaries.Value;
			}
		}

		public Vector Offset
		{
			get
			{
				return new Vector(XViewOffset, YViewOffset);
			}
		}

		public ObservableCollection<IDiagramPlacedItem> PlacedItems
		{
			get { return _placedItems; }
		}

		protected override int VisualChildrenCount
		{
			get
			{
				return _placedItems.Count;
			}
		}

		#endregion

		#region Methods

		#region Wpf Handlers

		protected static void OnXViewOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var diagram = obj as Diagram;
			if (diagram == null)
				return;

			diagram.InvalidateVisual();
		}

		protected static void OnYViewOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var diagram = obj as Diagram;
			if (diagram == null)
				return;

			diagram.InvalidateVisual();
		}

		protected static void OnZoomChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var diagram = obj as Diagram;
			if (diagram == null)
				return;

			diagram.InvalidateVisual();
		}

		#endregion

		#region Collection's Handlers

		private void EdgeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			InvalidateDiagram();
		}

		private void NodeCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			InvalidateDiagram();
		}

		private void SelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			InvalidateDiagram();
		}

		#endregion

		#region Обработчики событий диаграммы

		void OnNodeLButtonDblClick(NodeEventArgs args)
		{
			var node = args.Node;
			node.Label.BeginEdit();
		}

		void OnEdgeLButtonDblClick(EdgeEventArgs args)
		{
			var edge = args.Edge;
			edge.Label.BeginEdit();
		}

		void OnLabelLButtonDblClick(LabelEventArgs args)
		{
			var label = args.Label;
			label.BeginEdit();
		}

		#endregion

		#region Кастомные потомки

		protected override Visual GetVisualChild(int index)
		{
			return _placedItems[index].UIElement;
		}

		void CustomChilren_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (UIElement element in e.NewItems)
				{
					AddLogicalChild(element);
					AddVisualChild(element);
					element.Focus();
				}
			}

			if (e.OldItems != null)
			{
				foreach (UIElement element in e.OldItems)
				{
					RemoveLogicalChild(element);
					RemoveVisualChild(element);
				}
			}

			InvalidateVisual();
		}

		#endregion

		#region Обновление диаграммы

		public void InvalidateDiagram()
		{
			InvalidateShapes();
			InvalidateMeasure();
			InvalidateArrange();
			InvalidateVisual();
		}

		#region Measure

		protected override Size MeasureOverride(Size constraint)
		{
			Size desiredSize = new Size();

			for (int i = 0, count = _placedItems.Count; i < count; i++)
			{
				UIElement element = _placedItems[i].UIElement;
				element.Measure(constraint);
				desiredSize = element.DesiredSize;
			}

			return desiredSize;
		}

		#endregion

		#region Arrange

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			for (int i = 0, count = _placedItems.Count; i < count; i++)
			{
				IDiagramPlacedItem item = _placedItems[i];
				UIElement itemElement = _placedItems[i].UIElement;
				Size desiredSize = itemElement.DesiredSize;
				itemElement.Arrange(new Rect(item.TopLeft, (desiredSize.Width >= item.Size.Width &&
					desiredSize.Height >= item.Size.Height) ? desiredSize : item.Size));
			}

			return arrangeBounds;
		}

		#endregion

		#region Invalidating size

		public void InvalidateShapes()
		{
			if (LockRecalc)
				return;

			OnCalculateShape();

			if (DiagramRecalc != null)
			{
				DiagramRecalc();
			}
		}

		protected virtual void OnCalculateShape()
		{
			foreach (var node in Nodes)
			{
				node.CalculateGeometry();
				if (node.Label != null)
					node.Label.CalculateGeometry();
			}

			foreach (var edge in Edges)
			{
				edge.CalculateGeometry();
				if (edge.Label != null)
					edge.Label.CalculateGeometry();
			}
		}

		#endregion

		#region Render

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (LockRender)
				return;

			var rect = new Rect(0, 0, ActualWidth, ActualHeight);
			drawingContext.DrawRectangle(Background, null, rect);

			CalculateViewport();
			CalculateBoundaries();

			ScaleTransform scaleTransform = new ScaleTransform(Scale, Scale);
			TranslateTransform translateTransform = new TranslateTransform(XViewOffset, YViewOffset);

			drawingContext.PushTransform(scaleTransform);
			drawingContext.PushTransform(translateTransform);

			CalculateViewport();
			DrawItems(drawingContext);

			drawingContext.Pop();
			drawingContext.Pop();

			base.OnRender(drawingContext);

			if (DiagramRender != null)
				DiagramRender();
		}

		internal void DrawItems(DrawingContext drawingContext)
		{
			_itemsInDrawingOrder.Clear();

			var itemsToDraw = new List<DiagramItem>();

			foreach (var node in _nodes)
			{
				DrawNode(drawingContext, node, ref itemsToDraw);
			}

			foreach (var edge in _edges)
			{
				DrawEdge(drawingContext, edge, ref itemsToDraw);
			}

			foreach (var item in itemsToDraw)
			{
				item.Draw(drawingContext);
				//item.DrawSelectionBorder(drawingContext);

				_itemsInDrawingOrder.Add(item);
				if (item as DiagramLabel == null)
				{
					if (item.Label != null)
					{
						item.Label.Draw(drawingContext);
						_itemsInDrawingOrder.Add(item.Label);
					}
				}
			}
		}

		private void DrawNode(DrawingContext dc, DiagramNode node, ref List<DiagramItem> itemsToDraw)
		{
			if (_itemsInDrawingOrder.Contains(node) || itemsToDraw.Contains(node))
				return;

			if (_selection.Contains(node))
			{
				//node.Label.Draw(dc);
				if (!itemsToDraw.Contains(node))
					itemsToDraw.Add(node);
			}
			else
			{
				node.Draw(dc);
				_itemsInDrawingOrder.Add(node);

				if (_selection.Contains(node.Label))
				{
					if (!itemsToDraw.Contains(node.Label))
						itemsToDraw.Add(node.Label);
				}
				else
				{
					if (node.Label != null)
					{
						node.Label.Draw(dc);
						_itemsInDrawingOrder.Add(node.Label);
					}
				}
			}


			foreach (var edge in node.Edges)
			{
				DrawEdge(dc, edge, ref itemsToDraw);

				if (node == edge.SourceNode && edge.DestinationNode != null)
					DrawNode(dc, edge.DestinationNode, ref itemsToDraw);

				if (node == edge.DestinationNode && edge.SourceNode != null)
					DrawNode(dc, edge.SourceNode, ref itemsToDraw);
			}

		}

		private void DrawEdge(DrawingContext dc, DiagramEdge edge, ref List<DiagramItem> itemsToDraw)
		{
			if (_itemsInDrawingOrder.Contains(edge) || itemsToDraw.Contains(edge))
				return;

			if (_selection.Contains(edge))
			{
				if (!itemsToDraw.Contains(edge))
					itemsToDraw.Add(edge);
			}
			else
			{
				edge.Draw(dc);
				_itemsInDrawingOrder.Add(edge);

				if (_selection.Contains(edge.Label))
				{
					if (!itemsToDraw.Contains(edge.Label))
						itemsToDraw.Add(edge.Label);
				}
				else if (edge.Label != null)
				{
					edge.Label.Draw(dc);
					_itemsInDrawingOrder.Add(edge.Label);
					
				}
			}
		}

		#endregion

		#endregion

		#region Mouse

		protected bool IsInPlacedItem(Point point)
		{
			bool result = false;

			foreach (IDiagramPlacedItem item in _placedItems)
			{
				if (item.Rect.Contains(point))
				{
					result = true;
					break;
				}
			}

			return result;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			Point point = e.GetPosition(this);
			if (!IsInPlacedItem(point))
				this.Focus();

			_mouseManager.ProcessLeftButtonDown();

			e.Handled = true;
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			_mouseManager.ProcessLeftButtonUp();

			e.Handled = true;
		}

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			Point point = e.GetPosition(this);
			if (!IsInPlacedItem(point))
				this.Focus();

			_mouseManager.ProcessRightButtonDown();

			e.Handled = true;
		}

		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
		{
			_mouseManager.ProcessRightButtonUp();

			e.Handled = true;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			Point point = e.GetPosition(this);
			_mouseManager.ProcessMouseMove(point.ToViewPoint(Offset, Scale));

			e.Handled = true;
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			Point point = e.GetPosition(this);
			if (!IsInPlacedItem(point))
				this.Focus();

			_mouseManager.ProcessMouseWheel(point.ToViewPoint(Offset, Scale), e.Delta);

			e.Handled = true;
		}

		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			Point point = e.GetPosition(this);
			if (!IsInPlacedItem(point))
				this.Focus();

			_mouseManager.ProcessMouseDoubleClick(e.ChangedButton == MouseButton.Left, e.ChangedButton == MouseButton.Right);

			e.Handled = true;
		}

		#endregion

		#region Export

		public MemoryStream RenderToImage(float dpiX, float dpiY, Rect rect, BitmapEncoder encoder)
		{
			double qualityCoefficient = Math.Floor(4096.0/(Math.Max(rect.Width, rect.Height)*dpiX/25.4));
			if (Math.Abs(qualityCoefficient - 0) < GlobalData.Precision)
				qualityCoefficient = 4096.0/(Math.Max(rect.Width, rect.Height)*dpiX/25.4);

			var renderTargetBitmap = new RenderTargetBitmap((int)(rect.Width * dpiX / 25.4), (int)(rect.Height * dpiY / 25.4), dpiX * qualityCoefficient, dpiY * qualityCoefficient, PixelFormats.Pbgra32);

			renderTargetBitmap.Render(this);
			encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

			var stream = new MemoryStream();
			encoder.Save(stream);

			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}


		public void SaveToXml()
		{
		}

		public void LoadFromXml()
		{

		}

		#endregion

		#region Зумы

		public void ZoomIn()
		{
			Scale = Scale * ScaleFactor;
		}

		public void ZoomIn(Point point)
		{
			using (var diagramLock = new DiagramRenderLock(this))
			{
				ZoomIn();

				DoOffsetAfterZoom(point);
			}
		}

		public void ZoomOut()
		{
			Scale = Scale / ScaleFactor;
		}

		public void ZoomOut(Point point)
		{
			using (var diagramLock = new DiagramRenderLock(this))
			{
				double oldScale = Scale;
				Rect oldViewport = Viewport;

				ZoomOut();

				DoOffsetAfterZoom(point);
			}
		}

		protected void DoOffsetAfterZoom(Point point)
		{
			CalculateViewport();
			Rect newViewport = Viewport;
			Rect rightNewViewport = new Rect(point.X - newViewport.Width / 2, point.Y - newViewport.Height / 2, newViewport.Width,  newViewport.Height);
			XViewOffset = -rightNewViewport.TopLeft.X;
			YViewOffset = -rightNewViewport.TopLeft.Y;
		}

		#endregion Зумы

		#region Inner methods

		public void CalculateViewport()
		{
			Rect actualView = new Rect(0, 0, this.ActualWidth, this.ActualHeight);

			actualView.Scale(1 / Scale, 1 / Scale);
			actualView.Offset(-this.XViewOffset, -this.YViewOffset);

			_viewport = actualView;
		}

		public void CalculateBoundaries()
		{
			double topX = double.MaxValue;
			double topY = double.MaxValue;
			double bottomX = double.MinValue;
			double bottomY = double.MinValue;

			foreach (var node in _nodes)
			{
				node.GetBoundaries(ref topX, ref topY, ref bottomX, ref bottomY);
			}

			foreach (var edge in _edges)
			{
				edge.GetBoundaries(ref topX, ref topY, ref bottomX, ref bottomY);
			}

			_boundaries = new Rect(new Point(topX, topY), new Point(bottomX, bottomY));
		}

		#endregion

		#region Всякие

		public void ClearAll()
		{
			var diagramLock = new DiagramUpdateLock(this);
			_nodes.Clear();
			_edges.Clear();
			_selection.Clear();
		}

		#endregion

		#endregion
	}
}
