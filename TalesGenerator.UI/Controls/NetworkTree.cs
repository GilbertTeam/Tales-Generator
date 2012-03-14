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
				style = _generic["TreeHeaderObjectsStyle"] as Style;
				_nodeObjects.Style = style;
				this.Items.Add(_nodeObjects);

				//Вершины
				_nodeNodes = new TreeViewItem();
				_nodeNodes.Header = Properties.Resources.TreeNodes;
				style = _generic["TreeHeaderNodesStyle"] as Style;
				_nodeNodes.Style = style;
				_nodeObjects.Items.Add(_nodeNodes);

				//Связи
				_nodeLinks = new TreeViewItem();
				_nodeLinks.Header = Properties.Resources.TreeLinks;
				style = _generic["TreeHeaderLinksStyle"] as Style;
				_nodeLinks.Style = style;
				_nodeObjects.Items.Add(_nodeLinks);
			}

			DataTemplate template;

			_nodeNodes.ItemsSource = _network.Nodes;
			template = _generic["TreeItemNodeTemplate"] as DataTemplate;
			_nodeNodes.ItemTemplate = template;

			template = _generic["TreeItemLinkTemplate"] as DataTemplate;
			_nodeLinks.ItemsSource = _network.Edges;
			_nodeLinks.ItemTemplate = template;

			_nodeLinks.IsExpanded = true;
			_nodeNodes.IsExpanded = true;
			_nodeObjects.IsExpanded = true;
		}

		#endregion

		#region EventHandlers

		#endregion

	}
}
