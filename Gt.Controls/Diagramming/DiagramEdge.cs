using System.Windows;
using System.Windows.Media;

namespace Gt.Controls.Diagramming
{
	public class DiagramEdge : DiagramItem
	{

		#region Fields

		#region WPF

		//public static readonly DependencyProperty SourceNodeProperty;

		//public static readonly DependencyProperty DestinationNodeProperty;

		public static readonly DependencyProperty SourcePointProperty;

		public static readonly DependencyProperty DestinationPointProperty;

		public static readonly DependencyProperty AnchoringModeProperty;

		#endregion

		protected DiagramNode _sourceNode;

		protected DiagramNode _destinationNode;

		public event EdgeEventHandler SourceNodeChanged;

		public event EdgeEventHandler DestinationNodeChanged;

		public event EdgeEventHandler AnchoringModeChanged;

		#endregion

		#region Constructors

		static DiagramEdge()
		{
			var sourcePointMd = new PropertyMetadata(new Point(0, 0), OnSourcePointChanged);
			SourcePointProperty = DependencyProperty.Register("SourcePoint", typeof (Point), typeof (DiagramEdge), sourcePointMd);

			var destinationPointMd = new PropertyMetadata(new Point(0, 0), OnDestinationPointChanged);
			DestinationPointProperty = DependencyProperty.Register("DestinationPoint", typeof (Point), typeof (DiagramEdge), destinationPointMd);

			var anchoringModeMd = new PropertyMetadata(EdgeAnchoringMode.PointToPoint, OnAnchoringModeChanged);
			AnchoringModeProperty = DependencyProperty.Register("AnchoringMode", typeof (EdgeAnchoringMode), typeof (DiagramEdge), anchoringModeMd);
		}

		public DiagramEdge(Diagram diagram)
			: base(diagram)
		{
			_sourceNode = null;
			_destinationNode = null;

			_label = new DiagramLabel(diagram, this);
			_label.NeedRecalc = true;

			if (!diagram.Edges.Contains(this))
				diagram.Edges.Add(this);
		}

		#endregion

		#region Properties

		public DiagramNode SourceNode
		{
			get { return _sourceNode; }
			set
			{
				if (_sourceNode != value)
				{
					var oldSourceNode = _sourceNode;

					_sourceNode = value;

					OnSourceNodeChanged(oldSourceNode, _sourceNode);
				}
			}
			//get { return (DiagramNode)GetValue(SourceNodeProperty); }
			//set { SetValue(SourceNodeProperty, value); }
		}

		public DiagramNode DestinationNode
		{
			get { return _destinationNode; }
			set
			{
				if (_destinationNode != value)
				{
					var oldDestinationNode = _destinationNode;

					_destinationNode = value;

					OnDesinationNodeChanged(oldDestinationNode, _destinationNode);
				}
			}
			//get { return (DiagramNode)GetValue(DestinationNodeProperty); }
			//set { SetValue(DestinationNodeProperty, value); }
		}

		public Point SourcePoint
		{
			get { return (Point) GetValue(SourcePointProperty); }
			set { SetValue(SourcePointProperty, value); }
		}

		public Point DestinationPoint
		{
			get { return (Point) GetValue(DestinationPointProperty); }
			set { SetValue(DestinationPointProperty, value); }
		}

		public EdgeAnchoringMode AnchoringMode
		{
			get { return (EdgeAnchoringMode) GetValue(AnchoringModeProperty); }
			set { SetValue(AnchoringModeProperty, value); }
		}

		#endregion

		#region Methods

		private void OnSourceNodeChanged(DiagramNode oldSourceNode, DiagramNode newSourceNode)
		{
			var edge = this;

			if (oldSourceNode != null)
			{
				oldSourceNode.Edges.Remove(edge);
			}

			if (newSourceNode != null)
			{
				if (!newSourceNode.Edges.Contains(edge))
					newSourceNode.Edges.Add(edge);
			}

			if (AnchoringMode == EdgeAnchoringMode.NodeToPoint || AnchoringMode == EdgeAnchoringMode.NodeToNode)
				OnLayoutPropertyChanged(this);

			RaiseSourceNodeChanged();
		}

		private void OnDesinationNodeChanged(DiagramNode oldDestinationNode, DiagramNode newDestinationNode)
		{
			var edge = this;

			if (oldDestinationNode != null)
			{
				oldDestinationNode.Edges.Remove(edge);
			}

			if (newDestinationNode != null)
			{
				if (!newDestinationNode.Edges.Contains(edge))
					newDestinationNode.Edges.Add(edge);
			}

			if (AnchoringMode == EdgeAnchoringMode.PointToNode || AnchoringMode == EdgeAnchoringMode.NodeToNode)
				OnLayoutPropertyChanged(this);

			RaiseDestinationNodeChanged();
		}

		private static void OnSourcePointChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var edge  = obj as DiagramEdge;
			if (edge != null && (edge.AnchoringMode == EdgeAnchoringMode.PointToNode || edge.AnchoringMode == EdgeAnchoringMode.PointToPoint))
				OnLayoutPropertyChanged(obj);
		}

		private static void OnDestinationPointChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var edge = obj as DiagramEdge;
			if (edge != null && (edge.AnchoringMode == EdgeAnchoringMode.NodeToPoint || edge.AnchoringMode == EdgeAnchoringMode.PointToPoint))
				OnLayoutPropertyChanged(obj);
		}

		private static void OnAnchoringModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var edge = obj as DiagramEdge;
			if (edge != null)
			{
				switch (edge.AnchoringMode)
				{
					case EdgeAnchoringMode.PointToNode:
						edge.SourceNode = null;
						break;
					case EdgeAnchoringMode.PointToPoint:
						edge.SourceNode = null;
						edge.DestinationNode = null;
						break;
					case EdgeAnchoringMode.NodeToPoint:
						edge.DestinationNode = null;
						break;
				}
			}

			OnLayoutPropertyChanged(obj);

			edge.RaiseAnchoringModeChanged();
		}

		public override void  CalculateGeometry()
		{
			if (Drawer != null)
				CalculateGeometry(Drawer);
			else
			{
				var diagramDrawer = Diagram.DefaultEdgeDrawer;
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
				var diagramDrawer = Diagram.DefaultEdgeDrawer;
				if (diagramDrawer != null)
					diagramDrawer.Draw(dc, new Rect(), this);
			}
		}

		void RaiseSourceNodeChanged()
		{
			if (SourceNodeChanged != null)
			{
				EdgeEventArgs args = new EdgeEventArgs(Diagram, this);
				SourceNodeChanged(args);
			}
		}

		void RaiseDestinationNodeChanged()
		{
			if (DestinationNodeChanged != null)
			{
				EdgeEventArgs args = new EdgeEventArgs(Diagram, this);
				DestinationNodeChanged(args);
			}
		}

		void RaiseAnchoringModeChanged()
		{
			if (AnchoringModeChanged != null)
			{
				EdgeEventArgs args = new EdgeEventArgs(Diagram, this);
				AnchoringModeChanged(args);
			}
		}

		//public override void DrawSelectionBorder(DrawingContext dc)
		//{
		//    if (Drawer != null)
		//        Drawer.DrawSelectionBorder(dc, new Rect(), this);
		//    else
		//    {
		//        var diagramDrawer = Diagram.DefaultEdgeDrawer;
		//        if (diagramDrawer != null)
		//            diagramDrawer.DrawSelectionBorder(dc, new Rect(), this);
		//    }
		//}

		#endregion
	}
}
