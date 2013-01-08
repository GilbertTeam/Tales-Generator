using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System;
using System.Diagnostics;
using System.Windows.Media;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Gt.Controls.Diagramming
{
	internal class DiagramMouseManager
	{
		#region Fields

		protected bool _leftButtonDown;

		protected bool _rightButtonDown;

		protected Diagram _diagram;

		protected MouseAction _leftButtonAction;

		protected MouseAction _rightButtonAction;

		protected ResizeDirection _currentResizeDirection;

		protected DiagramItem _itemUnderCursor;

		protected DiagramItem _mouseDownOnItem;

		protected Point? _currentMousePosition;

		protected Point? _leftButtonDownMousePosition;

		protected Point? _rightButtonDownMousePosition;

		protected double _baseXViewOffset;

		protected double _baseYViewOffset;

		protected BackgroundWorker _offsetWorker;

		#region events

		public event NodeEventHandler NodeLButtonClick;
		public event NodeEventHandler NodeLButtonDblClick;
		public event EdgeEventHandler EdgeLButtonClick;
		public event EdgeEventHandler EdgeLButtonDblClick;
		public event LabelEventHandler LabelLButtonClick;
		public event LabelEventHandler LabelLButtonDblClick;

		#endregion

		#endregion

		#region Constructors

		public DiagramMouseManager(Diagram diagram)
		{
			_diagram = diagram;

			_currentResizeDirection = ResizeDirection.None;

			_leftButtonAction = MouseAction.None;
			_rightButtonAction = MouseAction.None;

			_leftButtonDown = false;
			_rightButtonDown = false;
			_leftButtonDownMousePosition = null;

			_currentMousePosition = null;
			_leftButtonDownMousePosition = null;

			_itemUnderCursor = null;

			_baseXViewOffset = 0;
			_baseYViewOffset = 0;

			_offsetWorker = null;

			Selector = null;
		}

		#endregion

		#region props

		public DiagramSelector Selector { get; set; }

		#endregion

		#region Methods

		public virtual void ProcessRightButtonDown()
		{
			if (_leftButtonDown)
				return;
			_diagram.CaptureMouse();
			_rightButtonDown = true;
			_rightButtonAction = MouseAction.MoveViewpot;

			_baseXViewOffset = _diagram.XViewOffset;
			_baseYViewOffset = _diagram.YViewOffset;

			_rightButtonDownMousePosition = _currentMousePosition;

		}

		public virtual void ProcessRightButtonUp()
		{
			_diagram.Cursor = Cursors.Arrow;
			_rightButtonDown = false;
			_rightButtonDownMousePosition = null;
			_diagram.ReleaseMouseCapture();
			_rightButtonAction = MouseAction.None;
		}

		public virtual void ProcessMouseDoubleClick(bool isLeft, bool isRight)
		{
			if (isLeft)
			{
				RaiseLButtonDblClick();
			}
		}

		public virtual void ProcessLeftButtonDown()
		{
			if (_rightButtonDown)
				return;

			Selector = null;

			_diagram.CaptureMouse();

			_leftButtonDown = true;
			_leftButtonAction = MouseAction.None;

			_mouseDownOnItem = _itemUnderCursor;

			if (_diagram.Edges.Count != 0)
			{
				bool res = _diagram.Edges[0].HitTest(_currentMousePosition.Value);
			}

			if (_diagram.Selection.Contains(_itemUnderCursor) || _currentResizeDirection != ResizeDirection.None)
			{
				_leftButtonAction = MouseAction.MoveItem;
			}
			else
			{
				if (_mouseDownOnItem as DiagramNode != null || (_mouseDownOnItem as DiagramLabel != null
					&& (_mouseDownOnItem as DiagramLabel).Owner as DiagramNode != null))
				{
					_leftButtonAction = MouseAction.CreateEdge;
				}
				else
				{
					_leftButtonAction = MouseAction.MultiSelect;
				}
			}
			_leftButtonDownMousePosition = _currentMousePosition;
		}

		public virtual void ProcessLeftButtonUp()
		{
			if (_offsetWorker != null)
			{
				_offsetWorker.CancelAsync();
				//_offsetWorker.
				//while (_offsetWorker.IsBusy) { }
				_offsetWorker = null;
			}

			if (_leftButtonDown && _leftButtonDownMousePosition != null && _currentMousePosition != null)
			{
				var isMouseMoved = MathUtils.Compare(GeometryUtils.Distance(_leftButtonDownMousePosition.Value, _currentMousePosition.Value), 0, GlobalData.PointPrecision) != 0;
				if (!isMouseMoved)
				{
					var selectionSucceded = SelectItem(_itemUnderCursor);
					if (!selectionSucceded)
					{
						CreateNode(_currentMousePosition.Value);
					}
					else
					{
						RaiseLButtonClick();
					}
				}
				else if (_leftButtonAction == MouseAction.MoveItem)
				{
					ResizeDirection direction = _currentResizeDirection;
					_currentResizeDirection = ResizeDirection.None;
					SetupSelectionEdgesEndings(direction);
				}
				else if (_leftButtonAction == MouseAction.MultiSelect)
				{
					UseSelector();
				}
				ChangeCursorKind(_currentMousePosition.Value);
			}

			_diagram.ReleaseMouseCapture();

			Selector = null;
			_leftButtonDown = false;
			_leftButtonDownMousePosition = null;
			_leftButtonAction = MouseAction.None;
		}

		public virtual void ProcessMouseMove(Point mousePosition, bool createSelector = true)
		{
			ResetIsUnderCursor(mousePosition);

			if (_leftButtonDown)
			{
				if (_offsetWorker == null)
				{
					_offsetWorker = new BackgroundWorker();
					_offsetWorker.WorkerSupportsCancellation = true;
					_offsetWorker.DoWork += new DoWorkEventHandler(CheckOffsets);
					_offsetWorker.RunWorkerAsync();
				}

				if (_leftButtonAction == MouseAction.MoveItem)
				{
					ResizeSelection(mousePosition);
				}
				if (_leftButtonAction == MouseAction.CreateEdge)
				{
					CreateEdge(mousePosition);
				}
				if (_leftButtonAction == MouseAction.MultiSelect)
				{
					ExpandSelector(mousePosition, createSelector);
				}
			}
			else if (_rightButtonDown)
			{
				if (_rightButtonAction == MouseAction.MoveViewpot)
				{
					MoveViewport(mousePosition);
				}
			}
			else
			{
				ChangeCursorKind(mousePosition);
			}

			_currentMousePosition = mousePosition;

		}

		private void ChangeCursorKind(Point mousePosition)
		{
			ResizeDirection? direction = null;
			foreach (var item in _diagram.Selection.Where(item => item.Border != null))
			{
				foreach (var resizeInfo in item.Border.ResizeInfos)
				{
					var rect = resizeInfo.Rect;
					if (rect.HasValue && rect.Value.Contains(mousePosition))
					{
						direction = resizeInfo.ResizeDirection;
					}
					if (direction != null)
						break;
				}

				if (direction != null)
					break;
			}

			if (direction == null)
			{
				// не на каком-то квадратике, ищем, внутри какого
				for (int i = 0; i < _diagram.Selection.Count; i++)
				{
					var item = _diagram.Selection[i];
					if (item.HitTest(mousePosition))
					{
						direction = ResizeDirection.AllRound;
						break;
					}
				}
			}

			_currentResizeDirection = direction != null ? direction.Value : ResizeDirection.None;
			_diagram.Cursor = Utils.ResizeDirectionToCursor(_currentResizeDirection);
		}

		protected void ResizeSelection(Point mousePosition)
		{
			if (_currentMousePosition == null)
				return;

			using (new DiagramUpdateLock(_diagram))
			{

				Point offset = new Point(mousePosition.X - _currentMousePosition.Value.X, mousePosition.Y - _currentMousePosition.Value.Y);
				switch (_currentResizeDirection)
				{
					case ResizeDirection.HorizontalLeft:
						{
							foreach (var item in _diagram.Selection)
							{
								var node = item as DiagramNode;
								if (node == null)
									continue;

								var nodeBounds = node.Bounds;
								var topLeft = nodeBounds.TopLeft;
								var bottomRight = nodeBounds.BottomRight;

								topLeft.Offset(offset.X, 0);
								if (Math.Abs(topLeft.X - bottomRight.X) < GlobalData.DefaulNodeEdgeLength)
									topLeft.X = bottomRight.X - GlobalData.DefaulNodeEdgeLength;

								node.Bounds = new Rect(topLeft, bottomRight);
							}
						}
						break;
					case ResizeDirection.HorizontalRight:
						{
							foreach (var item in _diagram.Selection)
							{
								var node = item as DiagramNode;
								if (node == null)
									continue;

								var nodeBounds = node.Bounds;
								var topLeft = nodeBounds.TopLeft;
								var bottomRight = nodeBounds.BottomRight;

								bottomRight.Offset(offset.X, 0);
								if (Math.Abs(topLeft.X - bottomRight.X) < GlobalData.DefaulNodeEdgeLength)
									bottomRight.X = topLeft.X + GlobalData.DefaulNodeEdgeLength;

								node.Bounds = new Rect(topLeft, bottomRight);
							}
						}
						break;
					case ResizeDirection.VerticalTop:
						{
							foreach (var item in _diagram.Selection)
							{
								var node = item as DiagramNode;
								if (node == null)
									continue;

								var nodeBounds = node.Bounds;
								var topLeft = nodeBounds.TopLeft;
								var bottomRight = nodeBounds.BottomRight;

								topLeft.Offset(0, offset.Y);
								if (Math.Abs(topLeft.Y - bottomRight.Y) < GlobalData.DefaulNodeEdgeLength)
									topLeft.Y = bottomRight.Y - GlobalData.DefaulNodeEdgeLength;

								node.Bounds = new Rect(topLeft, bottomRight);
							}
						}
						break;
					case ResizeDirection.VerticalBottom:
						{
							foreach (var item in _diagram.Selection)
							{
								var node = item as DiagramNode;
								if (node == null)
									continue;

								var nodeBounds = node.Bounds;
								var topLeft = nodeBounds.TopLeft;
								var bottomRight = nodeBounds.BottomRight;

								bottomRight.Offset(0, offset.Y);
								if (Math.Abs(topLeft.Y - bottomRight.Y) < GlobalData.DefaulNodeEdgeLength)
									bottomRight.Y = topLeft.Y + GlobalData.DefaulNodeEdgeLength;

								node.Bounds = new Rect(topLeft, bottomRight);
							}
						}
						break;
					case ResizeDirection.AscendingTopRight:
						{
							foreach (var item in _diagram.Selection)
							{
								var node = item as DiagramNode;
								if (node == null)
									continue;

								var nodeBounds = node.Bounds;
								var topRight = nodeBounds.TopRight;
								var bottomLeft = nodeBounds.BottomLeft;

								topRight.Offset(offset.X, offset.Y);

								if (Math.Abs(topRight.X - bottomLeft.X) < GlobalData.DefaulNodeEdgeLength)
									topRight.X = bottomLeft.X + GlobalData.DefaulNodeEdgeLength;
								if (Math.Abs(topRight.Y - bottomLeft.Y) < GlobalData.DefaulNodeEdgeLength)
									topRight.Y = bottomLeft.Y - GlobalData.DefaulNodeEdgeLength;

								node.Bounds = new Rect(new Point(bottomLeft.X, topRight.Y), new Point(topRight.X, bottomLeft.Y));
							}
						}
						break;
					case ResizeDirection.AscendingBottomLeft:
						{
							foreach (var item in _diagram.Selection)
							{
								var node = item as DiagramNode;
								if (node == null)
									continue;

								var nodeBounds = node.Bounds;
								var topRight = nodeBounds.TopRight;
								var bottomLeft = nodeBounds.BottomLeft;

								bottomLeft.Offset(offset.X, offset.Y);

								if (Math.Abs(topRight.X - bottomLeft.X) < GlobalData.DefaulNodeEdgeLength)
									bottomLeft.X = topRight.X - GlobalData.DefaulNodeEdgeLength;
								if (Math.Abs(topRight.Y - bottomLeft.Y) < GlobalData.DefaulNodeEdgeLength)
									bottomLeft.Y = topRight.Y + GlobalData.DefaulNodeEdgeLength;

								node.Bounds = new Rect(new Point(bottomLeft.X, topRight.Y), new Point(topRight.X, bottomLeft.Y));
							}
						}
						break;
					case ResizeDirection.DescendingTopLeft:
						{
							foreach (var item in _diagram.Selection)
							{
								var node = item as DiagramNode;
								if (node == null)
									continue;

								var nodeBounds = node.Bounds;
								var topLeft = nodeBounds.TopLeft;
								var bottomRight = nodeBounds.BottomRight;

								topLeft.Offset(offset.X, offset.Y);

								if (Math.Abs(topLeft.X - bottomRight.X) < GlobalData.DefaulNodeEdgeLength)
									topLeft.X = bottomRight.X - GlobalData.DefaulNodeEdgeLength;
								if (Math.Abs(topLeft.Y - bottomRight.Y) < GlobalData.DefaulNodeEdgeLength)
									topLeft.Y = bottomRight.Y - GlobalData.DefaulNodeEdgeLength;

								node.Bounds = new Rect(topLeft, bottomRight);
							}
						}
						break;
					case ResizeDirection.DescendingBottomRight:
						{
							foreach (var item in _diagram.Selection)
							{
								var node = item as DiagramNode;
								if (node == null)
									continue;

								var nodeBounds = node.Bounds;
								var topLeft = nodeBounds.TopLeft;
								var bottomRight = nodeBounds.BottomRight;

								bottomRight.Offset(offset.X, offset.Y);

								if (Math.Abs(topLeft.X - bottomRight.X) < GlobalData.DefaulNodeEdgeLength)
									bottomRight.X = topLeft.X + GlobalData.DefaulNodeEdgeLength;
								if (Math.Abs(topLeft.Y - bottomRight.Y) < GlobalData.DefaulNodeEdgeLength)
									bottomRight.Y = topLeft.Y + GlobalData.DefaulNodeEdgeLength;

								node.Bounds = new Rect(topLeft, bottomRight);
							}
						}
						break;
					case ResizeDirection.AllRound:
						{
							foreach (var item in _diagram.Selection)
							{
								DiagramNode node = item as DiagramNode;
								if (node != null)
								{
									var nodeBounds = node.Bounds;
									nodeBounds.Offset(offset.X, offset.Y);
									node.Bounds = nodeBounds;
								}

								DiagramLabel label = item as DiagramLabel;
								if (label != null)
								{
									var relPos = label.RelativePosition;
									relPos.Offset(offset.X, offset.Y);
									label.RelativePosition = relPos;
								}

								DiagramEdge edge = item as DiagramEdge;
								if (edge != null)
								{
									edge.AnchoringMode = EdgeAnchoringMode.PointToPoint;

									var sourcePoint = edge.SourcePoint;
									sourcePoint.Offset(offset.X, offset.Y);
									edge.SourcePoint = sourcePoint;

									var destinationPoint = edge.DestinationPoint;
									destinationPoint.Offset(offset.X, offset.Y);
									edge.DestinationPoint = destinationPoint;
								}
							}
						}
						break;
					case ResizeDirection.AllRoundSource:
						{
							foreach (var item in _diagram.Selection)
							{
								var edge = item as DiagramEdge;
								if (edge == null)
									continue;

								if (edge.AnchoringMode == EdgeAnchoringMode.NodeToPoint)
									edge.AnchoringMode = EdgeAnchoringMode.PointToPoint;
								if (edge.AnchoringMode == EdgeAnchoringMode.NodeToNode)
									edge.AnchoringMode = EdgeAnchoringMode.PointToNode;

								Point sourcePoint = edge.SourcePoint;
								sourcePoint.Offset(offset.X, offset.Y);
								edge.SourcePoint = sourcePoint;
							}
						}
						break;
					case ResizeDirection.AllRoundDestination:
						{
							foreach (var item in _diagram.Selection)
							{
								var edge = item as DiagramEdge;
								if (edge == null)
									continue;

								if (edge.AnchoringMode == EdgeAnchoringMode.PointToNode)
									edge.AnchoringMode = EdgeAnchoringMode.PointToPoint;
								if (edge.AnchoringMode == EdgeAnchoringMode.NodeToNode)
									edge.AnchoringMode = EdgeAnchoringMode.NodeToPoint;

								Point destinationPoint = edge.DestinationPoint;
								destinationPoint.Offset(offset.X, offset.Y);
								edge.DestinationPoint = destinationPoint;
							}
						}
						break;
				}
			}
		}

		private void MoveViewport(Point mousePosition)
		{
			if (_rightButtonDownMousePosition == null)
				return;

			if (_diagram.Cursor != Cursors.SizeAll)
				_diagram.Cursor = Cursors.SizeAll;

			mousePosition.X += _diagram.XViewOffset - _baseXViewOffset;
			mousePosition.Y += _diagram.YViewOffset - _baseYViewOffset;

			using (DiagramRenderLock diagramLock = new DiagramRenderLock(_diagram))
			{
				_diagram.XViewOffset = _baseXViewOffset + mousePosition.X - _rightButtonDownMousePosition.Value.X;
				_diagram.YViewOffset = _baseYViewOffset + mousePosition.Y - _rightButtonDownMousePosition.Value.Y;
			}
		}

		protected void SetupSelectionEdgesEndings(ResizeDirection direction)
		{
			var nodeUnderCursor = _diagram.Nodes.FirstOrDefault(item => item.IsUnderCursor);
			if (nodeUnderCursor == null)
				return;

			using (new DiagramUpdateLock(_diagram))
			{
				foreach (var item in _diagram.Selection)
				{
					var edge = item as DiagramEdge;
					if (edge == null)
						return;

					if (direction == ResizeDirection.AllRoundSource)
					{
						if (edge.AnchoringMode == EdgeAnchoringMode.PointToPoint)
							edge.AnchoringMode = EdgeAnchoringMode.NodeToPoint;
						if (edge.AnchoringMode == EdgeAnchoringMode.PointToNode)
							edge.AnchoringMode = EdgeAnchoringMode.NodeToNode;

						edge.SourceNode = nodeUnderCursor;
					}
					if (direction == ResizeDirection.AllRoundDestination)
					{
						if (edge.AnchoringMode == EdgeAnchoringMode.PointToPoint)
							edge.AnchoringMode = EdgeAnchoringMode.PointToNode;
						if (edge.AnchoringMode == EdgeAnchoringMode.NodeToPoint)
							edge.AnchoringMode = EdgeAnchoringMode.NodeToNode;

						edge.DestinationNode = nodeUnderCursor;
					}
				}
				ResetIsUnderCursor(_currentMousePosition.Value);
			}
		}

		private bool SelectItem(DiagramItem item)
		{
			bool isSelectionExists = _diagram.Selection.Count != 0;

			if (item == null)
			{
				_diagram.Selection.Clear();

				return isSelectionExists;
			}

			if (item.IsSelectable == false)
			{
				var label = item as DiagramLabel;
				if (label.Owner != null)
					item = label.Owner;
			}

			bool isLabel = item as DiagramLabel != null;
			bool isNode = item as DiagramNode != null;
			bool isEdge = item as DiagramEdge != null;

			bool isLabelSelection = !isSelectionExists ? false : (_diagram.Selection[0] as DiagramLabel != null);
			bool isNodeSelection = !isSelectionExists ? false : (_diagram.Selection[0] as DiagramNode != null);
			bool isEdgeSelection = !isSelectionExists ? false : (_diagram.Selection[0] as DiagramEdge != null);

			if (isLabel && !isLabelSelection)
				_diagram.Selection.Clear();

			if (isNode && !isNodeSelection)
				_diagram.Selection.Clear();

			if (isEdge && !isEdgeSelection)
				_diagram.Selection.Clear();

			if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
				_diagram.Selection.Clear();

			if (!_diagram.Selection.Contains(item))
				_diagram.Selection.Add(item);

			return true;
		}

		private void CreateNode(Point mousePosition)
		{
			using (new DiagramUpdateLock(_diagram))
			{
				var newNode = new DiagramNode(_diagram);
				newNode.Bounds = new Rect(mousePosition.X, mousePosition.Y, GlobalData.DefaultNodeSize.Width, GlobalData.DefaultNodeSize.Height);
			}
		}

		private void CreateEdge(Point mousePosition)
		{
			var node = _mouseDownOnItem as DiagramNode;
			if (node == null)
			{
				var label = _mouseDownOnItem as DiagramLabel;
				node = label.Owner as DiagramNode;
			}
			if (node != null && !node.Bounds.Contains(mousePosition))
			{
				using (new DiagramUpdateLock(_diagram))
				{
					var edge = new DiagramEdge(_diagram);
					edge.AnchoringMode = EdgeAnchoringMode.NodeToPoint;
					edge.SourceNode = node;
					edge.DestinationPoint = mousePosition;
					_currentResizeDirection = ResizeDirection.AllRoundDestination;

					_diagram.Selection.Clear();
					_diagram.Selection.Add(edge);
				}
				_leftButtonAction = MouseAction.MoveItem;
				_mouseDownOnItem = null;
			}
		}

		protected void ResetIsUnderCursor(Point mousePosition)
		{
			foreach (var node in _diagram.Nodes)
			{
				node.IsUnderCursor = false;
			}

			List<DiagramItem> items = new List<DiagramItem>(FindHittedItems(mousePosition));
			_itemUnderCursor = null;
			for (int i = 0; i < items.Count; i++)
			{
				var node = items[i] as DiagramNode;

				if (node != null && (_currentResizeDirection == ResizeDirection.AllRoundSource || _currentResizeDirection == ResizeDirection.AllRoundDestination))
				{
					node.IsUnderCursor = true;
				}

				if (i == 0)
				{
					_itemUnderCursor = items[i];
				}
			}
		}

		private void ExpandSelector(Point mousePosition, bool createSelector)
		{
			if (Selector == null)
			{
				if (createSelector)
				{
					Selector = new DiagramSelector();
					Selector.StartPoint = mousePosition;
				}
				return;
			}

			Selector.EndPoint = mousePosition;

			_diagram.InvalidateVisual();
		}

		private void UseSelector()
		{
			if (Selector == null)
				return;

			using (DiagramUpdateLock locker = new DiagramUpdateLock(_diagram))
			{
				Geometry selectorGeometry = Selector.Geometry;

				_diagram.Selection.Clear();

				foreach (var node in _diagram.Nodes)
				{
					if (node.Geometry == null || node.NeedRecalc)
						node.CalculateGeometry();

					if (node.Geometry == null)
						continue;

					if (GeometryUtils.AreIntersectOrContain(node.Geometry, node.BorderPen,
						selectorGeometry, GlobalData.SelectorBorderPen))
					{
						_diagram.Selection.Add(node);
					}
				}

				Selector = null;
			}
		}

		protected IEnumerable<DiagramItem> FindHittedItems(Point mousePoint)
		{
			var result = new List<DiagramItem>();
			for (var i = _diagram.ItemsInDrawingOrder.Count - 1; i >= 0; i--)
			{
				var item = _diagram.ItemsInDrawingOrder[i];
				if (item.HitTest(mousePoint))
				{
					result.Add(item);
				}
			}
			return result;
		}

		public virtual void ProcessMouseWheel(Point point, int delta)
		{
			if (_leftButtonDown || _rightButtonDown)
				return;

			bool zoomIn = delta > 0;
			
			int cnt = Math.Abs(delta / 120);
			for (int i = 0; i < cnt; i++)
			{
				if (zoomIn)
				{
					_diagram.ZoomIn(point);
				}
				else
				{
					_diagram.ZoomOut(point);
				}
			}
		}

		private void RaiseLButtonClick()
		{
			var label = _itemUnderCursor as DiagramLabel;
			if (label != null && LabelLButtonClick != null)
			{
				LabelLButtonClick(new LabelEventArgs(_diagram, label));
			}

			var node = _itemUnderCursor as DiagramNode;
			if (node != null && NodeLButtonClick != null)
			{
				NodeLButtonClick(new NodeEventArgs(_diagram, node));
			}

			var edge = _itemUnderCursor as DiagramEdge;
			if (edge != null && EdgeLButtonClick != null)
			{
				EdgeLButtonClick(new EdgeEventArgs(_diagram, edge));
			}
		}

		private void RaiseLButtonDblClick()
		{
			var label = _itemUnderCursor as DiagramLabel;
			if (label != null && LabelLButtonDblClick != null)
			{
				LabelLButtonDblClick(new LabelEventArgs(_diagram, label));
			}

			var node = _itemUnderCursor as DiagramNode;
			if (node != null && NodeLButtonDblClick != null)
			{
				NodeLButtonDblClick(new NodeEventArgs(_diagram, node));
			}

			var edge = _itemUnderCursor as DiagramEdge;
			if (edge != null && EdgeLButtonDblClick != null)
			{
				EdgeLButtonDblClick(new EdgeEventArgs(_diagram, edge));
			}
		}

		protected void CheckOffsets(object sender, DoWorkEventArgs args)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			if (worker == null)
				return;

			while (true)
			{
				if (worker.CancellationPending == true)
					break;

				_diagram.Dispatcher.Invoke((Action)(() =>
					{
						Rect viewport = _diagram.Viewport;
						Point currentPoint = _currentMousePosition.Value;
						Point currentScreenPoint = currentPoint.ToDisplayPoint(_diagram.Offset, _diagram.Scale);

						bool moveUp = false;
						bool moveDown = false;
						bool moveLeft = false;
						bool moveRight = false;

						double offsetX = viewport.Width * 0.05;
						double offsetY = viewport.Height * 0.05;

						if (currentPoint.X - viewport.Left < offsetX)
							moveLeft = true;

						if (viewport.Right - currentPoint.X < offsetX)
							moveRight = true;

						if (currentPoint.Y - viewport.Top < offsetY)
							moveUp = true;

						if (viewport.Bottom - currentPoint.Y < offsetY)
							moveDown = true;

						using (DiagramRenderLock locker = new DiagramRenderLock(_diagram))
						{
							if (moveUp)
								_diagram.YViewOffset += offsetY;

							if (moveDown)
								_diagram.YViewOffset -= offsetY;

							if (moveLeft)
								_diagram.XViewOffset += offsetX;

							if (moveRight)
								_diagram.XViewOffset -= offsetX;
						}

						ProcessMouseMove(currentScreenPoint.ToViewPoint(_diagram.Offset, _diagram.Scale), false);
					}));

				Thread.Sleep(100);
			}
		}

		#endregion
	}
}
