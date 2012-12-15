using System.Collections.ObjectModel;

namespace Gt.Controls.Diagramming
{
	public class DiagramItemCollection<T> : ObservableCollection<T>
	{
		#region Fields

		private readonly Diagram _diagram;

		#endregion

		#region Constructors

		public DiagramItemCollection(Diagram diagram)
			: base()
		{
			_diagram = diagram;
		}

		#endregion

		#region Properties

		public Diagram Diagram
		{
			get { return _diagram; }
		}

		#endregion

		#region Methods

		protected override void SetItem(int index, T item)
		{
			if (this.Contains(item))
				throw new DiagramException("Такой итем уже сщуествует в коллекции");

			base.SetItem(index, item);
		}

		protected override void InsertItem(int index, T item)
		{
			if (this.Contains(item))
				throw new DiagramException("Такой итем уже существует в коллекции");

			base.InsertItem(index, item);
		}

		#endregion
	}
}
