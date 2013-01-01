using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TalesGenerator.Net;

namespace TalesGenerator.Text
{
	public abstract class TemplateParserBaseResult
	{
		#region Properties

		/// <summary>
		/// Результирующий текст.
		/// </summary>
		public string Text { get; private set; }

		/// <summary>
		/// Падежи, которые не удалось найти.
		/// </summary>
		public IEnumerable<NetworkEdgeType> UnresolvedContext { get; private set; }
		#endregion

		#region Constructors

		protected TemplateParserBaseResult(string text)
		{
			Text = text;
		}

		protected TemplateParserBaseResult(string text, IEnumerable<NetworkEdgeType> unresolvedContext)
			: this(text)
		{
			Contract.Requires<ArgumentNullException>(unresolvedContext != null);

			UnresolvedContext = unresolvedContext;
		}
		#endregion

		#region Methods

		public override string ToString()
		{
			return string.Format("Text = \"{0}\".", Text);
		}
		#endregion
	}
}
