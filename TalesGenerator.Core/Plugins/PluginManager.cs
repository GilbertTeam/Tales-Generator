using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TalesGenerator.Core.Plugins
{
	public static class PluginManager
	{
		#region Fields

		private static string _pluginsDirPath = Path.Combine(Environment.CurrentDirectory, "Plugins");

		private static List<IPlugin> _plugins = new List<IPlugin>();
		#endregion

		#region Properties

		public static IEnumerable<IPlugin> Plugins
		{
			get { return _plugins; }
		}
		#endregion

		#region Methods

		public static IEnumerable<T> GetPlugins<T>() where T : IPlugin
		{
			return _plugins.Where(plugin => plugin.GetType().GetInterfaces().Contains(typeof(T))).Cast<T>();
		}

		public static IEnumerable<IPlugin> LoadAllPlugins()
		{
			return LoadAllPlugins(_pluginsDirPath);
		}

		public static IEnumerable<IPlugin> LoadAllPlugins(string pluginsDirPath)
		{
			return LoadPlugins<IPlugin>(pluginsDirPath);
		}

		public static IEnumerable<T> LoadPlugins<T>() where T : IPlugin
		{
			return LoadPlugins<T>(_pluginsDirPath);
		}

		public static IEnumerable<T> LoadPlugins<T>(string pluginsDirPath) where T : IPlugin
		{
			if (string.IsNullOrEmpty(pluginsDirPath))
			{
				throw new ArgumentException("pluginsDirPath");
			}

			List<T> loadedPlugins = new List<T>();

			if (Directory.Exists(pluginsDirPath))
			{
				var files = Directory.EnumerateFiles(pluginsDirPath, "*.dll");
				string pluginInterfaceName = typeof(T).Name;

				foreach (string file in files)
				{
					try
					{
						//TODO Необходимо загружать сборку в отдельный AppDomain.
						Assembly assembly = Assembly.LoadFile(file);
						Type[] assemblyTypes = assembly.GetTypes();
						Type pluginType = assemblyTypes.SingleOrDefault(type => !type.IsAbstract && type.GetInterfaces().Contains(typeof(T)));

						if (pluginType != null)
						{
							loadedPlugins.Add((T)Activator.CreateInstance(pluginType));
						}
					}
					catch (Exception) { }
				}
			}

			_plugins.AddRange(loadedPlugins.Cast<IPlugin>());

			return loadedPlugins;
		}
		#endregion
	}
}
