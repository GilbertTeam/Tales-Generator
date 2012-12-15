using System.Collections.Generic;

namespace Gt.Controls.Diagramming
{
	public class DiagramSelectionBorder
	{

		#region Fields

		private List<ResizeInfo> _infos;

		#endregion

		#region Constructors

		public DiagramSelectionBorder(bool isLinked = true)
		{
			_infos = new List<ResizeInfo>();
			IsLinked = isLinked;
		}

		#endregion

		#region Properties

		public List<ResizeInfo> ResizeInfos
		{
			get { return _infos; }
		}

		public bool IsLinked { get; set; }

		#endregion
	}
}
