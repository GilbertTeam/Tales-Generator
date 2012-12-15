using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gt.Controls.Diagramming
{
	public delegate void NodeEventHandler(NodeEventArgs args);

	public delegate void EdgeEventHandler(EdgeEventArgs args);

	public delegate void LabelEventHandler(LabelEventArgs args);

	public delegate void DiagramNotifyDelegate();
}
