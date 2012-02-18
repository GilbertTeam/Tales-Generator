using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TalesGenerator.Core
{
	/// <summary>
	/// Представляет вершину сети.
	/// </summary>
	public class NetworkNode : NetworkObject
	{
		#region Fields

		private readonly SemanticNetwork _network;
		#endregion

		#region Properties

		/// <summary>
		/// Возвращает или задает имя вершины.
		/// </summary>
		public string Name { get; set; }

		
		#endregion

		#region Constructors

		public NetworkNode()
		{
			
		}
		#endregion

		#region Event Handlers

		private void _incomingEdges_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				foreach (SemanticNetworkEdge edge in e.NewItems)
				{
					_network.Edges.Add(edge);
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
			{
				//_network.Edges.
			}

			//_network.Edge
		}

		private void _outgoingEdges_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			throw new NotImplementedException();
		}
		#endregion

		public string SaveToXml()
		{
			throw new NotImplementedException();

			XElement xNode = new XElement("Node");
		}
	}
}
