using System;
using System.Diagnostics.Contracts;
using TalesGenerator.TaleNet;

namespace TalesGenerator.Text
{
	internal class FunctionGenerationInfo
	{
		#region Properties

		public FunctionNode Function { get; private set; }

		public double RelevanceLevel { get; private set; }
		#endregion

		#region Constructors

		public FunctionGenerationInfo(FunctionNode function, double relevanceLevel)
		{
			Contract.Requires<ArgumentNullException>(function != null);

			Function = function;
			RelevanceLevel = relevanceLevel;
		}
		#endregion

		#region Methods

		public override string ToString()
		{
			return string.Format("Function = {0}. Relevance level = {1}.", Function, RelevanceLevel);
		}
		#endregion
	}
}
