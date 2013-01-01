using System;
using System.ComponentModel;

using TalesGenerator.Net;
using TalesGenerator.TaleNet;
using System.Xml.Linq;
using System.Windows;

using TalesGenerator.UI.Controls;

using Gt.Controls.Diagramming;


namespace TalesGenerator.UI.Classes
{
	class Project : INotifyPropertyChanged
	{
		#region Fields

		Diagram _diagram;

		TalesNetwork _network;

		string _path;

		public event PropertyChangedEventHandler PropertyChanged;

		LinkContextMenu _linkMenu;

		NodeContextMenu _nodeMenu;

		public const int DefaultNodeWidth = 50;

		public const int DefaultNodeHeight = 50;

		public event DiagramItemEventHandler EdgeCreated;

		public event BoolNotifyEventHander LockEdgeUpdate;

		public event BoolNotifyEventHander LockNodeUpdate;

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
		public TalesNetwork Network
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
			Network = new TalesNetwork();
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

			_network = new TalesNetwork();
			_network.LoadFromXml(xDoc);

			_linkMenu.Network = Network;
			_nodeMenu.Network = Network;

			DiagramSerializer diagSr = new DiagramSerializer(_diagram);
			diagSr.NodeAdded += new DiagramItemEventHandler(NodeAdded);
			diagSr.LinkAdded += new DiagramItemEventHandler(LinkAdded);
			diagSr.DiagramLoaded += new NotifyEventHandler(UpdateDiagramStyle);
			diagSr.NeedBuildDiagram += new NotifyEventHandler(RebuildDiagram);
			diagSr.LoadFromXDocument(xDoc, _network);
		}

		void UpdateDiagramStyle()
		{
			using (DiagramUpdateLock locker = new DiagramUpdateLock(_diagram))
			{
				foreach (var node in _diagram.Nodes)
				{
					var networkNode = _network.FindById(Convert.ToInt32(node.UserData)) as NetworkNode;
					if (networkNode != null)
					{
						NodeAdded(node, networkNode);
					}
				}

				foreach (var edge in _diagram.Edges)
				{
					var networkEdge = _network.Edges.FindById(Convert.ToInt32(edge.UserData)) as NetworkEdge;
					if (networkEdge != null)
					{
						LinkAdded(edge, networkEdge);
					}
				}
			}
		}

		public void NodeAdded(DiagramItem item, NetworkObject obj)
		{
			var node = item as DiagramNode;
			if (node == null)
				return;

			NetworkNode networkNode = obj as NetworkNode;
			if (networkNode == null)
				return;

			node.Label.Text = networkNode.Name;
			node.UserData = networkNode.Id.ToString();
			Utils.UpdateNodeStyle(node);

			node.Label.TextChanged += new LabelEventHandler(OnDiagramTextChanged);
			networkNode.PropertyChanged += new PropertyChangedEventHandler(OnNetworkNodePropertyChanged);
		}

		void OnDiagramTextChanged(LabelEventArgs args)
		{
			DiagramLabel label = args.Label;
			if (label == null)
				return;

			DiagramNode node = label.Owner as DiagramNode;
			if (node == null)
				return;

			NetworkNode netNode = _network.FindById(Int32.Parse(node.UserData)) as NetworkNode;
			if (netNode == null)
				return;

			netNode.Name = label.Text;
		}

		void OnNetworkNodePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName.ToLower())
			{
				case "name":
					OnNetworkNodeTextChanged(sender as NetworkNode);
					break;
			}
		}

		void OnNetworkNodeTextChanged(NetworkNode netNode)
		{
			if (netNode == null)
				return;

			DiagramNode node = Utils.FindItemByUserData(_diagram, netNode.Id) as DiagramNode;
			if (node == null)
				return;

			node.Label.Text = netNode.Name;
		}


		public void LinkAdded(DiagramItem item, NetworkObject obj)
		{
			var edge = item as DiagramEdge;
			if (edge == null)
				return;

			NetworkEdge networkEdge = obj as NetworkEdge;
			if (networkEdge == null)
				return;

			Utils.UpdateEdgeStyleFromType(edge, networkEdge.Type);
			edge.UserData = networkEdge.Id.ToString();

			networkEdge.PropertyChanged += new PropertyChangedEventHandler(OnNetworkEdgePropertyChanged);
		}

		public void SubscribeOnEdgeEvents(DiagramEdge edge)
		{
			if (edge == null)
				return;

			edge.SourceNodeChanged -= new EdgeEventHandler(OnSourceNodeChanged);
			edge.SourceNodeChanged += new EdgeEventHandler(OnSourceNodeChanged);
			edge.DestinationNodeChanged -= new EdgeEventHandler(OnDestinationNodeChanged);
			edge.DestinationNodeChanged += new EdgeEventHandler(OnDestinationNodeChanged);
			edge.AnchoringModeChanged -= new EdgeEventHandler(OnAnchoringModeChanged);
			edge.AnchoringModeChanged += new EdgeEventHandler(OnAnchoringModeChanged);
		}

		void OnSourceNodeChanged(EdgeEventArgs args)
		{
			UpdateNetworkEdge(args.Edge);
		}

		void OnDestinationNodeChanged(EdgeEventArgs args)
		{
			UpdateNetworkEdge(args.Edge);
		}

		void OnAnchoringModeChanged(EdgeEventArgs args)
		{
			UpdateNetworkEdge(args.Edge);
		}

		private void UpdateNetworkEdge(DiagramEdge edge)
		{
			RaiseLockEdgeUpdate(true);

			//delete old
			NetworkEdge networkEdge = _network.Edges.FindById(Convert.ToInt32(edge.UserData));
			if (networkEdge != null)
			{
				_network.Edges.Remove(networkEdge);
			}

			//create new
			RaiseEdgeCreated(edge);

			RaiseLockEdgeUpdate(false);
		}

		void OnNetworkEdgePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName.ToLower())
			{
				case "type":
					OnNetworkEdgeTypeChanged(sender as NetworkEdge);
					break;
			}
		}

		private void OnNetworkEdgeTypeChanged(NetworkEdge networkEdge)
		{
			if (networkEdge == null)
				return;

			DiagramEdge edge = Utils.FindItemByUserData(_diagram, networkEdge.Id) as DiagramEdge;
			if (edge == null)
				return;

			Utils.UpdateEdgeStyleFromType(edge, networkEdge.Type);
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

		private void RaiseEdgeCreated(DiagramEdge edge)
		{
			if (EdgeCreated != null)
			{
				EdgeCreated(edge, null);
			}
		}

		private void RaiseLockEdgeUpdate(bool value)
		{
			if (LockEdgeUpdate != null)
			{
				LockEdgeUpdate(value);
			}
		}

		private void RaiseLockNodeUpdate(bool value)
		{
			if (LockNodeUpdate != null)
			{
				LockNodeUpdate(value);
			}
		}

		public void RebuildDiagram()
		{
			_diagram.ClearAll();

			using (DiagramUpdateLock updateLock = new DiagramUpdateLock(_diagram))
			{
				CreateVisual();

				ArrangeVisual();
			}
		}

		public void CreateVisual()
		{
			if (_network == null)
				return;

			RaiseLockNodeUpdate(true);

			foreach (var netNode in _network.Nodes)
			{
				DiagramNode node = new DiagramNode(_diagram);
				NodeAdded(node, netNode);
				node.Bounds = node.Label.Drawer.CalculateGeometry(node.Label).Bounds;
				if (node.Bounds.Width == 0 || node.Bounds.Height == 0)
				{
					node.Bounds = new Rect(0, 0, 25, 25);
				}
				else
				{
					Rect nodeBounds = node.Bounds;
					nodeBounds.Width = nodeBounds.Width + 20;
					nodeBounds.Height = nodeBounds.Height + 12;
					node.Bounds = nodeBounds;
				}
			}

			RaiseLockNodeUpdate(false);

			RaiseLockEdgeUpdate(true);

			foreach (var netEdge in _network.Edges)
			{
				var origin = Utils.FindItemByUserData(_diagram, netEdge.StartNode.Id) as DiagramNode;
				var destination = Utils.FindItemByUserData(_diagram, netEdge.EndNode.Id) as DiagramNode;

				var edge = new DiagramEdge(_diagram);
				edge.AnchoringMode = EdgeAnchoringMode.NodeToNode;
				edge.SourceNode = origin;
				edge.DestinationNode = destination;

				LinkAdded(edge, netEdge);
			}

			RaiseLockEdgeUpdate(false);
		}

		public void ArrangeVisual()
		{
			// хаки остались :)
			MindFusion.Diagramming.Wpf.Diagram mfDiagram = new MindFusion.Diagramming.Wpf.Diagram();

			foreach (var node in _diagram.Nodes)
			{
				MindFusion.Diagramming.Wpf.ShapeNode shapeNode = new MindFusion.Diagramming.Wpf.ShapeNode(mfDiagram);
				shapeNode.Bounds = node.Bounds;
				shapeNode.Uid = node.UserData;

				mfDiagram.Nodes.Add(shapeNode);
			}

			foreach (var edge in _diagram.Edges)
			{
				if (edge.SourceNode == null || edge.DestinationNode == null)
					continue;

				MindFusion.Diagramming.Wpf.ShapeNode source = Utils.FindMfNode(mfDiagram, edge.SourceNode.UserData);
				MindFusion.Diagramming.Wpf.ShapeNode dest = Utils.FindMfNode(mfDiagram, edge.DestinationNode.UserData);


				mfDiagram.Links.Add(mfDiagram.Factory.CreateDiagramLink(source, dest));

			}
			MindFusion.Diagramming.Wpf.Layout.Layout layout = new MindFusion.Diagramming.Wpf.Layout.FractalLayout();

			layout.Arrange(mfDiagram);

			mfDiagram.ResizeToFitItems(50); // TODO create constant somewhere

			//копируем обратно :)
			foreach (MindFusion.Diagramming.Wpf.ShapeNode shapeNode in mfDiagram.Nodes)
			{
				DiagramNode node = Utils.FindItemByUserData(_diagram, Convert.ToInt32(shapeNode.Uid)) as DiagramNode;
				if (node == null)
					continue;

				node.Bounds = shapeNode.Bounds;
			}
		}

		#endregion
	}
}
