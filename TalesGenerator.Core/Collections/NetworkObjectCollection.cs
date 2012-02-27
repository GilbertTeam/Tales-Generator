using System;
using System.Linq;
using System.Collections.ObjectModel;

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
