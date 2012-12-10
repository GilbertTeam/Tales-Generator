using System;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;

namespace TalesGenerator.TaleNet.Collections
{
	public abstract class BaseTaleNodeCollection<T> : NetworkObjectCollection<T> where T : NetworkNode
	{
		#region Constructors

		protected BaseTaleNodeCollection(TalesNetwork talesNetwork)
			: base(talesNetwork)
		{

		}
		#endregion

		#region Methods

		public override void Add(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			Network.Nodes.Add(item);

			base.Add(item);
		}

		public override bool Remove(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			Network.Nodes.Remove(item);

			return base.Remove(item);
		}
		#endregion
	}
}
