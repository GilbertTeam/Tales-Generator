using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

using TalesGenerator.Net;

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

		public event NotifyEventHandler NeedBuildDiagram;

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
				RaiseNeedBuildDiagram();
			}
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

		protected void RaiseNeedBuildDiagram()
		{
			if (NeedBuildDiagram != null)
				NeedBuildDiagram();
		}

		#endregion
	}
}
