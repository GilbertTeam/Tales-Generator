using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TalesGenerator.Core;
using TalesGenerator.Core.Collections;
using TalesGenerator.Text;

namespace Test
{
	public class TextGenerator
	{
		#region Fields

		private readonly TextAnalyzer _textAnalyzer;

		private readonly Parser _parser;
		#endregion

		#region Constructors

		public TextGenerator(TextAnalyzer textAnalyzer)
		{
			if (textAnalyzer == null)
			{
				throw new ArgumentNullException("textAnalyzer");
			}

			_textAnalyzer = textAnalyzer;
			_parser = new Parser(_textAnalyzer);
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
				textBuilder.AppendFormat("{0}\n", _parser.Parse(currentNode));

				NetworkEdge followEdge = currentNode.OutgoingEdges.GetEdge(NetworkEdgeType.Follow);
				currentNode = followEdge != null ? followEdge.EndNode : null;
			}

			return textBuilder.ToString();
		}
		#endregion
	}
}
