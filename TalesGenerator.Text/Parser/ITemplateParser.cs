using System.Collections.Generic;
using TalesGenerator.TaleNet;

namespace TalesGenerator.Text
{
	public interface ITemplateParser
	{
		#region Properties

		IEnumerable<TemplateToken> CurrentSentence { get; }

		ITemplateParserContext NetworkContext { get; }

		TextAnalyzer TextAnalyzer { get; }
		#endregion

		#region Methods
		
 		/// <summary>
		/// Изменяет форму слова в соответствии с указанной граммемой.
		/// </summary>
		/// <param name="word">Слово, форму которого необходимо изменить.</param>
		/// <param name="grammem">Граммема, в соответствии с которой необходимо изменить форму слова.</param>
		/// <returns>Согласованная форма слова.</returns>
		string ReconcileWord(string word, Grammem grammem);

		/// <summary>
		/// Изменяет (согласует) форму слова, в соответствии с формой другого.
		/// </summary>
		/// <param name="word">Слово, форму которого необходимо согласовать.</param>
		/// <param name="baseWord">Слово, в соответствии с формой которого будет выполнено согласование.</param>
		/// <returns>Согласованная форма слова.</returns>
		string ReconcileWord(string word, string baseWord);

		TemplateParserResult Parse(FunctionNode functionNode);

		TemplateParserResult Parse(FunctionNode functionNode, ITemplateParserContext parserContext);
		#endregion
	}
}
