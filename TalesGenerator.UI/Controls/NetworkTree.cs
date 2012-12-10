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
using TalesGenerator.Net.Collections;
using TalesGenerator.UI.Classes;
using TalesGenerator.UI.Properties;
using System.ComponentModel;
using System.Globalization;
using System.Collections.ObjectModel;

namespace TalesGenerator.UI.Controls
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:TalesGenerator.UI.Controls"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:TalesGenerator.UI.Controls;assembly=TalesGenerator.UI.Controls"
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
	///     <MyNamespace:CustomControl1/>
	///
	/// </summary>
	public class NetworkTree : TreeView
	{
		static NetworkTree()
		{
			//DefaultStyleKeyProperty.OverrideMetadata(typeof(NetworkTree), new FrameworkPropertyMetadata(typeof(NetworkTree)));
		}


		#region Fields

		ResourceDictionary _dictionary;

		List<TreeItemInfo> _infos;

		Network _network;

		LinkContextMenu _linkMenu;

		NodeContextMenu _nodeMenu;

		#endregion

		#region Contructors

		public NetworkTree()
			: base()
		{
			_network = null;

			_dictionary = Application.LoadComponent(new Uri("/TalesGenerator;component/Themes/Generic.xaml", 
				UriKind.Relative)) as ResourceDictionary;

			_infos = new List<TreeItemInfo>();
			_linkMenu = new LinkContextMenu();
			_nodeMenu = new NodeContextMenu();

			InUpdate = false;
		}

		#endregion

		#region Properties

		public bool InUpdate { get; set; }

		public Network CurrentNetwork
		{
			get { return _network; }
			set
			{
				_network = value;
				RefreshTree();
				if (_network != null)
				{
					_linkMenu.Network = _network;
					_nodeMenu.Network = _network;
					_network.Nodes.CollectionChanged += new NotifyCollectionChangedEventHandler(Nodes_CollectionChanged);
					_network.Edges.CollectionChanged += new NotifyCollectionChangedEventHandler(Edges_CollectionChanged);
					_network.Edges.PropertyChanged += new PropertyChangedEventHandler(Edges_PropertyChanged);
				}
			}
		}

		void Edges_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RefreshTree();
		}

		void Edges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RefreshTree();
		}

		void Nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RefreshTree();
		}

		#endregion

		#region Methods

		private void RefreshTree()
		{
			InUpdate = true;

			if (_network == null)
			{
				this.Items.Clear();
				return;
			}
			SaveTreeState();
			this.Items.Clear();
			CreateNodes();
			_infos.Clear();
			InUpdate = false;
		}

		private void SaveTreeState()
		{
			foreach (TreeViewItem treeItem in Items)
			{
				SaveItemState(treeItem);
			}
		}

		private void SaveItemState(TreeViewItem item)
		{
			TreeItemInfo state = new TreeItemInfo();
			state.IsExpanded = item.IsExpanded;
			state.IsSelected = item.IsSelected;

			if (item.Uid != "")
			{
				state.Id = Convert.ToInt32(item.Uid);
				TreeViewItem parent = item.Parent as TreeViewItem;
				if (parent.Uid != "")
				{
					state.Parent = FindParent(parent.Uid);
				}
				else
				{
					if (parent.Header as string == Properties.Resources.TreeObjects)
					{
						state.Parent = null;
					}
					else
					{
						parent = parent.Parent as TreeViewItem;
						if (parent.Uid != "")
						{
							state.Parent = FindParent(parent.Uid);
						}
					}
				}
			}
			else
			{
				state.Name = item.Header as string;
				if (state.Name != Properties.Resources.TreeObjects)
				{
					TreeViewItem parent = item.Parent as TreeViewItem;
					if (parent.Uid != "")
					{

						state.Parent = FindParent(parent.Uid);
					}
				}
				else
				{
					state.Parent = null;
				}
			}

			_infos.Add(state);

			foreach (TreeViewItem treeItem in item.Items)
			{
				SaveItemState(treeItem);
			}
		}

		private NetworkObject FindParent(string uid)
		{
			int parentId = Convert.ToInt32(uid);
			NetworkObject obj = _network.Nodes.FindById(parentId);
			if (obj == null)
				obj = _network.Edges.FindById(parentId);
			return obj;
		}

		private TreeItemInfo FindItemInfo(int id, NetworkObject parent, string name = "")
		{
			TreeItemInfo info = null;

			foreach (TreeItemInfo itemInfo in _infos)
			{
				if (itemInfo.Parent == parent && (itemInfo.Id == id || id == -1 && itemInfo.Name == name))
				{
					info = itemInfo;
					break;
				}
			}

			return info;
		}

		private void LoadItemState(int id, NetworkObject node, TreeViewItem nodeRoot, string name = "")
		{
			TreeItemInfo info = FindItemInfo(id, node, name);

			if (info != null)
			{
				//nodeRoot.IsSelected = info.IsSelected;
				nodeRoot.IsExpanded = info.IsExpanded;
			}
		}

		private void CreateNodes()
		{
			//Строим дерево
			//Корень
			TreeViewItem rootNode = new TreeViewItem();
			rootNode.Header = Properties.Resources.TreeObjects;
			rootNode.Focusable = false;
			rootNode.Style = _dictionary["TreeHeaderObjectsStyle"] as Style;

			LoadItemState(-1, null, rootNode, Properties.Resources.TreeObjects);

			this.Items.Add(rootNode);

			if (_network == null)
				return;

			string str = Utils.ConvertToResourcesType(NetworkEdgeType.IsA) + "ItemTemplateKey";
			DataTemplate itemTemplate = Application.Current.TryFindResource(str) as DataTemplate;

			List<NetworkNode> primaryNodes = new List<NetworkNode>(_network.GetPrimaryNodes());
			foreach (NetworkNode node in primaryNodes)
			{
				TreeViewItem nodeTreeItem = CreateNodeItem(node, itemTemplate);

				LoadItemState(node.Id, null, nodeTreeItem);
				CreateDescendants(nodeTreeItem, node);

				rootNode.Items.Add(nodeTreeItem);
			}

		}

		private void CreateDescendants(TreeViewItem root, NetworkNode node)
		{
			//Связи
			NetworkEdgeType currentType;
			//TreeViewItem linksRoot = new TreeViewItem();
			//linksRoot.Header = "Связи";
			//root.Items.Add(linksRoot);
			TreeViewItem linksRoot = root;
			//Agent
			CreateTypedDescendant(linksRoot, node, NetworkEdgeType.Agent);
			//Recipient
			CreateTypedDescendant(linksRoot, node, NetworkEdgeType.Recipient);
			//Goal
			CreateTypedDescendant(linksRoot, node, NetworkEdgeType.Goal);
			//Follow
			CreateTypedDescendant(linksRoot, node, NetworkEdgeType.Follow);
			//Locative
			CreateTypedDescendant(linksRoot, node, NetworkEdgeType.Locative);
			//Экземплярыэ
			CreateInstances(root, node);
			//Is-a потомки
			CreateDescendantsNodes(root, node);

		}

		private void CreateDescendantsNodes(TreeViewItem root, NetworkNode node)
		{
			if (node.HasTypedIncomingEdges(NetworkEdgeType.IsA))
			{
				string str = Utils.ConvertToResourcesType(NetworkEdgeType.IsA) + "TemplateKey";
				DataTemplate template = Application.Current.TryFindResource(str) as DataTemplate;

				str = Utils.ConvertToResourcesType(NetworkEdgeType.IsA) + "ItemTemplateKey";
				DataTemplate itemTemplate = Application.Current.TryFindResource(str) as DataTemplate;

				TreeViewItem instanceRoot = new TreeViewItem();
				instanceRoot.Header = Properties.Resources.DescendantsLabel;
				instanceRoot.Focusable = false;
				instanceRoot.HeaderTemplate = template;

				LoadItemState(-1, node, instanceRoot, Properties.Resources.DescendantsLabel);

				root.Items.Add(instanceRoot);

				foreach (NetworkEdge edge in node.GetTypedIncomingEdges(NetworkEdgeType.IsA))
				{
					NetworkNode endNode = edge.StartNode;
					TreeViewItem nodeItem = CreateNodeItem(endNode, itemTemplate);

					LoadItemState(endNode.Id, node, nodeItem);
					CreateDescendants(nodeItem, endNode);

					instanceRoot.Items.Add(nodeItem);
				}
			}
		}

		private void CreateInstances(TreeViewItem root, NetworkNode node)
		{
			if (node.HasTypedIncomingEdges(NetworkEdgeType.IsInstance))
			{
				string str = Utils.ConvertToResourcesType(NetworkEdgeType.IsInstance) + "TemplateKey";
				DataTemplate template = Application.Current.TryFindResource(str) as DataTemplate;

				str = Utils.ConvertToResourcesType(NetworkEdgeType.IsInstance) + "ItemTemplateKey";
				DataTemplate itemTemplate = Application.Current.TryFindResource(str) as DataTemplate;

				TreeViewItem instanceRoot = new TreeViewItem();
				instanceRoot.Header = Properties.Resources.InstancesLabel;
				instanceRoot.Focusable = false;
				instanceRoot.HeaderTemplate = template;

				LoadItemState(-1, node, instanceRoot, Properties.Resources.InstancesLabel);

				root.Items.Add(instanceRoot);

				foreach (NetworkEdge edge in node.GetTypedIncomingEdges(NetworkEdgeType.IsInstance))
				{
					NetworkNode endNode = edge.StartNode;
					TreeViewItem nodeRoot = CreateNodeItem(endNode, itemTemplate);

					LoadItemState(endNode.Id, node, nodeRoot);
					CreateDescendants(nodeRoot, endNode);

					instanceRoot.Items.Add(nodeRoot);
				}

			}
		}

		private void CreateTypedDescendant(TreeViewItem root, NetworkNode node, NetworkEdgeType currentType)
		{
			NetworkEdge edge = node.GetTypedOutgoingEdge(currentType);
			if (edge != null)
			{
				string str = Utils.ConvertToResourcesType(currentType) + "TemplateKey";
				DataTemplate template = Application.Current.TryFindResource(str) as DataTemplate;
				str = Utils.ConvertToResourcesType(currentType) + "ItemTemplateKey";
				DataTemplate itemTemplate = Application.Current.TryFindResource(str) as DataTemplate;

				TreeViewItem linkRoot = new TreeViewItem();
				linkRoot.Header = Utils.ConvertType(edge.Type);
				linkRoot.HeaderTemplate = template;
				linkRoot.ContextMenu = _linkMenu;
				linkRoot.Uid = edge.Id.ToString();

				LoadItemState(edge.Id, node, linkRoot);

				root.Items.Add(linkRoot);

				NetworkNode nodeDesc = edge.EndNode;
				TreeViewItem nodeTreeItem = CreateNodeItem(nodeDesc, itemTemplate);

				LoadItemState(nodeDesc.Id, edge, nodeTreeItem);

				linkRoot.Items.Add(nodeTreeItem);
			}
		}

		private TreeViewItem CreateNodeItem(NetworkNode endNode, DataTemplate itemTemplate)
		{
			TreeViewItem nodeRoot = new TreeViewItem();
			Binding nodeBinding = new Binding();
			nodeBinding.Source = endNode;
			nodeBinding.Path = new PropertyPath("Name");
			nodeBinding.Converter = new NodeNameConverter();
			nodeRoot.SetBinding(TreeViewItem.HeaderProperty, nodeBinding);
			nodeRoot.Uid = endNode.Id.ToString(CultureInfo.InvariantCulture);
			nodeRoot.ContextMenu = _nodeMenu;
			nodeRoot.HeaderTemplate = itemTemplate;
			return nodeRoot;
		}

		public TreeViewItem FindNode(int id, TreeViewItem item = null)
		{
			TreeViewItem result = null;

			if (_network == null)
				return null;

			if (item == null)
			{
				item = this.Items[0] as TreeViewItem;
			}

			foreach (TreeViewItem treeItem in item.Items)
			{
				if (treeItem.Uid != "")
				{
					int itemId = Convert.ToInt32(treeItem.Uid);
					if (itemId == id)
						result = treeItem;
					else
					{
						if (!ItemInForbiddenTypes(treeItem.Header as string))
							result = FindNode(id, treeItem);
					}

				}
				else
				{
					string header = treeItem.Header as string;
					if (header == Properties.Resources.InstancesLabel ||
						header == Properties.Resources.DescendantsLabel)
					{
						result = FindNode(id, treeItem);
					}
				}
				if (result != null)
					break;
			}

			return result;


			//foreach (object item in _nodeLinks.Items)
			//{
			//    NetworkEdge edge = item as NetworkEdge;
			//    if (edge != null && edge.Id == id)
			//    {
			//        return _nodeLinks.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
			//    }
			//}

			//foreach (object item in _nodeNodes.Items)
			//{
			//    NetworkNode node = item as NetworkNode;
			//    if (node != null && node.Id == id)
			//    {
			//        object obj = _nodeNodes.ItemContainerGenerator.ContainerFromItem(item);
			//        return obj as TreeViewItem;
			//    }
			//}
		}

		private bool ItemInForbiddenTypes(string p)
		{
			return p == Properties.Resources.AgentLabel || p == Properties.Resources.RecipientLabel ||
				p == Properties.Resources.GoalLabel || p == Properties.Resources.FollowLabel ||
				p == Properties.Resources.LocativeLabel;
		}

		public void ClearSelection()
		{
			//if (_nodeLinks != null)
			//{
			//    foreach (object item in _nodeLinks.Items)
			//    {
			//        TreeViewItem treeItem = _nodeLinks.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
			//        if (treeItem != null) treeItem.IsSelected = false;
			//    }
			//}

			//if (_nodeNodes != null)
			//{
			//    foreach (object item in _nodeNodes.Items)
			//    {
			//        TreeViewItem treeItem = _nodeNodes.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
			//        if (treeItem != null) treeItem.IsSelected = false;
			//    }
			//}
		}

		#endregion

		#region EventHandlers

		#endregion
	}

	internal class TreeItemInfo
	{
		public int Id { get; set; }
		public NetworkObject Parent { get; set; }
		public bool IsExpanded { get; set; }
		public bool IsSelected { get; set; }
		public string Name { get; set; }
	}
}
