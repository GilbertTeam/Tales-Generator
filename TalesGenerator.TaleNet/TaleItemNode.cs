using TalesGenerator.Net;
using TalesGenerator.Text;

namespace TalesGenerator.TaleNet
{
	public class TaleItemNode : NetworkNode, IFormattable
	{
		#region Properties

		public Grammem Grammem { get; set; }
		#endregion

		#region Constructors

		internal TaleItemNode(TalesNetwork talesNetwork, string name)
			: base(talesNetwork, name)
		{

		}

		internal TaleItemNode(TalesNetwork talesNetwork, string name, TaleItemNode baseNode)
			: base(talesNetwork, name, baseNode)
		{

		}
		#endregion
	}
}
