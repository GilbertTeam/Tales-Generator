using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TalesGenerator.Text.Plugins
{
	public abstract class TemplateParserPlugin : ITemplateParserPlugin
	{
		#region Fields

		private readonly IEnumerable<Regex> _availableRegexes;
		#endregion

		#region Properties

		public abstract string Name { get; }
		#endregion

		#region Constructors

		public TemplateParserPlugin()
		{
			_availableRegexes = GetAvailableRegexes();
		}
		#endregion

		#region Methods

		protected abstract IEnumerable<Regex> GetAvailableRegexes();

		public abstract string Parse(ITemplateParser parser, string template);

		public bool CanParse(string template)
		{
			if (string.IsNullOrEmpty(template))
			{
				throw new ArgumentException("template");
			}

			return _availableRegexes.Any(regex => regex.IsMatch(template));
		}
		#endregion
	}
}
