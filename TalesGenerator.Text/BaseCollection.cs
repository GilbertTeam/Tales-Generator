using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace TalesGenerator.Text
{
	public class BaseCollection<T> : ICollection<T>
	{
		#region Fields

		private readonly List<T> _list = new List<T>();
		#endregion

		#region Properties

		protected List<T> List
		{
			get { return _list; }
		}
		#endregion

		#region Methods

		public virtual void Add(T item)
		{
			_list.Add(item);
		}

		public void AddRange(IEnumerable<T> items)
		{
			Contract.Requires<ArgumentNullException>(items != null);

			foreach (T item in items)
			{
				Add(item);
			}
		}

		public void AddRange(params T[] items)
		{
			AddRange((IEnumerable<T>)items);
		}

		public void Clear()
		{
			_list.Clear();
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public bool IsReadOnly
		{
			get { return ((ICollection<T>)_list).IsReadOnly; }
		}

		public bool Remove(T item)
		{
			return _list.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}
		#endregion
	}
}
