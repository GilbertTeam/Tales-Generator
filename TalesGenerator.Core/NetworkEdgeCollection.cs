using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace TalesGenerator.Core
{
	public class NetworkEdgeCollection : ObservableCollection<NetworkEdge>
	{
		public void AddRange(IEnumerable<NetworkEdge> edges)
		{
			if (edges == null)
			{
				throw new ArgumentNullException("edges");
			}

			foreach (NetworkEdge edge in edges)
			{
				Add(edge);
			}
		}
	}
}
