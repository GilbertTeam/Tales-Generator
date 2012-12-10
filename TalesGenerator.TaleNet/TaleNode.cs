using TalesGenerator.Net;
using TalesGenerator.TaleNet.Collections;

namespace TalesGenerator.TaleNet
{
	public class TaleNode : NetworkNode
	{
		#region Fields

		private readonly TaleFunctionNodeCollection _functionNodes;
		#endregion

		#region Properties

		public TaleFunctionNodeCollection FunctionNodes
		{
			get { return _functionNodes; }
		}
		#endregion

		#region Constructors

		internal TaleNode(TalesNetwork taleNetwork, string name)
			: base(taleNetwork, name)
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
