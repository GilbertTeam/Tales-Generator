﻿using System;
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

			XElement xDiagZoom = new XElement("ZoomFactor");
			xDiagZoom.Add(Convert.ToString(_diagram.ZoomFactor));
			xEl.Add(xDiagZoom);
			
			XElement xNodes = new XElement("Nodes");
			foreach (ShapeNode node in _diagram.Nodes)
			{
				XElement xNode = new XElement("ShapeNode");
				xNode.Add(new XAttribute("Id", node.Uid));

				XElement xBounds = new XElement("Bounds");
				Utils.SaveRectToXElement(node.Bounds, xBounds);
				xNode.Add(xBounds);

				xNodes.Add(xNode);
			}
			xEl.Add(xNodes);

			XElement xLinks = new XElement("Links");
			foreach (DiagramLink link in _diagram.Links)
			{
				XElement xLink = new XElement("DiagramLink");
				xLink.Add(new XAttribute("Id", link.Uid));

				XElement xPoints = new XElement("Points");
				foreach (Point point in link.ControlPoints)
				{
					XElement xPoint = new XElement("Point");
					Utils.SavePointToXElement(point, xPoint);
					xPoints.Add(xPoint);
				}

				xLink.Add(xPoints);

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
			_diagram.ZoomFactor = Convert.ToDouble(xEl.Element("ZoomFactor").Value);

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
				int linkId = Convert.ToInt32(xLink.Attribute("Id").Value);
				NetworkEdge edge = network.Edges.FindById(linkId);

				ShapeNode origin = Utils.FindNodeByUid(_diagram, edge.StartNode.Id);
				ShapeNode destination = Utils.FindNodeByUid(_diagram, edge.EndNode.Id);

				DiagramLink link = _diagram.Factory.CreateDiagramLink(origin, destination);
				link.Uid = linkId.ToString();

				link.ControlPoints.Clear();

				XElement xPoints = xLink.Element("Points");
				foreach (XElement xPoint in xPoints.Elements("Point"))
				{
					Point point = Utils.LoadPointFromXElement(xPoint);
					link.ControlPoints.Add(point);
				}

				link.UpdateFromPoints();
				link.HeadShape = ArrowHeads.PointerArrow;

				Binding binding = new Binding();
				binding.Path = new PropertyPath("Type");
				binding.Converter = new NetworkEdgeTypeStringConverter();
				binding.Source = edge;
				binding.ConverterParameter = link;
				binding.Mode = BindingMode.TwoWay;
				link.SetBinding(DiagramLink.TextProperty, binding);
			}
		}
	}
}
