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
using System.Windows.Navigation;
using System.Windows.Shapes;

using TalesGenerator.UI.Classes;
using TalesGenerator.Core;

namespace TalesGenerator.UI.Controls
{
	/// <summary>
	/// Interaction logic for CtrlObjectDispatcher.xaml
	/// </summary>
	public partial class CtrlObjectDispatcher : UserControl
	{
		#region Fields

		public event OnSelectionChanged SelectionChanged;

		#endregion

		#region Contructors

		public CtrlObjectDispatcher()
		{
			InitializeComponent();
		}

		#endregion

		#region Methods

		public void SetNetwork(Network network)
		{
			NetworkObjectsTree.CurrentNetwork = network;
		}

		public void SetSelection(int id)
		{
			if (NetworkObjectsTree.InUpdate)
				return;
			if (id == -1)
			{
				NetworkObjectsTree.ClearSelection();
			}
			else
			{
				TreeViewItem item = NetworkObjectsTree.FindNode(id);
				if (item != null)
				{
					item.IsSelected = true;
					TreeViewItem parent = item.Parent as TreeViewItem;
					while (parent != null)
					{
						parent.IsExpanded = true;
						parent = parent.Parent as TreeViewItem;
					}
				}
			}
		}

		#endregion

		#region Event Handlers

		private void NetworkObjectsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeViewItem item = e.NewValue as TreeViewItem;
			if (item == null)
				return;

			if (NetworkObjectsTree.CurrentNetwork != null && item.Uid != "")
			{
				int id = Convert.ToInt32(item.Uid);

				SelectionChanged(id);
			}
		}

		#endregion
	}
}
