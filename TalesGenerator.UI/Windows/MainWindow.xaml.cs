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

using TalesGenerator.Core;
using TalesGenerator.UI.Classes;
using TalesGenerator.UI.Properties;

using MindFusion.Diagramming.Wpf;
using MindFusion.Diagramming.Wpf.Export;

namespace TalesGenerator.UI.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow
	{
		Project _project;

		bool _updatingDiagramSelection;

		public MainWindow()
		{
			InitializeComponent();

			_project = new Project();
			_project.Diagram = DiagramNetwork;
			_project.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_project_PropertyChanged);

			DiagramNetwork.IsEnabled = _project.Network != null;
			ButtonViewShowPropsPanel.IsChecked = this.PanelProps.Visibility == Visibility.Visible;
			DispatcherPanelButton.IsChecked = this.DispatcherPanel.Visibility == System.Windows.Visibility.Visible;

			DispatcherPanel.SelectionChanged += new OnSelectionChanged(DispatcherPanel_SelectionChanged);
			_updatingDiagramSelection = false;

			AssignNetwork();
		}

		void _project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Network")
			{
				AssignNetwork();
			}
		}

		#region CommandHandlers

		private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void New_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			_project.Network = new Network();
			//Tree.CurrentNetwork = _project.Network;
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

		private void ShowProps_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void ShowProps_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.PanelProps.Visibility = this.PanelProps.Visibility == Visibility.Collapsed ?
				Visibility.Visible : Visibility.Collapsed;
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

		private void ShowDispatcher_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void ShowDispatcher_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DispatcherPanel.Visibility = DispatcherPanel.Visibility == System.Windows.Visibility.Visible ?
				Visibility.Collapsed : Visibility.Visible;
		}

		private void ChooseLinkType_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			bool canExecute =  DiagramNetwork.IsEnabled && DiagramNetwork.Selection.Items.Count == 1 &&
				(DiagramNetwork.Selection.Items[0] as DiagramLink != null);
			e.CanExecute = canExecute;
			LinkTypeButton.IsEnabled = canExecute;
		}

		private void StartConsult_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{

		}

		#endregion

		#region Window events

		private void RibbonWindow_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ResizeDiagram();
		}

		#endregion

		#region MF events

		private void DiagramNetwork_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			int count = Math.Abs(e.Delta / 120);
			bool zoomOut = e.Delta < 0;
			try
			{
				for (int i = 0; i < count; i++)
				{
					if (zoomOut)
					{
						DiagramNetwork.ZoomOut();
						ResizeDiagram();
					}
					else DiagramNetwork.ZoomIn();
				}
			}
			catch (Exception ex) { }
			e.Handled = true;
		}

		#region NodeEvents

		private void DiagramNetwork_NodeCreated(object sender, NodeEventArgs e)
		{
			ShapeNode newNode = e.Node as ShapeNode;
			if (newNode == null)
				return;

			Network network = _project.Network;
			NetworkNode netNode = network.Nodes.Add();
			newNode.Uid = netNode.Id.ToString();
			// биндинг
			Binding binding = new Binding();
			binding.Path = new PropertyPath("Name");
			binding.Source = netNode;
			newNode.SetBinding(DiagramItem.TextProperty, binding);
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
			PanelProps.Node = netNode;

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

			if (netNode == PanelProps.Node)
				PanelProps.Node = null;

			//if (netNode != null)
			//    netNode.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(netNode_PropertyChanged);
		}

		private void DiagramNetwork_SelectionChanged(object sender, EventArgs e)
		{
			if (_updatingDiagramSelection)
				return;
			if (DiagramNetwork.Selection.Items.Count == 0)
			{
				DispatcherPanel.SetSelection(-1);
				return;
			}
			DiagramItem item = DiagramNetwork.Selection.Items[0];
			int id = Convert.ToInt32(item.Uid);
			DispatcherPanel.SetSelection(id);
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
				NetworkEdge edge = network.Edges.Add(origin, destination);
				link.Uid = edge.Id.ToString();
				link.Text = Utils.ConvertType(edge.Type);
				//yay, the king has returned!
				Binding binding = new Binding();
				binding.Path = new PropertyPath("Type");
				binding.Converter = new NetworkEdgeTypeStringConverter();
				binding.Source = edge;
				binding.Mode = BindingMode.TwoWay;
				link.SetBinding(DiagramLink.TextProperty, binding);
			}
			catch (ArgumentException ex)
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
			PanelProps.Edge = edge;

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

			if (PanelProps.Edge == edge)
				PanelProps.Edge = null;
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
			Utils.FindItemByUid(DiagramNetwork, id).Selected = true;
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
			Rect newBounds = new Rect(0, 0, ScrollDiagram.ActualWidth * factor,
					ScrollDiagram.ActualHeight * factor);
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
			RefreshFrame();
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

		protected bool DoClose()
		{
			if (_project.Network.IsDirty)
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

		#endregion
	}
}
