using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MindFusion.Diagramming.Wpf;
using TalesGenerator.Net;

namespace TalesGenerator.UI.Classes
{
	public delegate void OnSelectionChanged(int id);
	public delegate void DiagramItemEventHandler(DiagramItem item, NetworkObject obj);
}
