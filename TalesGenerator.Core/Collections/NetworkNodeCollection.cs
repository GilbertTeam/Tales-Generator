
namespace TalesGenerator.Core.Collections
{
	public class NetworkNodeCollection : NetworkObjectCollection<NetworkNode>
	{
		#region Constructors

		public NetworkNodeCollection(Network network)
			: base(network)
		{
		}
		#endregion

		#region Methods

		public NetworkNode Add()
		{
			NetworkNode networkNode = new NetworkNode(_network);

			Add(networkNode);

			return networkNode;
		}

		public NetworkNode Add(string name)
		{
			NetworkNode networkNode = new NetworkNode(_network, name);

			Items.Add(networkNode);

			return networkNode;
		}
		#endregion
	}
}
