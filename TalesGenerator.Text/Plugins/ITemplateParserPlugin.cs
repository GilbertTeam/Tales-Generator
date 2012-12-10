using TalesGenerator.Core.Plugins;

namespace TalesGenerator.Text.Plugins
{
	public interface ITemplateParserPlugin : IPlugin
	{
		bool CanParse(string template);

		TemplateParserResult Parse(ITemplateParser parser, string template);
	}
}
