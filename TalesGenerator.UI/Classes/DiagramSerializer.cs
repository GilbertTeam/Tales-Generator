using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using TalesGenerator.Core;

using MindFusion.Diagramming.Wpf;
using System.Windows;
using System.Windows.Data;

namespace TalesGenerator.UI.Classes
{
	class DiagramSerializer
	{
		Diagram _diagram;

		public DiagramSerializer(Diagram diagram)
		{
			if (diagram == null)
				throw new ArgumentNullException("Diagram");

			_diagram = diagram;
		}

		/// <summary>
		/// Сохраняет диаграмму в XElement
		/// </summary>
		/// <param name="xDoc"></param>
		public void SaveToXDocument(XDocument xDoc)
		{
			XElement xDiagram = new XElement("Diagram");
			this.SaveToXElement(xDiagram);
			if (xDoc.Root != null)
				xDoc.Root.Add(xDiagram);
			else xDiagram.AddFirst(xDiagram);
		}

		/// <summary>
		/// Сохранение диаграммы в элемент
		/// </summary>
		/// <param name="xEl"></param>
		public void SaveToXElement(XElement xEl)
		{
			XElement xDiagBounds = new XElement("Bounds");
			Utils.SaveRectToXElement(_diagram.Bounds, xDiagBounds);
			xEl.Add(xDiagBounds);
			
			XElement xNodes = new XElement("Nodes");
			foreach (ShapeNode node in _diagram.Nodes)
			{
				XElement xNode = new XElement("ShapeNode");
				xNode.Add(new XAttribute("Id", node.Uid));

				XElement xBounds = new XElement("Bounds");
				Utils.SaveRectToXElement(node.Bounds, xBounds);
				xNode.Add(xBounds);

				//XElement xText = new XElement("Text");
				//xText.Add(node.Text);
				//xNode.Add(xText);

				xNodes.Add(xNode);
			}
			xEl.Add(xNodes);

			XElement xLinks = new XElement("Links");
			foreach (DiagramLink link in _diagram.Links)
			{
				XElement xLink = new XElement("DiagramLink");
				xLink.Add(new XAttribute("Id", link.Uid));

				XElement xOriginId = new XElement("OriginId");
				xOriginId.Add(link.Origin.Uid);
				xLink.Add(xOriginId);

				XElement xDestinationId = new XElement("DestinationId");
				xDestinationId.Add(link.Destination.Uid);
				xLink.Add(xDestinationId);

				//XElement xText = new XElement("Text");
				//xText.Add(link.Text);
				//xLink.Add(xText);

				xLinks.Add(xLink);
			}
			xEl.Add(xLinks);
		}

		public void LoadFromXDocument(XDocument xDoc, Network network)
		{
			XElement xDiagram = xDoc.Root.Element("Diagram");
			LoadFromXElement(xDiagram, network);
		}

		public void LoadFromXElement(XElement xEl, Network network)
		{
			_diagram.Bounds = Utils.LoadRectFromXElement(xEl.Element("Bounds"));

			XElement xNodes = xEl.Element("Nodes");
			foreach (XElement xNode in xNodes.Elements("ShapeNode"))
			{
				Rect rect = Utils.LoadRectFromXElement(xNode.Element("Bounds"));
				ShapeNode node = _diagram.Factory.CreateShapeNode(rect);

				int id = Int32.Parse(xNode.Attribute("Id").Value);
				node.Uid = id.ToString();
				NetworkNode netNode = network.Nodes.FindById(id);

				Binding binding = new Binding();
				binding.Path = new PropertyPath("Name");
				binding.Source = netNode;
				node.SetBinding(DiagramItem.TextProperty, binding);
			}
			XElement xLinks = xEl.Element("Links");
			foreach (XElement xLink in xLinks.Elements("DiagramLink"))
			{
				int originId = Int32.Parse(xLink.Element("OriginId").Value);
				int destId = Int32.Parse(xLink.Element("DestinationId").Value);

				ShapeNode origin = Utils.FindNodeByUid(_diagram, originId);
				ShapeNode destination = Utils.FindNodeByUid(_diagram, destId);

				DiagramLink link = _diagram.Factory.CreateDiagramLink(origin, destination);
				int id = Int32.Parse(xLink.Attribute("Id").Value);
				link.Uid = id.ToString();

				NetworkEdge edge = network.Edges.FindById(id);
				Binding binding = new Binding();
				binding.Path = new PropertyPath("Type");
				binding.Converter = new NetworkEdgeTypeStringConverter();
				binding.Source = edge;
				binding.Mode = BindingMode.TwoWay;
				link.SetBinding(DiagramLink.TextProperty, binding);
			}
		}
	}
}
