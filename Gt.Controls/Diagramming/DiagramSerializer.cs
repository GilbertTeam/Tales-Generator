using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;
using System.Globalization;
using System.Windows;

namespace Gt.Controls.Diagramming
{
	public class DiagramSerializer : BaseSerializer
	{
		#region fields

		protected Diagram _diagram;

		#endregion

		#region constructors

		public DiagramSerializer(Diagram diagram)
		{
			_diagram = diagram;
		}

		#endregion

		#region methods

		public string SaveToString()
		{
			return "";
		}

		public void LoadFromString()
		{
		}

		public void SaveToXDocument(XDocument xDoc)
		{
			if (xDoc.Root == null)
			{
				return;
			}

			XElement xDiag = xDoc.Element("Gt.Diagram");
			if (xDiag == null)
			{
				xDiag = new XElement("Gt.Diagram");
				if (xDoc.Root == null)
					xDoc.Add(xDiag);
				else xDoc.Root.Add(xDiag);
			}

			SaveToXElement(xDiag);
		}

		public void LoadFromXDocument(XDocument xDoc)
		{
			XElement xDiag = xDoc.Element("Gt.Diagram");
			if (xDiag == null && xDoc.Root != null)
			{
				xDiag = xDoc.Root.Element("Gt.Diagram");
			}

			if (xDiag == null)
				return;

			using (DiagramUpdateLock locker = new DiagramUpdateLock(_diagram))
			{
				LoadFromXElement(xDiag);
			}
		}

		public void SaveToXElement(XElement xDiag)
		{
			if (_diagram == null)
				return;

			WriteElement(xDiag, "Scale", _diagram.Scale);
			WriteElement(xDiag, "ScaleFactor", _diagram.ScaleFactor);
			WriteElement(xDiag, "XViewOffset", _diagram.XViewOffset);
			WriteElement(xDiag, "YViewOffset", _diagram.YViewOffset);

			SaveNodesToXElement(xDiag);
			SaveEdgesToXElement(xDiag);
		}

		public void LoadFromXElement(XElement xDiag)
		{
			if (_diagram == null)
				return;

			double tmp = 0;
			if (ReadElement(xDiag, "Scale", out tmp))
			{
				_diagram.Scale = tmp;
			}

			if (ReadElement(xDiag, "ScaleFactor", out tmp))
			{
				_diagram.ScaleFactor = tmp;
			}

			if (ReadElement(xDiag, "XViewOffset", out tmp))
			{
				_diagram.XViewOffset = tmp;
			}

			if (ReadElement(xDiag, "YViewOffset", out tmp))
			{
				_diagram.YViewOffset = tmp;
			}

			XElement xNodes = xDiag.Element("Nodes");
			if (xNodes != null)
			{
				LoadNodesFromXElement(xNodes);
			}

			XElement xEdges = xDiag.Element("Edges");
			if (xEdges != null)
			{
				LoadEdgesFromXElement(xEdges);
			}
		}

		private void SaveNodesToXElement(XElement xDiag)
		{
			XElement xNodes = new XElement("Nodes");

			foreach (var node in _diagram.Nodes)
			{
				SaveNodeToXElement(xNodes, node);
			}

			xDiag.Add(xNodes);
		}

		private void LoadNodesFromXElement(XElement xNodes)
		{
			IEnumerable<XElement> xNodeList = xNodes.Elements("Node");
			foreach (var xNode in xNodeList)
			{
				DiagramNode node = new DiagramNode(_diagram);
				LoadNodeFromXElement(xNode, node);
			}

		}

		private void SaveNodeToXElement(XElement xNodes, DiagramNode node)
		{
			XElement xNode = new XElement("Node");

			WriteElement(xNode, "RadiusX", node.RadiusX);
			WriteElement(xNode, "RadiusY", node.RadiusY);
			WriteElement(xNode, "Bounds", node.Bounds);

			SaveItemPart(xNode, node);

			if (node.Label != null)
				SaveLabelToXElement(xNode, node.Label);

			xNodes.Add(xNode);
		}

		private void LoadNodeFromXElement(XElement xNode, DiagramNode node)
		{
			double tmp = 0;
			if (ReadElement(xNode, "RadiusX", out tmp))
			{
				node.RadiusX = tmp;
			}

			if (ReadElement(xNode, "RadiusY", out tmp))
			{
				node.RadiusY = tmp;
			}

			Rect bounds;
			if (ReadElement(xNode, "Bounds", out bounds))
			{
				node.Bounds = bounds;
			}

			LoadItemPart(xNode, node);

			XElement xLabel = xNode.Element("Label");
			if (xLabel != null)
			{
				LoadLabelFromXElement(xLabel, node.Label);
			}
		}

		private void SaveEdgesToXElement(XElement xDiag)
		{
			XElement xEdges = new XElement("Edges");

			foreach (var edge in _diagram.Edges)
			{
				SaveEdgeToXElement(xEdges, edge);
			}

			xDiag.Add(xEdges);
		}

		private void LoadEdgesFromXElement(XElement xEdges)
		{
			IEnumerable<XElement> xEdgeList = xEdges.Elements("Edge");

			foreach (var xEdge in xEdgeList)
			{
				DiagramEdge edge = new DiagramEdge(_diagram);
				LoadEdgeFromXElement(xEdge, edge);
			}
		}

		private void SaveEdgeToXElement(XElement xEdges, DiagramEdge edge)
		{
			XElement xEdge = new XElement("Edge");

			WriteElement(xEdge, "AnchoringMode", Enum.GetName(typeof(EdgeAnchoringMode), edge.AnchoringMode));
			WriteElement(xEdge, "SourcePoint", edge.SourcePoint);
			WriteElement(xEdge, "DestinationPoint", edge.DestinationPoint);

			if (edge.SourceNode != null)
			{
				int sourceIndex = _diagram.Nodes.IndexOf(edge.SourceNode);
				if (sourceIndex >= 0)
				{
					WriteElement(xEdge, "SourceNodeIndex", sourceIndex);
				}
			}

			if (edge.DestinationNode != null)
			{
				int destinationIndex = _diagram.Nodes.IndexOf(edge.DestinationNode);
				if (destinationIndex >= 0)
				{
					WriteElement(xEdge, "DestinationNodeIndex", destinationIndex);
				}
			}

			SaveItemPart(xEdge, edge);

			if (edge.Label != null)
			{
				SaveLabelToXElement(xEdge, edge.Label);
			}

			xEdges.Add(xEdge);
		}

		private void LoadEdgeFromXElement(XElement xEdge, DiagramEdge edge)
		{
			string anMode = "";
			if (ReadElement(xEdge, "AnchoringMode", out anMode))
			{
				edge.AnchoringMode = (EdgeAnchoringMode)Enum.Parse(typeof(EdgeAnchoringMode), anMode);
			}

			Point sourcePoint;
			if (ReadElement(xEdge, "SourcePoint", out sourcePoint))
			{
				edge.SourcePoint = sourcePoint;
			}

			Point DestinationPoint;
			if (ReadElement(xEdge, "DestinationPoint", out DestinationPoint))
			{
				edge.DestinationPoint = DestinationPoint;
			}

			int sourceNodeIndex = -1;
			if (ReadElement(xEdge, "SourceNodeIndex", out sourceNodeIndex))
			{
				if (sourceNodeIndex >= 0 && sourceNodeIndex < _diagram.Nodes.Count)
				{
					edge.SourceNode = _diagram.Nodes[sourceNodeIndex];
				}
			}

			int destinationNodeIndex = -1;
			if (ReadElement(xEdge, "DestinationNodeIndex", out destinationNodeIndex))
			{
				if (destinationNodeIndex >= 0 && destinationNodeIndex < _diagram.Nodes.Count)
				{
					edge.DestinationNode = _diagram.Nodes[destinationNodeIndex];
				}
			}

			LoadItemPart(xEdge, edge);

			XElement xLabel = xEdge.Element("Label");
			if (xLabel != null)
			{
				LoadLabelFromXElement(xLabel, edge.Label);
			}
		}

		private void SaveLabelToXElement(XElement xItem, DiagramLabel label)
		{
			XElement xLabel = new XElement("Label");

			WriteElement(xLabel, "RelativePosition", label.RelativePosition);
			WriteElement(xLabel, "Text", label.Text);
			SaveItemPart(xLabel, label);

			xItem.Add(xLabel);
		}

		private void LoadLabelFromXElement(XElement xLabel, DiagramLabel label)
		{
			Point relPos = new Point();
			if (ReadElement(xLabel, "RelativePosition", out relPos))
			{
				label.RelativePosition = relPos;
			}

			string text = "";
			if (ReadElement(xLabel, "Text", out text))
			{
				label.Text = text;
			}

			LoadItemPart(xLabel, label);
		}

		private void SaveItemPart(XElement xIn, DiagramItem item)
		{
			WriteElement(xIn, "UserData", item.UserData);
		}

		private void LoadItemPart(XElement xIn, DiagramItem item)
		{
			string res;
			if (ReadElement(xIn, "UserData", out res))
			{
				item.UserData = res;
			}
		}

		#endregion
	}
}
