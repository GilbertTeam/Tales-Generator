using System.Collections.Generic;
using System.Linq;
using TalesGenerator.Net;
using TalesGenerator.TaleNet.Collections;

namespace TalesGenerator.TaleNet
{
	public class TaleNode : TaleBaseItemNode
	{
		#region Fields

		private readonly TaleFunctionNodeCollection _functionNodes;
		#endregion

		#region Properties

		internal override TaleNodeKind NodeKind
		{
			get { return TaleNodeKind.Tale; }
		}

		public TaleFunctionNodeCollection Functions
		{
			get { return _functionNodes; }
		}

		public IEnumerable<FunctionType> Scenario
		{
			get
			{
				return _functionNodes.Select(functionNode => functionNode.FunctionType);
			}
		}
		#endregion

		#region Constructors

		internal TaleNode(TalesNetwork talesNetwork)
			: base(talesNetwork)
		{
			_functionNodes = new TaleFunctionNodeCollection(this);
		}

		internal TaleNode(TalesNetwork talesNetwork, string name)
			: base(talesNetwork, name)
		{
			_functionNodes = new TaleFunctionNodeCollection(this);

			//TODO Необходимо разобраться с событиями коллекций. Возможно, они сами должны подписывать сеть.
			//_functionNodes.CollectionChanged += OnNetworkObjectCollectionChanged;
			//_functionNodes.PropertyChanged += OnNetworkObjectPropertyChanged;
		}

		internal TaleNode(TalesNetwork taleNetwork, string name, TaleNode baseTaleNode)
			: base(taleNetwork, name, baseTaleNode)
		{
			_functionNodes = new TaleFunctionNodeCollection(this);
		}
		#endregion

		#region Event Handlers

		//private void OnNetworkObjectCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		//{
		//    _isDirty = true;
		//}

		//private void OnNetworkObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		//{
		//    _isDirty = true;
		//}
		#endregion
	}
}
