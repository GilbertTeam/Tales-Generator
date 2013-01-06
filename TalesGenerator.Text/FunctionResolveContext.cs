using System;
using System.Diagnostics.Contracts;
using TalesGenerator.TaleNet;

namespace TalesGenerator.Text
{
	internal class FunctionResolveContext
	{
		#region Properties

		public FunctionNode Function { get; private set; }

		public int RelevanceLevel { get; private set; }
		#endregion

		#region Constructors

		public FunctionResolveContext(FunctionNode function, int relevanceLevel)
		{
			Contract.Requires<ArgumentNullException>(function != null);

			Function = function;
			RelevanceLevel = relevanceLevel;
		}
		#endregion
	}
}
