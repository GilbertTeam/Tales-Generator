using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gt.Controls.Diagramming
{
	public class DiagramUpdateLock : IDisposable
	{
		#region Fields

		private Diagram _diagram;

		#endregion

		#region Constructors

		public DiagramUpdateLock(Diagram diagram)
		{
			_diagram = diagram;

			_diagram.LockRecalc = true;
			_diagram.LockRender = true;
		}

		#endregion

		#region Methods

		public void Dispose()
		{
			_diagram.LockRecalc = false;
			_diagram.LockRender = false;

			_diagram.InvalidateDiagram();
		}

		#endregion
	}
}
