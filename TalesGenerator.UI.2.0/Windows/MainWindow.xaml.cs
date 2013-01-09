using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Windows.Controls.Ribbon;
using Microsoft.Win32;

using TalesGenerator.Net;
using TalesGenerator.TaleNet;
using TalesGenerator.UI.Classes;
using TalesGenerator.UI.Properties;

using TalesGenerator.UI.Controls;

using Gt.Controls;
using Gt.Controls.Diagramming;
using Gt.Controls.Diagramming.NodeDrawers;
using Gt.Controls.Diagramming.EdgeDrawers;

using System.Xml.Linq;

namespace TalesGenerator.UI.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow
	{

		#region Fields

		Project _project;

		bool _updatingDiagramSelection;

		NetworkEdgeType _currentType;

		private bool _lockEdgeUpdate;

		private bool _lockNodeUpdate;

		#endregion

		#region Constructors

		public MainWindow()
		{
			InitializeComponent();

			_project = new Project();
			_project.Diagram = NetworkVisual;
			_project.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_project_PropertyChanged);
			_project.LockEdgeUpdate += new BoolNotifyEventHander(_project_LockEdgeUpdate);
			_project.EdgeCreated += new DiagramItemEventHandler(_project_EdgeCreated);
			_project.LockNodeUpdate += new BoolNotifyEventHander(_project_LockNodeUpdate);

			NetworkVisual.IsEnabled = _project.Network != null;
			NetworkVisual.DefaultNodeDrawer = new EllipseNodeDrawer();
			NetworkVisual.DefaultEdgeDrawer = new ArrowEdgeDrawer();

			NetworkVisual.Nodes.CollectionChanged += new NotifyCollectionChangedEventHandler(VisualNodes_CollectionChanged);
			NetworkVisual.Edges.CollectionChanged += new NotifyCollectionChangedEventHandler(VisualEdges_CollectionChanged);
			NetworkVisual.Selection.CollectionChanged += new NotifyCollectionChangedEventHandler(VisualSelection_CollectionChanged);
			//ButtonViewShowPropsPanel.IsChecked = this.PanelProps.Visibility == Visibility.Visible;
			//DispatcherPanelButton.IsChecked = this.DispatcherPanel.Visibility == System.Windows.Visibility.Visible;
			DiagramView.Diagram = NetworkVisual;

			DispatcherPanel.SelectionChanged += new IntNotifyEventHandler(DispatcherPanel_SelectionChanged);
			DispatcherPanel.SelectLinkedNodes += new IntNotifyEventHandler(DispatcherPanel_SelectLinkedNodes);
			_updatingDiagramSelection = false;

			_currentType = NetworkEdgeType.IsA;

			_lockEdgeUpdate = false;
			_lockNodeUpdate = false;

			AssignNetwork();
		}

		void _project_LockEdgeUpdate(bool value)
		{
			_lockEdgeUpdate = value;
		}

		void _project_LockNodeUpdate(bool value)
		{
			_lockNodeUpdate = value;
		}

		void _project_EdgeCreated(DiagramItem item, NetworkObject obj)
		{
			OnEdgeCreated(item as DiagramEdge);
		}

		void _project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Network")
			{
				AssignNetwork();
			}
		}

		#endregion

		#region CommandHandlers

		#region AppMenu Commands

		private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void New_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			_project.Network = new TalesNetwork();
			NetworkVisual.ClearAll();
			_project.RebuildDiagram();
		}

		private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (_project.Network != null)
				this.CloseProject_Executed(sender, e);
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Filter = "TalesGeneratorProject files (*.tgp)|*.tgp | Xml files (*.xml)|*xml";
			bool? result = openDialog.ShowDialog();
			if (result == true)
			{
				_lockEdgeUpdate = true;
				_lockNodeUpdate = true;

				_project.Path = openDialog.FileName;
				_project.Load();
				if (_project.Network != null)
				{
					NetworkVisual.IsEnabled = true;
				}
				AssignNetwork();

				_lockEdgeUpdate = false;
				_lockNodeUpdate = false;
			}
		}

		private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (_project.Path != "")
			{
				_project.Save();
				RefreshFrame();
				return;
			}
			Save_AsExecuted(sender, e);
		}

		private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		private void Save_AsExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.Filter = "TalesGeneratorProject files (*.tgp)|*.tgp";
			bool? result = saveDialog.ShowDialog();
			if (result == true)
			{
				_project.Path = saveDialog.FileName;

				_project.Save();
			}
			RefreshFrame();
		}

		private void CloseProject_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		private void CloseProject_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.DoClose();
		}

		private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
			
		}

		private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (DoClose())
				this.Close();
		}

		private void SaveAsPdf_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		private void SaveAsPdf_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ExportDiagram(ExportType.pdf);
		}

		private void SaveAsSvg_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		private void SaveAsSvg_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ExportDiagram(ExportType.svg);
		}

		private void Export_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		#endregion

		#region View category

		private void ShowProps_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void ShowProps_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			//this.PanelProps.Visibility = this.PanelProps.Visibility == Visibility.Collapsed ?
			//    Visibility.Visible : Visibility.Collapsed;
		}

		private void ShowDispatcher_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void ShowDispatcher_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			//ContentGrid.ColumnDefinitions[0].Width = GridLength.Auto;
			DispatcherPanel.Visibility = DispatcherPanel.Visibility == System.Windows.Visibility.Visible ?
				Visibility.Collapsed : Visibility.Visible;
			this.InvalidateArrange();
		}

		private void ZoomOut_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		private void ZoomOut_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.DoZoom(true);
		}

		private void ZoomIn_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		private void ZoomIn_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.DoZoom(false);
		}



		private void Layout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		private void Layout_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ApplyLayout();
		}

		#endregion

		#region Network Category

		private void EditXml_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		private void EditXml_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string xml = _project.Network.SaveToXml().ToString();

			XmlEditWindow wnd = new XmlEditWindow(xml);
			bool? result = wnd.ShowDialog();
			if (result == true)
			{
				_lockEdgeUpdate = true;
				_lockNodeUpdate = true;
				XDocument xDoc = XDocument.Parse(wnd.Result);

				TalesNetwork talesNetwork = new TalesNetwork();
				talesNetwork.LoadFromXml(xDoc);

				_project.Network = talesNetwork;
				_project.RebuildDiagram();

				AssignNetwork();

				_lockEdgeUpdate = false;
				_lockNodeUpdate = false;
			}
		}

		#endregion

		#region Links Category

		private void ChooseLinkType_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			bool canExecute = NetworkVisual != null && NetworkVisual.IsEnabled;
			if (e.Parameter != null && Utils.ConvertType(e.Parameter as string) == _currentType)
			{

				RibbonRadioButton button = GetButton(e.Parameter as string);
				if (button != null)
					button.IsChecked = true;
			}
			e.CanExecute = canExecute;
		}

		private RibbonRadioButton GetButton(string p)
		{
			RibbonRadioButton button = null;
			foreach (RibbonRadioButton item in LinksGroup.Items)
			{
				if (item.Label == p)
				{
					button = item;
					break;
				}
			}
			return button;
		}

		private void ChooseLinkType_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (e.Parameter != null)
			{
				NetworkEdgeType type = Utils.ConvertType(e.Parameter as string);
				if (NetworkVisual.Selection.Count == 1 &&
					(NetworkVisual.Selection[0] as DiagramEdge != null))
				{
					DiagramEdge link = NetworkVisual.Selection[0] as DiagramEdge;
					NetworkEdge edge = _project.Network.Edges.FindById(Convert.ToInt32(link.UserData));
					try
					{
						edge.Type = type;
					}
					catch (Exception ex)
					{
						ShowErrorMessage(ex.Message);
					}
				}
				_currentType = type;
			}
		}

		private void DeleteLink_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null &&
				NetworkVisual != null && NetworkVisual.Selection.Count == 1 &&
				NetworkVisual.Selection[0] as DiagramEdge != null;
		}

		private void DeleteLink_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			NetworkVisual.Edges.Remove(NetworkVisual.Selection[0] as DiagramEdge);
		}

		#endregion

		#region Consult category

		private void StartConsult_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		private void StartConsult_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ConsultWindow consultWnd = new ConsultWindow(_project.Network);
			consultWnd.ShowDialog();
		}

		private void Consult2_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ConsultWindow2 consultWnd = new ConsultWindow2(_project.Network);
			consultWnd.ShowDialog();
		}

		private void Consult2_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null;
		}

		#endregion

		#region Nodes category

		private void RenameNode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute =_project != null && _project.Network!= null &&
				NetworkVisual != null && NetworkVisual.Selection.Count == 1 &&
				NetworkVisual.Selection[0] as DiagramNode != null;
		}

		private void RenameNode_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DiagramNode node = NetworkVisual.Selection[0] as DiagramNode;
			if (node == null)
				return;

			NetworkNode netNode = _project.Network.Nodes.FindById(Int32.Parse(node.UserData));
			if (netNode == null)
				return;

			StringEditWindow edit = new StringEditWindow(netNode.Name);
			edit.ShowDialog();
			bool? res = edit.DialogResult;
			if (res == true)
			{
				netNode.Name = edit.Value;
			}
		}

		private void DeleteNode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null &&
				NetworkVisual != null && NetworkVisual.Selection.Count == 1 &&
				NetworkVisual.Selection[0] as DiagramNode != null;
		}

		private void DeleteNode_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			NetworkVisual.Nodes.Remove(NetworkVisual.Selection[0] as DiagramNode);
		}

		#endregion

		#region NonRibbon Commands

		private void StartEdit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null &&
				NetworkVisual != null && NetworkVisual.Selection.Count == 1 &&
				NetworkVisual.Selection[0] as DiagramNode != null;
		}

		private void StartEdit_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DiagramNode node = NetworkVisual.Selection[0] as DiagramNode;
			if (node == null)
				return;

			// TODO
			//_project.StartEdit(node);
		}

		#endregion

		#endregion

		#region Window events

		private void RibbonWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ResizeDiagram();
		}

		private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!DoClose())
				e.Cancel = true;
		}

		#endregion

		#region MF events

		private void DiagramNetwork_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			int count = Math.Abs(e.Delta / 120);
			bool zoomOut = e.Delta < 0;
			if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
			{
				for (int i = 0; i < count; i++)
				{
					DoZoom(zoomOut);
				}
				e.Handled = true;
			}
		}

		#region NodeEvents

		private void OnNodeCreated(DiagramNode node)
		{
			if (node == null)
				return;

			TalesNetwork network = _project.Network;
			NetworkNode netNode = network.Nodes.Add();

			_project.NodeAdded(node, netNode);

		}

		private void OnNodeDeleted(DiagramNode node)
		{
			if (node == null)
				return;

			TalesNetwork network = _project.Network;
			if (network == null)
				return;

			NetworkNode netNode = network.Nodes.FindById(Int32.Parse(node.UserData));
			if (netNode != null)
				network.Nodes.Remove(netNode);

		}

		private void OnNodeSelected(DiagramNode node)
		{
			if (node == null)
				return;

			TalesNetwork network = _project.Network;
			if (network == null)
				return;

			NetworkNode netNode = network.Nodes.FindById(Int32.Parse(node.UserData));
			//PanelProps.Node = netNode;

			//netNode.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(netNode_PropertyChanged);

		}

		////void netNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		////{
		////    ShapeNode mfNode = DiagramNetwork.Selection.Items[0] as ShapeNode;
		////    if (mfNode == null)
		////        return;

		////    Network network = _project.Network;
		////    NetworkNode node = network.Nodes.FindById(Int32.Parse(mfNode.Uid));

		////    if (e.PropertyName == "Name")
		////    {
		////        mfNode.Text = node.Name;
		////    }
		////}

		private void OnNodeDeselected(DiagramNode node)
		{
			if (node == null)
				return;

			TalesNetwork network = _project.Network;
			if (network == null)
				return;

			NetworkNode netNode = network.Nodes.FindById(Int32.Parse(node.UserData));

			//if (netNode == PanelProps.Node)
			//    PanelProps.Node = null;

			//if (netNode != null)
			//    netNode.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(netNode_PropertyChanged);
		}

		#endregion

		#region Link events

		private void OnEdgeCreated(DiagramEdge edge)
		{
			if (edge == null)
				return;

			_project.SubscribeOnEdgeEvents(edge);

			if (edge.SourceNode == null || edge.DestinationNode == null || edge.AnchoringMode != EdgeAnchoringMode.NodeToNode)
			{
			//    NetworkVisual.Edges.Remove(edge);
			//    MessageBox.Show("Нельзя создавать неприязанные дуги", "Ошибка!");
				return;
			}

			TalesNetwork network = _project.Network;
			if (network == null)
				return;

			NetworkNode origin = network.Nodes.FindById(Int32.Parse(edge.SourceNode.UserData));
			NetworkNode destination = network.Nodes.FindById(Int32.Parse(edge.DestinationNode.UserData));

			try
			{
				NetworkEdge networkEdge = network.Edges.Add(origin, destination, _currentType);
				_project.LinkAdded(edge, networkEdge);


			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				NetworkVisual.Edges.Remove(edge);
			}
		}

		private void OnEdgeDeleted(DiagramEdge edge)
		{
			if (edge == null)
				return;

			TalesNetwork network = _project.Network;
			if (network == null)
				return;

			NetworkEdge networkEdge = network.Edges.FindById(Convert.ToInt32(edge.UserData));
			if (networkEdge != null)
				network.Edges.Remove(networkEdge);
		}

		private void OnEdgeSelected(DiagramEdge edge)
		{
			if (edge == null || edge.UserData == null)
				return;

			TalesNetwork network = _project.Network;
			if (network == null)
				return;

			NetworkEdge networkEdge = network.Edges.FindById(Int32.Parse(edge.UserData));
			//PanelProps.Edge = edge;
			if (networkEdge == null)
				return;

			_currentType = networkEdge.Type;

			//edge.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(edge_PropertyChanged);

		}

		private void OnEdgeDeselected(DiagramEdge edge)
		{
			if (edge == null)
				return;

			TalesNetwork network = _project.Network;
			if (network == null)
				return;

			NetworkEdge networkEdge = network.Edges.FindById(Int32.Parse(edge.UserData));

			//if (PanelProps.Edge == edge)
			//    PanelProps.Edge = null;
			//if (edge != null)
			//    edge.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(edge_PropertyChanged);
		}

		//void edge_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		//{
		//    DiagramLink link = DiagramNetwork.Selection.Items[0] as DiagramLink;
		//    if (link == null)
		//        return;

		//    Network network = _project.Network;
		//    NetworkEdge edge = network.Edges.FindById(Int32.Parse(link.Uid));

		//    if (e.PropertyName == "Type")
		//    {
		//        link.Text = Utils.ConvertType(edge.Type);
		//    }

		//}

		#endregion

		private void VisualNodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (_lockNodeUpdate)
				return;

			_lockNodeUpdate = true;

			if (e.OldItems != null)
			{
				foreach (DiagramNode node in e.OldItems)
				{
					OnNodeDeleted(node);
				}
			}

			if (e.NewItems != null)
			{
				foreach (DiagramNode node in e.NewItems)
				{
					OnNodeCreated(node);
				}
			}

			_lockNodeUpdate = false;
		}

		private void VisualEdges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (_lockEdgeUpdate == true)
				return;

			_lockEdgeUpdate = true;

			if (e.OldItems != null)
			{
				foreach (DiagramEdge edge in e.OldItems)
				{
					OnEdgeDeleted(edge);
				}
			}

			if (e.NewItems != null)
			{
				foreach (DiagramEdge edge in e.NewItems)
				{
					OnEdgeCreated(edge);
				}
			}

			_lockEdgeUpdate = false;
		}

		private void VisualSelection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (DiagramItem item in e.OldItems)
				{
					OnItemDeselected(item);
				}
			}

			if (e.NewItems != null)
			{
				foreach (DiagramItem item in e.NewItems)
				{
					OnItemSelected(item);
				}
			}

			OnSelectionChanged();
		}

		private void OnItemDeselected(DiagramItem item)
		{
			var node = item as DiagramNode;
			if (node != null)
			{
				OnNodeDeselected(node);
			}

			var edge = item as DiagramEdge;
			if (edge != null)
			{
				OnEdgeDeselected(edge);
			}
		}

		private void OnItemSelected(DiagramItem item)
		{
			var node = item as DiagramNode;
			if (node != null)
			{
				OnNodeSelected(node);
			}

			var edge = item as DiagramEdge;
			if (edge != null)
			{
				OnEdgeSelected(edge);
			}
		}

		private void OnSelectionChanged()
		{
			if (_updatingDiagramSelection)
				return;
			_updatingDiagramSelection = true;
			if (NetworkVisual.Selection.Count == 0)
			{
				DispatcherPanel.SetSelection(-1);
				_updatingDiagramSelection = false;
				return;
			}
			DiagramItem item = NetworkVisual.Selection[0];
			int id = Convert.ToInt32(item.UserData);
			DispatcherPanel.SetSelection(id);
			_updatingDiagramSelection = false;
		}

		#endregion

		#region ScrollDiagram events

		private void ScrollDiagram_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				DiagramItemCollection<DiagramItem> selectedItems = NetworkVisual.Selection;
				for (int count = selectedItems.Count, i = count - 1; i >= 0; i--)
				{
					DiagramItem item = selectedItems[i];
					if (item as DiagramNode != null) NetworkVisual.Nodes.Remove(item as DiagramNode);
					else NetworkVisual.Edges.Remove(item as DiagramEdge);

				}
				e.Handled = true;
			}
		}

		#endregion

		#region Panels events

		void DispatcherPanel_SelectionChanged(int id)
		{
			if (_updatingDiagramSelection)
				return;

			// TODO

			_updatingDiagramSelection = true;
			NetworkVisual.Selection.Clear();
			DiagramItem item = Utils.FindItemByUserData(NetworkVisual, id);
			NetworkVisual.Selection.Add(item);
			NetworkVisual.GoToItem(item);
			_updatingDiagramSelection = false;
		}

		void DispatcherPanel_SelectLinkedNodes(int id)
		{
			if (_updatingDiagramSelection)
				return;

			using (DiagramUpdateLock locker = new DiagramUpdateLock(NetworkVisual))
			{

				_updatingDiagramSelection = true;
				NetworkVisual.Selection.Clear();
				DiagramNode node = Utils.FindItemByUserData(NetworkVisual, id) as DiagramNode;
				if (node != null)
				{
					foreach (var edge in node.Edges)
					{
						if (edge.AnchoringMode == EdgeAnchoringMode.NodeToNode)
						{
							var selNode = edge.SourceNode == node ? edge.DestinationNode : edge.SourceNode;
							NetworkVisual.Selection.Add(selNode);
						}
					}

					if (NetworkVisual.Selection.Count != 0)
					{
						NetworkVisual.GoToItem(NetworkVisual.Selection[0]);
					}
				}
				_updatingDiagramSelection = false;
			}
		}

		#endregion

		#region Window methods

		/// <summary>
		/// Ресайз диаграммы
		/// </summary>
		protected void ResizeDiagram()
		{
			//double factor = 100 / DiagramNetwork.ZoomFactor;
			//Rect newBounds = new Rect(DiagramNetwork.Bounds.X, DiagramNetwork.Bounds.Y,
			//    ScrollDiagram.ActualWidth * factor,	ScrollDiagram.ActualHeight * factor);
			//if (newBounds.Bottom > DiagramNetwork.Bounds.Bottom ||
			//    newBounds.Right > DiagramNetwork.Bounds.Right)
			//    DiagramNetwork.Bounds = newBounds;
		}

		/// <summary>
		/// Экпорт диаграммы в один из типов
		/// </summary>
		/// <param name="type">Тип для экспорта</param>
		public void ExportDiagram(ExportType type)
		{
			//SaveFileDialog saveDialog = new SaveFileDialog();
			//saveDialog.Filter = type + " файлы (*." + type + ")|*." + type;
			//bool? result = saveDialog.ShowDialog();
			//if (result == true)
			//switch (type)
			//{

			//    case ExportType.pdf:
			//        PdfExporter pdfExporter = new PdfExporter();
			//        pdfExporter.Export(DiagramNetwork, saveDialog.FileName);
			//        break;
			//    case ExportType.svg:
			//        SvgExporter svgExporter = new SvgExporter();
			//        svgExporter.Export(DiagramNetwork, saveDialog.FileName);
			//        break;
			//}
		}

		private void AssignNetwork()
		{
			DispatcherPanel.SetNetwork(_project.Network);
			this.NetworkVisual.IsEnabled = _project.Network != null;
			if (_project.Network != null)
			{
				_project.Network.Edges.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Edges_CollectionChanged);
				_project.Network.Nodes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Nodes_CollectionChanged);
			}

			RefreshFrame();
		}

		void Nodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (_lockNodeUpdate == true)
				return;

			_lockNodeUpdate = true;

			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach (NetworkNode node in e.OldItems)
				{
					DiagramNode digramNode = Utils.FindItemByUserData(NetworkVisual, node.Id) as DiagramNode;
					if (digramNode != null)
						NetworkVisual.Nodes.Remove(digramNode);
				}
			}

			_lockNodeUpdate = false;
		}

		void Edges_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (_lockEdgeUpdate == true)
				return;

			_lockEdgeUpdate = true;

			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach (NetworkEdge edge in e.OldItems)
				{
					DiagramEdge diagramEdge = Utils.FindItemByUserData(NetworkVisual, edge.Id) as DiagramEdge;
					if (diagramEdge != null)
						NetworkVisual.Edges.Remove(diagramEdge);
				}
			}

			_lockEdgeUpdate = false;
		}

		protected void RefreshFrame()
		{
			string caption = GetCaption();
			this.Title = caption;
		}

		private string GetCaption()
		{
			
			if (_project.Network == null)
			{
				return Properties.Resources.AppName;
			}

			string result = _project.Path;
			result += " - " + Properties.Resources.AppName;

			return result;
		}

		private void ShowErrorMessage(string message)
		{
			MessageBox.Show(message, Properties.Resources.ErrorMsgCaption, MessageBoxButton.OK,
						MessageBoxImage.Error);
		}

		protected bool DoClose()
		{
			if (_project != null && _project.Network != null && _project.Network.IsDirty)
			{
				MessageBoxResult result = MessageBox.Show(Properties.Resources.SaveWarning,
					Properties.Resources.Confirmation,
					MessageBoxButton.YesNoCancel);
				//стандартный мес. бокс некрасивый. По уму надо бы свой написать
				if (result == MessageBoxResult.Cancel)
				{
					return false;
				}
				if (result == MessageBoxResult.Yes)
				{
					this.Save_Executed(null, null);
				}
			}
			_project.Network = null;
			_project.Path = "";
			AssignNetwork();
			NetworkVisual.ClearAll();
			return true;
		}

		protected void DoZoom(bool direction)
		{
			if (direction)
			{
				NetworkVisual.ZoomIn();
			}
			else
			{
				NetworkVisual.ZoomOut();
			}
		}

		protected void ApplyLayout()
		{
			using (DiagramUpdateLock locker = new DiagramUpdateLock(NetworkVisual))
			{
				_project.ArrangeVisual();
			}
		}

		#endregion

		#region ContextMenus

		private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
		{
			MenuItem linkTypes = sender as MenuItem;
			if (linkTypes == null)
				return;

			foreach (MenuItem item in linkTypes.Items)
			{
				MenuItemUpdate(item);
			}
		}

		private void MenuItemUpdate(MenuItem item)
		{
			NetworkEdgeType type = Utils.ConvertType(item.Header.ToString());
			NetworkEdge edge = GetNetworkObjectFromMenuItem(item) as NetworkEdge;
			if (edge != null)
			{
				if (edge.Type == type)
					item.IsChecked = true;
				else item.IsChecked = false;
			}
		}

		private void MenuItemType_Click(object sender, RoutedEventArgs e)
		{
			MenuItem source = e.Source as MenuItem;
			if (source == null)
				return;

			MenuItem category = source.Parent as MenuItem;
			foreach (MenuItem item in category.Items)
			{
				item.IsChecked = false;
			}

			source.IsChecked = true;

			NetworkEdgeType type = Utils.ConvertType(source.Header.ToString());

			NetworkEdge edge = GetNetworkObjectFromMenuItem(source) as NetworkEdge;
			if (edge != null)
			{
				try
				{
					edge.Type = type;
					_currentType = type;
				}
				catch (Exception ex)
				{
					ShowErrorMessage(ex.Message);
					
				}
			}
		}

		private void MenuItemDeleteLink_Click(object sender, RoutedEventArgs e)
		{
			MenuItem source = e.Source as MenuItem;
			if (source == null)
				return;

			ContextMenuDeleteDiagramItem(source);
		}

		private void MenuItemNodeChangeText_Click(object sender, RoutedEventArgs e)
		{
			MenuItem source = e.Source as MenuItem;
			if (source == null)
				return;

			ContextMenu menu = source.Parent as ContextMenu;
			if (menu == null)
				return;
			//DiagramNode node = menu.PlacementTarget as DiagramNode;
			//if (node == null)
			//    return;

			//StartEdit(node);

		}

		private void MenuItemNodeDelete_Click(object sender, RoutedEventArgs e)
		{
			MenuItem source = e.Source as MenuItem;
			if (source == null)
				return;

			ContextMenuDeleteDiagramItem(source);
		}

		private NetworkObject GetNetworkObjectFromMenuItem(MenuItem item)
		{
			return null;
			//MenuItem category = item.Parent as MenuItem;
			//NetworkEdgeType type = Utils.ConvertType(item.Header.ToString());

			//ContextMenu menu = category.Parent as ContextMenu;
			//if (menu == null)
			//    return null;
			//DiagramEdge link = menu.PlacementTarget as DiagramEdge;
			//if (link == null)
			//    return null;
			//int id = Convert.ToInt32(link.UserData);
			//NetworkObject obj = _project.Network.Edges.FindById(id);
			//if (obj == null)
			//    obj = _project.Network.Nodes.FindById(id);
			//return obj;
		}

		private void ContextMenuDeleteDiagramItem(MenuItem source)
		{
			ContextMenu menu = source.Parent as ContextMenu;
			if (menu == null)
				return;
			//DiagramItem diagramItem = menu.PlacementTarget as DiagramItem;
			//if (diagramItem == null)
			//    return;

			//DiagramNetwork.Items.Remove(diagramItem);
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			if (item == null)
				return;

			ContextMenu menu = item.Parent as ContextMenu;
			if (menu == null)
				return;

			Diagram diagram = menu.PlacementTarget as Diagram;
			if (diagram == null)
				return;

			Point mousePos = Mouse.GetPosition(diagram);

			//подгоним позицию
			mousePos.X += diagram.Viewport.X;
			mousePos.Y += diagram.Viewport.Y;

			//ShapeNode node = diagram.Factory.CreateShapeNode(mousePos, new Size(Project.DefaultNodeWidth, Project.DefaultNodeHeight));
			//DiagramNetwork_NodeCreated(null, new NodeEventArgs(node));
		}

		#endregion

	}
}
