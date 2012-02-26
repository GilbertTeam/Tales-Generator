using System;
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

using TalesGenerator.Core;
using TalesGenerator.UI.Classes;

using MindFusion.Diagramming.Wpf;

namespace TalesGenerator.UI.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow
	{
		Network _network;

		public MainWindow()
		{
			InitializeComponent();
			
			_network = null;

			ButtonViewShowPropsPanel.IsChecked = this.PanelShowProps.Visibility == Visibility.Visible;
		}

		#region CommandHandlers

		private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void New_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			_network = new Network();
		}

		private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _network != null;
		}

		private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _network != null;
		}

		private void Save_AsExecuted(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void CloseProject_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _network != null;
		}

		private void CloseProject_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}

		private void ShowProps_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void ShowProps_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.PanelShowProps.Visibility = this.PanelShowProps.Visibility == Visibility.Collapsed ?
				Visibility.Visible : Visibility.Collapsed;
		}

		#endregion

		#region MF events

		private void DiagramNetwork_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			int count = Math.Abs(e.Delta / 120);
			bool zoomOut = e.Delta < 0;
			for (int i = 0; i < count; i++)
			{
				if (zoomOut && DiagramNetwork.ZoomFactor != 0)
				{
					DiagramNetwork.ZoomOut();
					double factor = 100 / DiagramNetwork.ZoomFactor;
					Rect newBounds = new Rect(0, 0, ScrollDiagram.ActualWidth * factor,
						ScrollDiagram.ActualHeight * factor);
					if (newBounds.Bottom > DiagramNetwork.Bounds.Bottom ||
						newBounds.Right > DiagramNetwork.Bounds.Right)
						DiagramNetwork.Bounds = newBounds;
				}
				else DiagramNetwork.ZoomIn();
			}
			e.Handled = true;
		}

		private void RibbonWindow_KeyDown(object sender, KeyEventArgs e)
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

		private void DiagramNetwork_NodeSelected(object sender, NodeEventArgs e)
		{

		}

		private void DiagramNetwork_LinkSelected(object sender, LinkEventArgs e)
		{

		}

		private void DiagramNetwork_NodeDeleted(object sender, NodeEventArgs e)
		{

		}

		private void DiagramNetwork_LinkDeleted(object sender, LinkEventArgs e)
		{

		}

		#endregion

		private void DiagramNetwork_NodeCreated(object sender, NodeEventArgs e)
		{

		}

		private void DiagramNetwork_LinkCreated(object sender, LinkEventArgs e)
		{

		}

		private void DiagramNetwork_NodeDeselected(object sender, NodeEventArgs e)
		{

		}

		private void DiagramNetwork_LinkDeselected(object sender, LinkEventArgs e)
		{

		}

	}
}
