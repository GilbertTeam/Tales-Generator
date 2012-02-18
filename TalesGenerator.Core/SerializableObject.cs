using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TalesGenerator.Core
{
	public abstract class SerializableObject
	{
		#region Fields

		public const string Namespace = "http://www.giblertteam.com";

		internal const XNamespace XNamespace = Namespace;
		#endregion

		#region Methods

		internal abstract void SaveToXml(XElement xElement);

		//internal abstract void Save

		public abstract string SaveToXml();

		internal abstract void LoadFromXml(XElement xElement);
		#endregion
	}
}
