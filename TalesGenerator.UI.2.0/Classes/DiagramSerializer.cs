using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

using TalesGenerator.Net;

//using MindFusion.Diagramming.Wpf;
//using MindFusion.Diagramming.Wpf.Layout;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using Gt.Controls;
using Gt.Controls.Diagramming;

namespace TalesGenerator.UI.Classes
{
	class DiagramSerializer
	{
		#region Fields

		Diagram _diagram;

		public event DiagramItemEventHandler NodeAdded;

		public event DiagramItemEventHandler LinkAdded;

		public event NotifyEventHandler DiagramLoaded;

		const int _version = 2;

		#endregion

		#region Constructors

		public DiagramSerializer(Diagram diagram)
		{
			if (diagram == null)
				throw new ArgumentNullException("Diagram");

			_diagram = diagram;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Сохраняет диаграмму в XElement
		/// </summary>
		/// <param name="xDoc"></param>
		public void SaveToXDocument(XDocument xDoc)
		{

			Gt.Controls.Diagramming.DiagramSerializer serializer = new Gt.Controls.Diagramming.DiagramSerializer(_diagram);
			serializer.SaveToXDocument(xDoc);
		}

		public void LoadFromXDocument(XDocument xDoc, Network network)
		{
			Gt.Controls.Diagramming.DiagramSerializer serializer =
				new Gt.Controls.Diagramming.DiagramSerializer(_diagram);
			XElement xDiagram = xDoc.Root != null ? xDoc.Root.Element("Gt.Diagram") : null;
			if (xDiagram != null)
			{
				serializer.LoadFromXDocument(xDoc);
				RaiseDiagramLoaded();
			}
			else
			{
				using (DiagramUpdateLock updateLock = new DiagramUpdateLock(_diagram))
				{
					CreateVisual(network);

					ArrangeVisual();
				}
			}
		}

		protected void CreateVisual(Network network)
		{
			foreach (var netNode in network.Nodes)
			{
				DiagramNode node = new DiagramNode(_diagram);
				RaiseNodeAdded(node, netNode);
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

			foreach (var netEdge in network.Edges)
			{
				var origin = Utils.FindItemByUserData(_diagram, netEdge.StartNode.Id) as DiagramNode;
				var destination = Utils.FindItemByUserData(_diagram, netEdge.EndNode.Id) as DiagramNode;

				var edge = new DiagramEdge(_diagram);
				edge.AnchoringMode = EdgeAnchoringMode.NodeToNode;
				edge.SourceNode = origin;
				edge.DestinationNode = destination;

				RaiseLinkAdded(edge, netEdge);
			}
		}

		protected void ArrangeVisual()
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

				MindFusion.Diagramming.Wpf.ShapeNode source = FindMfNode(mfDiagram, edge.SourceNode.UserData);
				MindFusion.Diagramming.Wpf.ShapeNode dest = FindMfNode(mfDiagram, edge.DestinationNode.UserData);


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

		private MindFusion.Diagramming.Wpf.ShapeNode FindMfNode(MindFusion.Diagramming.Wpf.Diagram mfDiagram, string p)
		{
			foreach (MindFusion.Diagramming.Wpf.ShapeNode node in mfDiagram.Nodes)
			{
				if (node.Uid == p)
				{
					return node;
				}
			}
			return null;
		}

		protected void RaiseNodeAdded(DiagramNode node, NetworkObject obj)
		{
			if (NodeAdded != null)
				NodeAdded(node, obj);
		}

		protected void RaiseLinkAdded(DiagramEdge link, NetworkObject obj)
		{
			if (LinkAdded != null)
				LinkAdded(link, obj);
		}

		protected void RaiseDiagramLoaded()
		{
			if (DiagramLoaded != null)
				DiagramLoaded();
		}

		#endregion
	}
}
