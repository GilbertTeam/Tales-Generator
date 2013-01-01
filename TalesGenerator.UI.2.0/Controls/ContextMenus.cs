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
using System.Windows.Navigation;
using System.Windows.Shapes;

using TalesGenerator.Net;
using TalesGenerator.TaleNet;
using TalesGenerator.TaleNet.Collections;
using TalesGenerator.UI.Classes;
using TalesGenerator.UI.Properties;
using System.ComponentModel;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using TalesGenerator.UI.Windows;

namespace TalesGenerator.UI.Controls
{
	class NetworkContextMenu : ContextMenu
	{
		#region Fields

		#endregion

		#region Contrsuctors

		public NetworkContextMenu(TalesNetwork network)
			: base()
		{
			Network = network;

			CreateMenu();
		}

		#endregion

		#region Properties

		public TalesNetwork Network { get; set; }

		#endregion

		#region Methods

		protected virtual void CreateMenu()
		{
		}

		#endregion
	}

	class LinkContextMenu : NetworkContextMenu
	{
		#region Contrsuctors

		public LinkContextMenu(TalesNetwork network = null) : base(network)
		{
		}

		#endregion

		#region Methods

		protected override void CreateMenu()
		{
			MenuItem linkTypeItem = new MenuItem();
			linkTypeItem.Header = Properties.Resources.LinkTypeLabel;
			linkTypeItem.SubmenuOpened += new RoutedEventHandler(linkTypeItem_SubmenuOpened);
			Items.Add(linkTypeItem);

			MenuItem isAItem = new MenuItem();
			isAItem.Header = Properties.Resources.IsALabel;
			isAItem.IsCheckable = true;
			isAItem.Click += new RoutedEventHandler(subMenuItem_Click);
			linkTypeItem.Items.Add(isAItem);

			MenuItem agentItem = new MenuItem();
			agentItem.Header = Properties.Resources.AgentLabel;
			agentItem.IsCheckable = true;
			agentItem.Click += new RoutedEventHandler(subMenuItem_Click);
			linkTypeItem.Items.Add(agentItem);

			MenuItem recipientItem = new MenuItem();
			recipientItem.Header = Properties.Resources.RecipientLabel;
			recipientItem.IsCheckable = true;
			recipientItem.Click += new RoutedEventHandler(subMenuItem_Click);
			linkTypeItem.Items.Add(recipientItem);

			MenuItem goalItem = new MenuItem();
			goalItem.Header = Properties.Resources.GoalLabel;
			goalItem.IsCheckable = true;
			goalItem.Click += new RoutedEventHandler(subMenuItem_Click);
			linkTypeItem.Items.Add(goalItem);

			MenuItem followItem = new MenuItem();
			followItem.Header = Properties.Resources.FollowLabel;
			followItem.IsCheckable = true;
			followItem.Click += new RoutedEventHandler(subMenuItem_Click);
			linkTypeItem.Items.Add(followItem);

			MenuItem locativeItem = new MenuItem();
			locativeItem.Header = Properties.Resources.LocativeLabel;
			locativeItem.IsCheckable = true;
			locativeItem.Click += new RoutedEventHandler(subMenuItem_Click);
			linkTypeItem.Items.Add(locativeItem);

			MenuItem isInstanceItem = new MenuItem();
			isInstanceItem.Header = Properties.Resources.IsInstanceLabel;
			isInstanceItem.IsCheckable = true;
			isInstanceItem.Click += new RoutedEventHandler(subMenuItem_Click);
			linkTypeItem.Items.Add(isInstanceItem);

			Separator separator = new Separator();
			Items.Add(separator);

			MenuItem deleteLinkitem = new MenuItem();
			deleteLinkitem.Header = Properties.Resources.DeleteLinkLabel;
			deleteLinkitem.Click += new RoutedEventHandler(deleteLinkitem_Click);
			Items.Add(deleteLinkitem);

		}

		#region EventHandlers

		void subMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem linkTypeItem = sender as MenuItem;
			if (linkTypeItem == null)
				return;

			NetworkEdgeType itemType = Utils.ConvertType(linkTypeItem.Header.ToString());

			NetworkEdge edge = GetNetworkEdge();
			if (edge == null)
				return;

			edge.Type = itemType;
		}

		void linkTypeItem_SubmenuOpened(object sender, RoutedEventArgs e)
		{
			MenuItem linkTypeItem = sender as MenuItem;
			if (linkTypeItem == null)
				return;

			NetworkEdge edge = GetNetworkEdge();

			if (edge == null)
				return;

			foreach (MenuItem item in linkTypeItem.Items)
			{
				MenuItemUpdate(item, edge.Type);
			}
		}

		private NetworkEdge GetNetworkEdge()
		{
			FrameworkElement element = this.PlacementTarget as FrameworkElement;

			if (element == null)
				return null;

			if (element.Uid == "")
				return null;

			if (Network == null)
				return null;

			int linkId = Convert.ToInt32(element.Uid);
			NetworkEdge edge = Network.FindById(linkId) as NetworkEdge;

			return edge;
		}

		private void MenuItemUpdate(MenuItem item, NetworkEdgeType type)
		{
			NetworkEdgeType itemType = Utils.ConvertType(item.Header.ToString());
			if (itemType == type)
				item.IsChecked = true;
			else item.IsChecked = false;
		}

		void deleteLinkitem_Click(object sender, RoutedEventArgs e)
		{
			NetworkEdge edge = GetNetworkEdge();
			if (edge != null)
				Network.Edges.Remove(edge);
		}

		#endregion

		#endregion
	}

	class NodeContextMenu : NetworkContextMenu
	{
		public event IntNotifyEventHandler SelectLinkedNodes;

		#region Contrsuctors

		public NodeContextMenu(TalesNetwork network = null) : base(network)
		{
		}

		#endregion

		#region Methods

		protected override void CreateMenu()
		{
			MenuItem selectLinkedNodesItem = new MenuItem();
			selectLinkedNodesItem.Header = "Выделить связанные вершины";
			selectLinkedNodesItem.Click += new RoutedEventHandler(selectLinkedNodesItem_Click);
			Items.Add(selectLinkedNodesItem);

			var separator = new Separator();
			Items.Add(separator);

			MenuItem changeNodeNameItem = new MenuItem();
			changeNodeNameItem.Header = Properties.Resources.ChangeNodeTextLabel;
			changeNodeNameItem.Click += new RoutedEventHandler(changeNodeNameItem_Click);
			Items.Add(changeNodeNameItem);

			separator = new Separator();
			Items.Add(separator);

			MenuItem deleteNodeItem = new MenuItem();
			deleteNodeItem.Header = Properties.Resources.DeleteNodeLabel;
			deleteNodeItem.Click += new RoutedEventHandler(deleteNodeItem_Click);
			Items.Add(deleteNodeItem);
		}

		#region EventHandlers

		void selectLinkedNodesItem_Click(object sender, RoutedEventArgs e)
		{
			NetworkNode node = GetNetworkNode();
			if (node == null)
				return;

			RaiseSelectLinkedNodes(node.Id);
		}

		void changeNodeNameItem_Click(object sender, RoutedEventArgs e)
		{
			NetworkNode node = GetNetworkNode();
			if (node == null)
				return;

			StringEditWindow edit = new StringEditWindow(node.Name);
			edit.ShowDialog();
			bool? res = edit.DialogResult;
			if (res == true)
			{
				node.Name = edit.Value;
			}
		}

		void deleteNodeItem_Click(object sender, RoutedEventArgs e)
		{
			NetworkNode node = GetNetworkNode();
			if (node == null)
				return;

			Network.Nodes.Remove(node);
		}

		private NetworkNode GetNetworkNode()
		{
			FrameworkElement element = this.PlacementTarget as FrameworkElement;

			if (element == null)
				return null;

			if (element.Uid == "")
				return null;

			if (Network == null)
				return null;

			int linkId = Convert.ToInt32(element.Uid);
			NetworkNode edge = Network.FindById(linkId) as NetworkNode;

			return edge;
		}

		#endregion

		protected void RaiseSelectLinkedNodes(int id)
		{
			if (SelectLinkedNodes != null)
			{
				SelectLinkedNodes(id);
			}
		}

		#endregion
	}
}
