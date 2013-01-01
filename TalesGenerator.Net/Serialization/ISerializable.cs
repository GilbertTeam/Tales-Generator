using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TalesGenerator.Net.Serialization
{
	public interface ISerializable
	{
		XElement SaveToXml();

		void LoadFromXml(XElement xElement);
	}
}
