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
	}
}
