using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using MindFusion.Diagramming.Wpf;

using TalesGenerator.Net;

namespace TalesGenerator.UI.Classes
{
	class DiagramNodeEx : InplaceEditable
	{
		DiagramNode DiagramNode { get; set; }
		NetworkNode NetworkNode { get; set; }

		public DiagramNodeEx(DiagramNode _node, NetworkNode _netNode)
		{
			DiagramNode = _node;
			NetworkNode = _netNode;
		}

		public DiagramItem GetDiagramItem()
		{
			return DiagramNode;
		}

		public System.Windows.Rect GetEditRect(System.Windows.Point mousePosition)
		{
			Point endPoint = new Point(mousePosition.X + DiagramNode.Bounds.Width, 
				mousePosition.Y + DiagramNode.Bounds.Height);
			return new Rect(0, 0, DiagramNode.Bounds.Width, DiagramNode.Bounds.Height);
		}

		public string GetTextToEdit()
		{
			return NetworkNode.Name;
		}

		public void SetEditedText(string newText)
		{
			NetworkNode.Name = newText;
		}
	}
}
