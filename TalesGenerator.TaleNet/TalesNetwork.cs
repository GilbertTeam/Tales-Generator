using System.Collections.Specialized;
using System.ComponentModel;
using TalesGenerator.Net;
using TalesGenerator.TaleNet.Collections;

namespace TalesGenerator.TaleNet
{
	public class TalesNetwork : Network
	{
		#region Fields

		private readonly TaleNodeCollection _taleNodes;
		#endregion

		#region Properties

		public TaleNodeCollection Tales
		{
			get { return _taleNodes; }
		}
		#endregion

		#region Constructors

		public TalesNetwork()
		{
			_taleNodes = new TaleNodeCollection(this);
		}
		#endregion
	}
}
