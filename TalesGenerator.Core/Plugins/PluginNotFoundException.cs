using System;

namespace TalesGenerator.Core.Plugins
{
	public class PluginNotFoundException : Exception
	{
		#region Constructors

		public PluginNotFoundException(string pluginName)
			: this(pluginName, null)
		{

		}

		public PluginNotFoundException(string pluginName, Exception innerException)
			: base(string.Format(Properties.Resources.PluginNotFoundError, pluginName), innerException)
		{

		}
		#endregion
	}
}
