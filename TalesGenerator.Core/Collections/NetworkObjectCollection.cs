using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

namespace TalesGenerator.Core.Collections
{
	public abstract class NetworkObjectCollection<T> : INotifyCollectionChanged, INotifyPropertyChanged, IEnumerable, IEnumerable<T> where T : NetworkObject
	{
		#region Fields

		private readonly List<T> _items = new List<T>();

		protected readonly Network _network;
		#endregion

		#region Properties

		public int Count
		{
			get { return _items.Count; }
		}

		public T this[int index]
		{
			get { return _items[index]; }
		}
		#endregion

		#region Events

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Constructors

		protected NetworkObjectCollection(Network network)
		{
			if (network == null)
			{
				throw new NotImplementedException();
			}

			_network = network;
		}
		#endregion

		#region Event Handlers

		private void NetworkObjectOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(sender, e.PropertyName);
		}
		#endregion

		#region Methods

		protected void OnCollectionChanged(NotifyCollectionChangedAction action)
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
			}
		}

		protected void OnCollectionChanged(NotifyCollectionChangedAction action, T changedItem)
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
			}
		}

		protected void OnCollectionChanged(NotifyCollectionChangedAction action, IList<T> changedItems)
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItems));
			}
		}

		protected void OnPropertyChanged(object sender, string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}

		internal void Add(T item)
		{
			item.PropertyChanged += new PropertyChangedEventHandler(NetworkObjectOnPropertyChanged);

			_items.Add(item);

			OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
		}

		internal void RemoveAt(int index)
		{
			if (index < 0 || index >= _items.Count)
			{
				throw new ArgumentException("index");
			}

			T item = _items[index];
			item.PropertyChanged -= NetworkObjectOnPropertyChanged;

			bool result = _items.Remove(item);

			//TODO Подумать над этим.
			OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
		}

		public virtual bool Remove(T item)
		{
			item.PropertyChanged -= NetworkObjectOnPropertyChanged;

			bool result = _items.Remove(item);

			//TODO Подумать над этим.
			OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);

			return result;
		}

		public T FindById(int id)
		{
			T networkObject = null;
			var networkObjects = _items.Where(node => node.Id == id);
			int count = networkObjects.Count();

			if (count == 1)
			{
				networkObject = networkObjects.First();
			}
			else if (count > 1)
			{
				//В сети не может быть объектов с одинаковыми идентификаторами.
				throw new InvalidOperationException();
			}

			return networkObject;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return _items.GetEnumerator();
		}
		#endregion
	}
}
