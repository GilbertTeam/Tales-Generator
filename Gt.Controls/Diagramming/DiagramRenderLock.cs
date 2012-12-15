using System;

namespace Gt.Controls.Diagramming
{
	public class DiagramRenderLock : IDisposable
	{
		#region Fields

		readonly Diagram _diagram;

		#endregion

		#region Constructors

		public DiagramRenderLock(Diagram diagram)
		{
			_diagram = diagram;

			_diagram.LockRender = true;
		}

		#endregion

		#region Methods

		public void Dispose()
		{
			_diagram.LockRender = false;
			_diagram.InvalidateVisual();
		}

		#endregion
	}
}
