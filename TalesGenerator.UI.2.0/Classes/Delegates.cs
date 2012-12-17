using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TalesGenerator.Net;
using TalesGenerator.TaleNet;

using Gt.Controls.Diagramming;

namespace TalesGenerator.UI.Classes
{
	public delegate void NotifyEventHandler();

	public delegate void BoolNotifyEventHander(bool value);

	public delegate void OnSelectionChanged(int id);
	//public delegate void DiagramItemEventHandler(DiagramItem item, NetworkObject obj);

	public delegate void DiagramItemEventHandler(DiagramItem item, NetworkObject obj);
}
