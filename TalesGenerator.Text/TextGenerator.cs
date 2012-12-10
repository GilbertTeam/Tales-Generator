using System;
using System.Text;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;

namespace TalesGenerator.Text
{
	public class TextGenerator
	{
		#region Fields

		private readonly TextAnalyzer _textAnalyzer;

		private readonly TemplateParser _parser;
		#endregion

		#region Constructors

		public TextGenerator(TextAnalyzer textAnalyzer)
		{
			if (textAnalyzer == null)
			{
				throw new ArgumentNullException("textAnalyzer");
			}

			_textAnalyzer = textAnalyzer;
			_parser = new TemplateParser(_textAnalyzer);
		}
		#endregion

		#region Methods

		public string GenerateText(NetworkNode startNode)
		{
			if (startNode == null)
			{
				throw new ArgumentNullException("startNode");
			}

			NetworkNode currentNode = startNode;
			StringBuilder textBuilder = new StringBuilder();

			while (currentNode != null)
			{
				textBuilder.AppendFormat("{0}{1}", _parser.Parse(currentNode), Environment.NewLine);

				NetworkEdge followEdge = currentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Follow);
				currentNode = followEdge != null ? followEdge.EndNode : null;
			}

			return textBuilder.ToString();
		}
		#endregion
	}
}
