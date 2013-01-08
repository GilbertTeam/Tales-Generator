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
using TalesGenerator.TaleNet;

namespace TalesGenerator.UI.Controls
{
	/// <summary>
	/// Interaction logic for CtrlObjectDispatcher.xaml
	/// </summary>
	public partial class CtrlObjectDispatcher : UserControl
	{
		#region Fields

		public event IntNotifyEventHandler SelectionChanged;

		public event IntNotifyEventHandler SelectLinkedNodes;

		#endregion

		#region Contructors

		public CtrlObjectDispatcher()
		{
			InitializeComponent();

			NetworkObjectsTree.SelectLinkedNodes += new IntNotifyEventHandler(OnSelectLinkedNods);
		}

		#endregion

		#region Methods

		public void SetNetwork(TalesNetwork network)
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
					item.BringIntoView();
				}
			}
		}

		protected void RaiseSelectLinkedNodes(int id)
		{
			if (SelectLinkedNodes != null)
			{
				SelectLinkedNodes(id);
			}
		}

		#endregion

		#region Event Handlers

		private void NetworkObjectsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (NetworkObjectsTree.InUpdate)
				return;
			TreeViewItem item = e.NewValue as TreeViewItem;
			if (item == null)
				return;

			if (NetworkObjectsTree.CurrentNetwork != null && item.Uid != "")
			{
				int id = Convert.ToInt32(item.Uid);

				SelectionChanged(id);
			}
		}

		void OnSelectLinkedNods(int id)
		{
			RaiseSelectLinkedNodes(id);
		}

		#endregion
	}
}
