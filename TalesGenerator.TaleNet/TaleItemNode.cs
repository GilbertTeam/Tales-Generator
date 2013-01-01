using System;
using System.Diagnostics.Contracts;
using System.Xml.Linq;
using TalesGenerator.Text;

namespace TalesGenerator.TaleNet
{
	public class TaleItemNode : TaleBaseItemNode, IFormattable
	{
		#region Properties

		internal override TaleNodeKind NodeKind
		{
			get { return TaleNodeKind.TaleItem; }
		}

		public Grammem Grammem { get; set; }
		#endregion

		#region Constructors

		internal TaleItemNode(TalesNetwork talesNetwork)
			: base(talesNetwork)
		{

		}

		internal TaleItemNode(TalesNetwork talesNetwork, string name)
			: base(talesNetwork, name)
		{

		}

		internal TaleItemNode(TalesNetwork talesNetwork, string name, TaleItemNode baseNode)
			: base(talesNetwork, name, baseNode)
		{

		}
		#endregion

		#region Methods

		public override XElement GetXml()
		{
			XElement xElement = base.GetXml();

			xElement.Add(new XAttribute("grammem", Grammem));

			return xElement;
		}

		public override void LoadFromXml(XElement xElement)
		{
			Contract.Requires<ArgumentNullException>(xElement != null);

			base.LoadFromXml(xElement);

			Grammem = (Grammem)Enum.Parse(typeof(Grammem), xElement.Attribute("grammem").Value);
		}
		#endregion
	}
}
