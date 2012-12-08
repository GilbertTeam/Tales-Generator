using TalesGenerator.Core.Plugins;

namespace TalesGenerator.Text.Plugins
{
	public interface ITemplateParserPlugin : IPlugin
	{
		bool CanParse(string template);

		string Parse(ITemplateParser parser, string template);
	}
}
