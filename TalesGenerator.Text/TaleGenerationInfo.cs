using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TalesGenerator.TaleNet;

namespace TalesGenerator.Text
{
	internal class TaleGenerationInfo
	{
		#region Properties

		public TaleNode Tale { get; private set; }

		public double RelevanceLevel { get; private set; }

		public ICollection<FunctionConflictSet> ConflictSets { get; private set; }
		#endregion

		#region Constructors

		public TaleGenerationInfo(TaleNode tale, double relevanceLevel)
		{
			Contract.Requires<ArgumentNullException>(tale != null);

			Tale = tale;
			RelevanceLevel = relevanceLevel;
			ConflictSets = new List<FunctionConflictSet>();
		}
		#endregion
	}
}
