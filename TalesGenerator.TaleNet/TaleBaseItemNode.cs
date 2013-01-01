using TalesGenerator.Net;
using System.Xml.Linq;
using System.Diagnostics.Contracts;
using System;

namespace TalesGenerator.TaleNet
{
	public enum TaleNodeKind
	{
		Tale,
		TaleItem,
		Function
	}

	public abstract class TaleBaseItemNode : NetworkNode
	{
		#region Properties

		internal abstract TaleNodeKind NodeKind { get; }
		#endregion

		#region Constructors

		protected TaleBaseItemNode(TalesNetwork talesNetwork)
			: base(talesNetwork)
		{

		}

		protected TaleBaseItemNode(TalesNetwork talesNetwork, string name)
			: base(talesNetwork, name)
		{

		}

		protected TaleBaseItemNode(TalesNetwork talesNetwork, string name, TaleBaseItemNode baseNode)
			: base(talesNetwork, name, baseNode)
		{

		}
		#endregion

		#region Methods

		public override XElement GetXml()
		{
			XElement xElement = base.GetXml();

			xElement.Add(new XAttribute("nodeKind", NodeKind));

			return xElement;
		}
		#endregion
	}
}
