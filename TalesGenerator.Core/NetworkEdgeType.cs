using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalesGenerator.Core
{
	public class NetworkEdgeType
	{
		protected string _name;
	}

	public class NetworkSystemEdgeType : NetworkEdgeType
	{
		public string Name
		{
			get { return _name; }
		}
	}
}
