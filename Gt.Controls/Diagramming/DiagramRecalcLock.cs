using System;

namespace Gt.Controls.Diagramming
{
	class DiagramRecalcLock : IDisposable
	{
		#region Fields

		private Diagram _diagram;

		#endregion

		#region Constructors

		public DiagramRecalcLock(Diagram diagram)
		{
			_diagram = diagram;

			_diagram.LockRecalc = true;
		}

		#endregion

		#region Methods

		public void Dispose()
		{
			_diagram.LockRecalc = false;
			_diagram.InvalidateDiagram();
		}

		#endregion
	}
}
