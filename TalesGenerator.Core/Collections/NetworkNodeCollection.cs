
namespace TalesGenerator.Core.Collections
{
	public class NetworkNodeCollection : NetworkObjectCollection<NetworkNode>
	{
		#region Constructors

		internal NetworkNodeCollection(Network network)
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

			Add(networkNode);

			return networkNode;
		}

		public NetworkNode Add(string name, NetworkNode baseNode)
		{
			NetworkNode networkNode = new NetworkNode(_network, name, baseNode);

			Add(networkNode);

			return networkNode;
		}
		#endregion
	}
}
