﻿using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TalesGenerator.Core.Collections
{
	public abstract class NetworkObjectCollection<T> : ObservableCollection<T> where T : NetworkObject
	{
		#region Fields

		protected readonly Network _network;
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

		#region Methods

		internal new void Add(T networkObject)
		{
			Add(networkObject);
		}

		public T FindById(int id)
		{
			T networkObject = null;
			var networkObjects = Items.Where(node => node.Id == id);
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
