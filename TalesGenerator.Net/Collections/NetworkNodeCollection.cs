
namespace TalesGenerator.Net.Collections
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
			NetworkNode networkNode = new NetworkNode(Network);

			Add(networkNode);

			return networkNode;
		}

		public NetworkNode Add(string name)
		{
			NetworkNode networkNode = new NetworkNode(Network, name);

			Add(networkNode);

			return networkNode;
		}

		public NetworkNode Add(string name, NetworkNode baseNode)
		{
			NetworkNode networkNode = new NetworkNode(Network, name, baseNode);

			Add(networkNode);

			return networkNode;
		}
		#endregion
	}
}
