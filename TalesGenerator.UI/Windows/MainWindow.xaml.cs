﻿using System;
using System.Collections.Generic;
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
using TalesGenerator.UI.Classes;
using TalesGenerator.UI.Properties;

using MindFusion.Diagramming.Wpf;
using MindFusion.Diagramming.Wpf.Export;
using TalesGenerator.UI.Controls;

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

		#endregion

		#region Constructors

		public MainWindow()
		{
			InitializeComponent();

			_project = new Project();
			_project.Diagram = DiagramNetwork;
			_project.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_project_PropertyChanged);

			DiagramNetwork.IsEnabled = _project.Network != null;
			//ButtonViewShowPropsPanel.IsChecked = this.PanelProps.Visibility == Visibility.Visible;
			//DispatcherPanelButton.IsChecked = this.DispatcherPanel.Visibility == System.Windows.Visibility.Visible;

			DispatcherPanel.SelectionChanged += new OnSelectionChanged(DispatcherPanel_SelectionChanged);
			_updatingDiagramSelection = false;

			_currentType = NetworkEdgeType.IsA;

			AssignNetwork();
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
			_project.Network = new Network();
			DiagramNetwork.Items.Clear();
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
			openDialog.Filter = "TalesGeneratorProject files (*.tgp)|*.tgp";
			bool? result = openDialog.ShowDialog();
			if (result == true)
			{
				_project.Path = openDialog.FileName;
				_project.Load();
				if (_project.Network != null)
				{
					DiagramNetwork.IsEnabled = true;
				}
				AssignNetwork();
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

		#endregion

		#region Links Category

		private void ChooseLinkType_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			bool canExecute = DiagramNetwork != null && DiagramNetwork.IsEnabled;
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
				if (DiagramNetwork.Selection.Items.Count == 1 &&
					(DiagramNetwork.Selection.Items[0] as DiagramLink != null))
				{
					DiagramLink link = DiagramNetwork.Selection.Items[0] as DiagramLink;
					NetworkEdge edge = _project.Network.Edges.FindById(Convert.ToInt32(link.Uid));
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
				DiagramNetwork != null && DiagramNetwork.Selection.Items.Count == 1 &&
				DiagramNetwork.Selection.Items[0] as DiagramLink != null;
		}

		private void DeleteLink_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DiagramNetwork.Items.Remove(DiagramNetwork.Selection.Items[0]);
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

		#endregion

		#region Nodes category

		private void RenameNode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute =_project != null && _project.Network!= null &&
				DiagramNetwork != null && DiagramNetwork.Selection.Items.Count == 1 &&
				DiagramNetwork.Selection.Items[0] as ShapeNode != null;
		}

		private void RenameNode_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ShapeNode node = DiagramNetwork.Selection.Items[0] as ShapeNode;
			if (node == null)
				return;

			NetworkNode netNode = _project.Network.Nodes.FindById(Int32.Parse(node.Uid));
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
				DiagramNetwork != null && DiagramNetwork.Selection.Items.Count == 1 &&
				DiagramNetwork.Selection.Items[0] as ShapeNode != null;
		}

		private void DeleteNode_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DiagramNetwork.Items.Remove(DiagramNetwork.Selection.Items[0]);
		}

		#endregion

		#region NonRibbon Commands

		private void StartEdit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _project != null && _project.Network != null &&
				DiagramNetwork != null && DiagramNetwork.Selection.Items.Count == 1 &&
				DiagramNetwork.Selection.Items[0] as ShapeNode != null;
		}

		private void StartEdit_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ShapeNode node = DiagramNetwork.Selection.Items[0] as ShapeNode;
			if (node == null)
				return;

			_project.StartEdit(node);
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

		private void DiagramNetwork_NodeCreated(object sender, NodeEventArgs e)
		{
			ShapeNode newNode = e.Node as ShapeNode;
			if (newNode == null)
				return;

			Network network = _project.Network;
			NetworkNode netNode = network.Nodes.Add();

			_project.NodeAdded(newNode, netNode);

		}

		private void DiagramNetwork_NodeDeleted(object sender, NodeEventArgs e)
		{
			ShapeNode deletedNode = e.Node as ShapeNode;
			if (deletedNode == null)
				return;

			Network network = _project.Network;
			if (network == null)
				return;

			NetworkNode netNode = network.Nodes.FindById(Int32.Parse(deletedNode.Uid));
			if (netNode != null)
				network.Nodes.Remove(netNode);

		}

		private void DiagramNetwork_NodeSelected(object sender, NodeEventArgs e)
		{
			if (DiagramNetwork.Selection.Items.Count > 1)
				return;

			ShapeNode selectedNode = e.Node as ShapeNode;
			if (selectedNode == null)
				return;

			Network network = _project.Network;
			if (network == null)
				return;

			NetworkNode netNode = network.Nodes.FindById(Int32.Parse(selectedNode.Uid));
			//PanelProps.Node = netNode;

			//netNode.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(netNode_PropertyChanged);

		}

		//void netNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		//{
		//    ShapeNode mfNode = DiagramNetwork.Selection.Items[0] as ShapeNode;
		//    if (mfNode == null)
		//        return;

		//    Network network = _project.Network;
		//    NetworkNode node = network.Nodes.FindById(Int32.Parse(mfNode.Uid));

		//    if (e.PropertyName == "Name")
		//    {
		//        mfNode.Text = node.Name;
		//    }
		//}

		private void DiagramNetwork_NodeDeselected(object sender, NodeEventArgs e)
		{
			ShapeNode selectedNode = e.Node as ShapeNode;
			if (selectedNode == null)
				return;

			Network network = _project.Network;
			if (network == null)
				return;

			NetworkNode netNode = network.Nodes.FindById(Int32.Parse(selectedNode.Uid));

			//if (netNode == PanelProps.Node)
			//    PanelProps.Node = null;

			//if (netNode != null)
			//    netNode.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(netNode_PropertyChanged);
		}

		private void DiagramNetwork_SelectionChanged(object sender, EventArgs e)
		{
			if (_updatingDiagramSelection)
				return;
			_updatingDiagramSelection = true;
			if (DiagramNetwork.Selection.Items.Count == 0)
			{
				DispatcherPanel.SetSelection(-1);
				_updatingDiagramSelection = false;
				return;
			}
			DiagramItem item = DiagramNetwork.Selection.Items[0];
			int id = Convert.ToInt32(item.Uid);
			DispatcherPanel.SetSelection(id);
			_updatingDiagramSelection = false;
		}

		#endregion

		#region Link events

		private void DiagramNetwork_LinkCreated(object sender, LinkEventArgs e)
		{
			DiagramLink link = e.Link;
			if (link == null)
				return;

			Network network = _project.Network;
			if (network == null)
				return;

			NetworkNode origin = network.Nodes.FindById(Int32.Parse(link.Origin.Uid));
			NetworkNode destination = network.Nodes.FindById(Int32.Parse(link.Destination.Uid));

			try
			{
				NetworkEdge edge = network.Edges.Add(origin, destination, _currentType);
				//link.Uid = edge.Id.ToString();
				//link.Text = Utils.ConvertType(edge.Type);
				////yay, the king has returned!
				//Binding binding = new Binding();
				//binding.Path = new PropertyPath("Type");
				//binding.Converter = new NetworkEdgeTypeStringConverter();
				//binding.Source = edge;
				//binding.Mode = BindingMode.TwoWay;
				//binding.NotifyOnSourceUpdated = true;
				//binding.NotifyOnTargetUpdated = true;
				//binding.ConverterParameter = link;
				//link.SetBinding(DiagramLink.TextProperty, binding);
				_project.LinkAdded(link, edge);

				
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				DiagramNetwork.Links.Remove(e.Link);
			}
		}

		private void DiagramNetwork_LinkDeleted(object sender, LinkEventArgs e)
		{
			DiagramLink link = e.Link;
			if (link == null)
				return;

			Network network = _project.Network;
			if (network == null)
				return;

			if (link.Uid != "")
			{
				NetworkEdge edge = network.Edges.FindById(Int32.Parse(link.Uid));
				if (edge != null)
					network.Edges.Remove(edge);
			}
		}

		private void DiagramNetwork_LinkSelected(object sender, LinkEventArgs e)
		{
			if (DiagramNetwork.Selection.Items.Count > 1)
				return;
			
			DiagramLink link = e.Link;
			if (link == null)
				return;

			Network network = _project.Network;
			if (network == null)
				return;

			NetworkEdge edge = network.Edges.FindById(Int32.Parse(link.Uid));
			//PanelProps.Edge = edge;
			if (edge == null)
				return;

			_currentType = edge.Type;

			//edge.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(edge_PropertyChanged);

		}

		private void DiagramNetwork_LinkDeselected(object sender, LinkEventArgs e)
		{
			DiagramLink link = e.Link;
			if (link == null)
				return;


			Network network = _project.Network;
			if (network == null)
				return;

			NetworkEdge edge = network.Edges.FindById(Int32.Parse(link.Uid));

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

		#endregion

		#region ScrollDiagram events

		private void ScrollDiagram_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				DiagramItemCollection selectedItems = DiagramNetwork.Selection.Items;
				for (int count = selectedItems.Count, i = count - 1; i >= 0; i--)
				{
					DiagramItem item = selectedItems[i];
					if (item as DiagramNode != null) DiagramNetwork.Nodes.Remove(item as DiagramNode);
					else DiagramNetwork.Links.Remove(item as DiagramLink);

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
			_updatingDiagramSelection = true;
			this.DiagramNetwork.Selection.Clear();
			DiagramItem item = Utils.FindItemByUid(DiagramNetwork, id);
			item.Selected = true;
			DiagramNetwork.ScrollTo(new Point(item.GetBounds().X - DiagramNetwork.Viewport.Width / 2,
				item.GetBounds().Y - DiagramNetwork.Viewport.Height / 2));
			_updatingDiagramSelection = false;
		}

		#endregion

		#region Window methods

		/// <summary>
		/// Ресайз диаграммы
		/// </summary>
		protected void ResizeDiagram()
		{
			double factor = 100 / DiagramNetwork.ZoomFactor;
			Rect newBounds = new Rect(DiagramNetwork.Bounds.X, DiagramNetwork.Bounds.Y,
				ScrollDiagram.ActualWidth * factor,	ScrollDiagram.ActualHeight * factor);
			if (newBounds.Bottom > DiagramNetwork.Bounds.Bottom ||
				newBounds.Right > DiagramNetwork.Bounds.Right)
				DiagramNetwork.Bounds = newBounds;
		}

		/// <summary>
		/// Экпорт диаграммы в один из типов
		/// </summary>
		/// <param name="type">Тип для экспорта</param>
		public void ExportDiagram(ExportType type)
		{
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.Filter = type + " файлы (*." + type + ")|*." + type;
			bool? result = saveDialog.ShowDialog();
			if (result == true)
			switch (type)
			{

				case ExportType.pdf:
					PdfExporter pdfExporter = new PdfExporter();
					pdfExporter.Export(DiagramNetwork, saveDialog.FileName);
					break;
				case ExportType.svg:
					SvgExporter svgExporter = new SvgExporter();
					svgExporter.Export(DiagramNetwork, saveDialog.FileName);
					break;
			}
		}

		private void AssignNetwork()
		{
			DispatcherPanel.SetNetwork(_project.Network);
			this.DiagramNetwork.IsEnabled = _project.Network != null;
			if (_project.Network != null)
			{
				_project.Network.Edges.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Edges_CollectionChanged);
				_project.Network.Nodes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Nodes_CollectionChanged);
			}

			RefreshFrame();
		}

		void Nodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach (NetworkNode node in e.OldItems)
				{
					DiagramNode digramNode = Utils.FindItemByUid(DiagramNetwork, node.Id) as DiagramNode;
					if (digramNode != null)
						DiagramNetwork.Items.Remove(digramNode);
				}
			}
		}

		void Edges_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				foreach (NetworkEdge node in e.OldItems)
				{
					DiagramLink digramNode = Utils.FindItemByUid(DiagramNetwork, node.Id) as DiagramLink;
					if (digramNode != null)
						DiagramNetwork.Items.Remove(digramNode);
				}
			}
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
			DiagramNetwork.ClearAll();
			return true;
		}

		private void StartEdit(DiagramNode node)
		{
			Network network = _project.Network;
			if (network == null)
				return;

			NetworkNode netNode = network.Nodes.FindById(Int32.Parse(node.Uid));

			DiagramNodeEx nodeEx = new DiagramNodeEx(node, netNode);

			DiagramNetwork.BeginEdit(nodeEx);
		}

		protected void DoZoom(bool direction)
		{
			try
			{
				if (direction)
				{
					DiagramNetwork.ZoomOut();
					ResizeDiagram();
				}
				else DiagramNetwork.ZoomIn();
			}
			catch { }
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
			DiagramNode node = menu.PlacementTarget as DiagramNode;
			if (node == null)
				return;

			StartEdit(node);

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
			MenuItem category = item.Parent as MenuItem;
			NetworkEdgeType type = Utils.ConvertType(item.Header.ToString());

			ContextMenu menu = category.Parent as ContextMenu;
			if (menu == null)
				return null;
			DiagramLink link = menu.PlacementTarget as DiagramLink;
			if (link == null)
				return null;
			int id = Convert.ToInt32(link.Uid);
			NetworkObject obj = _project.Network.Edges.FindById(id);
			if (obj == null)
				obj = _project.Network.Nodes.FindById(id);
			return obj;
		}

		private void ContextMenuDeleteDiagramItem(MenuItem source)
		{
			ContextMenu menu = source.Parent as ContextMenu;
			if (menu == null)
				return;
			DiagramItem diagramItem = menu.PlacementTarget as DiagramItem;
			if (diagramItem == null)
				return;

			DiagramNetwork.Items.Remove(diagramItem);
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

			ShapeNode node = diagram.Factory.CreateShapeNode(mousePos, new Size(Project.DefaultNodeWidth, Project.DefaultNodeHeight));
			DiagramNetwork_NodeCreated(null, new NodeEventArgs(node));
		}

		#endregion

	}
}
