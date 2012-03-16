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

using TalesGenerator.Core;
using TalesGenerator.Core.Collections;
using TalesGenerator.UI.Classes;
using TalesGenerator.UI.Properties;
using System.ComponentModel;
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

		ResourceDictionary _generic;

		Network _network;

		TreeViewItem _nodeObjects;
		TreeViewItem _nodeNodes;
		TreeViewItem _nodeLinks;

		#endregion

		#region Contructors

		public NetworkTree()
			: base()
		{
			_network = null;
			_nodeObjects = null;
			_nodeNodes = null;
			_nodeLinks = null;

			_generic = new ResourceDictionary();
			_generic.Source =
				new Uri("/TalesGenerator;component/Themes/Generic.xaml",
					UriKind.RelativeOrAbsolute);
		}

		#endregion

		#region Properties

		public Network CurrentNetwork
		{
			get { return _network; }
			set
			{
				_network = value;
				RefreshTree();
			}
		}

		#endregion

		#region Methods

		private void RefreshTree()
		{
			if (_network == null)
			{
				this.Items.Clear();
				_nodeObjects = _nodeLinks = _nodeNodes = null;
				return;
			}

			RefreshNodes();
		}

		private void RefreshNodes()
		{
			if (_nodeObjects == null)
			{
				Style style;
				//Создаем основные вершины
				//Объекты
				_nodeObjects = new TreeViewItem();
				_nodeObjects.Header = Properties.Resources.TreeObjects;
				style = this.FindResource("TreeHeaderObjectsStyle") as Style;
				_nodeObjects.Style = style;
				this.Items.Add(_nodeObjects);

				//Вершины
				_nodeNodes = new TreeViewItem();
				_nodeNodes.Header = Properties.Resources.TreeNodes;
				style = this.FindResource("TreeHeaderNodesStyle") as Style;
				_nodeNodes.Style = style;
				_nodeObjects.Items.Add(_nodeNodes);
				_nodeNodes.ContextMenu = FindResource("NodeContextMenuKey") as ContextMenu;

				//Связи
				_nodeLinks = new TreeViewItem();
				_nodeLinks.Header = Properties.Resources.TreeLinks;
				style = this.FindResource("TreeHeaderLinksStyle") as Style;
				_nodeLinks.Style = style;
				_nodeObjects.Items.Add(_nodeLinks);
			}

			DataTemplate template;

			_nodeNodes.ItemsSource = _network.Nodes;
			template = this.FindResource("TreeItemNodeTemplate") as DataTemplate;
			_nodeNodes.ItemTemplate = template;

			template = this.FindResource("TreeItemLinkTemplate") as DataTemplate;
			_nodeLinks.ItemsSource = _network.Edges;
			_nodeLinks.ItemTemplate = template;

			_nodeLinks.IsExpanded = true;
			_nodeNodes.IsExpanded = true;
			_nodeObjects.IsExpanded = true;
		}

		//private ObservableCollection<NetworkObjectTreeViewItem> CreateCollectionFromNodes(NetworkNodeCollection networkNodeCollection)
		//{
		//    ObservableCollection<NetworkObjectTreeViewItem> collection = new ObservableCollection<NetworkObjectTreeViewItem>();

		//    foreach (NetworkNode node in networkNodeCollection)
		//    {
		//        NetworkObjectTreeViewItem item = new NetworkObjectTreeViewItem();
		//        item.IsExpanded = false;
		//        item.IsSelected = false;
		//        item.NetObject = node;
		//    }

		//    return collection;
		//}

		//private ObservableCollection<NetworkObjectTreeViewItem> CreateCollectionFromEdges(NetworkEdgeCollection networkNodeCollection)
		//{
		//    ObservableCollection<NetworkObjectTreeViewItem> collection = new ObservableCollection<NetworkObjectTreeViewItem>();

		//    foreach (NetworkEdge edge in networkNodeCollection)
		//    {
		//        NetworkObjectTreeViewItem item = new NetworkObjectTreeViewItem();
		//        item.IsExpanded = false;
		//        item.IsSelected = false;
		//        item.NetObject = edge;
		//    }

		//    return collection;
		//}

		public TreeViewItem FindNode(int id)
		{
			if (_network == null)
				return null;

			foreach (object item in _nodeLinks.Items)
			{
				NetworkEdge edge = item as NetworkEdge;
				if (edge != null && edge.Id == id)
				{
					return _nodeLinks.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
				}
			}

			foreach (object item in _nodeNodes.Items)
			{
				NetworkNode node = item as NetworkNode;
				if (node != null && node.Id == id)
				{
					object obj = _nodeNodes.ItemContainerGenerator.ContainerFromItem(item);
					return obj as TreeViewItem;
				}
			}

			return null;
		}

		public void ClearSelection()
		{
			if (_nodeLinks != null)
			{
				foreach (object item in _nodeLinks.Items)
				{
					TreeViewItem treeItem = _nodeLinks.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
					if (treeItem != null) treeItem.IsSelected = false;
				}
			}

			if (_nodeNodes != null)
			{
				foreach (object item in _nodeNodes.Items)
				{
					TreeViewItem treeItem = _nodeNodes.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
					if (treeItem != null) treeItem.IsSelected = false;
				}
			}
		}

		#endregion

		#region EventHandlers

		#endregion
	}

	//public class NetworkObjectTreeViewItem : INotifyPropertyChanged
	//{
	//    public event PropertyChangedEventHandler PropertyChanged;

	//    private bool _isSelected;
	//    private bool _isExpanded;
	//    private NetworkObject _netObject;

	//    public bool IsExpanded
	//    {
	//        get { return _isExpanded; }
	//        set
	//        {
	//            _isExpanded = value;
	//            this.PropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
	//        }
	//    }

	//    public bool IsSelected
	//    {
	//        get { return _isSelected; }
	//        set
	//        {
	//            _isSelected = value;
	//            this.PropertyChanged(this, new PropertyChangedEventArgs("IsSelected"));
	//        }
	//    }

	//    public NetworkObject NetObject
	//    {
	//        get { return _netObject; }
	//        set
	//        {
	//            _netObject = value;
	//            this.PropertyChanged(this, new PropertyChangedEventArgs("NetObject"));
	//        }
	//    }

	//}
}
