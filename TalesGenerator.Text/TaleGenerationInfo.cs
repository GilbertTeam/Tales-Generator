using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TalesGenerator.TaleNet;

namespace TalesGenerator.Text
{
	public class TaleGenerationInfo
	{
		#region Properties

		public TaleNode Tale { get; private set; }

		public double RelevanceLevel { get; private set; }

		public FunctionConflictSetCollection ConflictSets { get; private set; }
		#endregion

		#region Constructors

		public TaleGenerationInfo(TaleNode tale, double relevanceLevel)
		{
			Contract.Requires<ArgumentNullException>(tale != null);

			Tale = tale;
			RelevanceLevel = relevanceLevel;
			ConflictSets = new FunctionConflictSetCollection();
		}
		#endregion

		#region Methods

		public override string ToString()
		{
			return string.Format("Tale = {0}. Relevance level = {1}.", Tale, RelevanceLevel);
		}
		#endregion
	}
}
