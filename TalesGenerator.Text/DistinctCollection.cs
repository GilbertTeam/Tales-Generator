using System;

namespace TalesGenerator.Text
{
	internal class DistinctCollection<T> : BaseCollection<T>
	{
		#region Methods

		public override void Add(T item)
		{
			if (!Contains(item))
			{
				base.Add(item);
			}
		}
		#endregion
	}
}
