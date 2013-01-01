using TalesGenerator.Core.Plugins;

namespace TalesGenerator.Text.Plugins
{
	public interface ITemplateParserPlugin : IPlugin
	{
		bool CanParse(string template);

		// TODO: Нужно избавиться от передачи парсера плагинам.
		TemplateParserPluginResult Parse(ITemplateParser parser, string template);
	}
}
