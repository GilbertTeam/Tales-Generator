using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel;
using System.Xml;

using TalesGenerator.Core;
using MindFusion.Diagramming.Wpf;
using System.Xml.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using TalesGenerator.UI.Controls;


namespace TalesGenerator.UI.Classes
{
	class Project : INotifyPropertyChanged
	{
		#region Fields

		Diagram _diagram;

		Network _network;

		string _path;

		public event PropertyChangedEventHandler PropertyChanged;

		LinkContextMenu _linkMenu;

		NodeContextMenu _nodeMenu;

		#endregion

		#region Contructors

		public Project()
		{
			_diagram = null;
			_network = null;
			_path = "";

			_linkMenu = new LinkContextMenu();
			_nodeMenu = new NodeContextMenu();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Диагрмма текущего проекта
		/// </summary>
		public Diagram Diagram
		{
			get { return _diagram; }
			set 
			{
				if (value == null)
					throw new ArgumentNullException("Diagram");
				if (_diagram != value)
				{
					_diagram = value;

					OnPropertyChanged("Diagram");
				}
			}

		}

		/// <summary>
		/// Модель текущего проекта
		/// </summary>
		public Network Network
		{
			get { return _network; }
			set 
			{
				if (_network != value)
				{
					_network = value;

					_linkMenu.Network = Network;
					_nodeMenu.Network = Network;

					OnPropertyChanged("Network");
				}
			}
		}

		/// <summary>
		/// Путь текущего проекта
		/// </summary>
		public string Path
		{
			get { return _path; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("Path");
				if (_path != value)
				{
					_path = value;

					OnPropertyChanged("Path");
				}
			}
		}

		#endregion

		#region Methods

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Новый проект
		/// </summary>
		public void NewProject()
		{
			Diagram = new Diagram();
			Network = new Network();
			Path = "";

			_linkMenu.Network = Network;
			_nodeMenu.Network = Network;
		}

		/// <summary>
		/// Сохранить проект
		/// </summary>
		public void Save()
		{
			CheckObjects();

			XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
			XElement xEl = new XElement("TalesGeneratorProject");
			xDoc.AddFirst(xEl);
			xEl.Add(_network.SaveToXml());
			DiagramSerializer diagSr = new DiagramSerializer(Diagram);
			diagSr.SaveToXDocument(xDoc);
			xDoc.Save(_path);
		}

		/// <summary>
		/// Загрузить проект
		/// </summary>
		public void Load()
		{

			if (Path == "")
				throw new ArgumentException("Path");
			XDocument xDoc = XDocument.Load(_path);

			_network = Network.LoadFromXml(xDoc);

			_linkMenu.Network = Network;
			_nodeMenu.Network = Network;

			DiagramSerializer diagSr = new DiagramSerializer(_diagram);
			diagSr.NodeAdded += new DiagramItemEventHandler(NodeAdded);
			diagSr.LinkAdded += new DiagramItemEventHandler(LinkAdded);
			diagSr.LoadFromXDocument(xDoc, _network);
		}

		public void NodeAdded(DiagramItem item, NetworkObject obj)
		{
			NetworkNode node = obj as NetworkNode;
			if (node == null)
				return;

			Binding binding = new Binding();
			binding.Path = new PropertyPath("Name");
			binding.Source = node;
			item.SetBinding(DiagramItem.TextProperty, binding);

			//newNode.SetBinding(DiagramItem.TextProperty, binding);
			item.Uid = node.Id.ToString();
			item.ContextMenu = _nodeMenu;

			item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);
		}

		void item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				DiagramNode node = sender as DiagramNode;

				StartEdit(node);

				e.Handled = true;
			}
		}

		public void StartEdit(DiagramNode node)
		{
			Network network = this.Network;
			if (network == null)
				return;

			NetworkNode netNode = network.Nodes.FindById(Int32.Parse(node.Uid));

			DiagramNodeEx nodeEx = new DiagramNodeEx(node, netNode);

			Diagram.BeginEdit(nodeEx);
		}


		public void LinkAdded(DiagramItem item, NetworkObject obj)
		{
			DiagramLink link = item as DiagramLink;
			if (link == null)
				return;

			NetworkEdge edge = obj as NetworkEdge;
			if (edge == null)
				return;

			link.HeadShape = ArrowHeads.PointerArrow;

			Binding binding = new Binding();
			binding.Path = new PropertyPath("Type");
			binding.Converter = new NetworkEdgeTypeStringConverter();
			binding.Source = edge;
			binding.ConverterParameter = link;
			binding.Mode = BindingMode.TwoWay;
			link.SetBinding(DiagramLink.TextProperty, binding);

			link.Uid = edge.Id.ToString();
			link.ContextMenu = _linkMenu;
			link.HeadShape = ArrowHeads.PointerArrow;
		}

		private void CheckObjects()
		{
			if (Path == "")
				throw new ArgumentException("Path");
			if (Diagram == null)
				throw new ArgumentNullException("Diagram");
			if (Network == null)
				throw new ArgumentNullException("Network");
		}

		#endregion
	}
}
