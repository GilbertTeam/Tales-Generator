using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace TalesGenerator.Core
{
	public class NetworkNodeCollection : ObservableCollection<NetworkNode>
	{
		public void AddRange(IEnumerable<NetworkNode> nodes)
		{
			if (nodes == null)
			{
				throw new ArgumentNullException("nodes");
			}

			foreach (NetworkNode node in nodes)
			{
				Add(node);
			}
		}

		public NetworkNode FindById(int id)
		{
			NetworkNode networkNode = null;
			var networkNodes = Items.Where(node => node.Id == id);
			int count = networkNodes.Count();

			if (count == 1)
			{
				networkNode = networkNodes.First();
			}
			else if (count > 1)
			{
				//В сети не может быть вершин с одинаковыми идентификаторами.
				throw new InvalidOperationException();
			}

			return networkNode;
		}
	}
}
