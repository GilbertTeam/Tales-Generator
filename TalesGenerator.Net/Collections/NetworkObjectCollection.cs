using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace TalesGenerator.Net.Collections
{
	public abstract class NetworkObjectCollection<T> : INotifyCollectionChanged, INotifyPropertyChanged, IEnumerable, IEnumerable<T> where T : NetworkObject
	{
		#region Fields

		private readonly List<T> _items = new List<T>();

		private readonly Network _network;
		#endregion

		#region Properties

		protected Network Network
		{
			get { return _network; }
		}

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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return _items.GetEnumerator();
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

		protected void OnCollectionChanged(NotifyCollectionChangedAction action, T changedItem, int index)
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem, index));
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

		//TODO Не уверен, что метод должен быть открытым.
		public virtual void Add(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (_items.Contains(item))
			{
				throw new ArgumentException("item");
			}
			if (_items.Where(i => i.Id == item.Id).Any())
			{
				throw new ArgumentException("item");
			}

			item.PropertyChanged += NetworkObjectOnPropertyChanged;

			_items.Add(item);

			OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
		}

		public virtual bool Remove(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			item.PropertyChanged -= NetworkObjectOnPropertyChanged;

			int index = _items.IndexOf(item);
			bool result = _items.Remove(item);

			//TODO Подумать над этим.
			OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);

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
		#endregion
	}
}
