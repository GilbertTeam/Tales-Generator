using System;
using System.Collections.Generic;
using TalesGenerator.Net;

namespace TalesGenerator.Text
{
	public class TemplateParserResult
	{
		#region Properties

		public string Text { get; private set; }

		public IEnumerable<NetworkEdgeType> UnresolvedContext { get; private set; }
		#endregion

		#region Constructors

		public TemplateParserResult(string text)
		{
			Text = text;
		}

		public TemplateParserResult(string text, IEnumerable<NetworkEdgeType> unresolvedContext)
		{
			if (unresolvedContext == null)
			{
				throw new ArgumentNullException("unresolvedContext");
			}

			Text = text;
			UnresolvedContext = unresolvedContext;
		}
		#endregion

		#region Methods

		public override string ToString()
		{
			return string.Format("Text = {0}", Text);
		}
		#endregion
	}
}
