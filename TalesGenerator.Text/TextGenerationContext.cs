using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TalesGenerator.Net;
using TalesGenerator.TaleNet;

namespace TalesGenerator.Text
{
	internal class TextGenerationContext
	{
		#region Properties

		public string Text { get; private set; }

		public TalesNetwork Network { get; private set; }

		public GenerationCollection Tales { get; private set; }

		public ICollection<FunctionGenerationInfo> ResolvedFunctions { get; private set; }

		public ICollection<NetworkNode> ResolvedPersons { get; private set; }

		public ICollection<NetworkNode> ResolvedLocatives { get; private set; }

		public ICollection<NetworkNode> ResolvedActions { get; private set; }

		public DistinctCollection<NetworkNode> CurrentContextNodes { get; private set; }
		#endregion

		#region Constructors

		public TextGenerationContext(TalesNetwork network, string text)
		{
			Contract.Requires<ArgumentNullException>(network != null);
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(text));

			Network = network;
			Text = text;
			ResolvedFunctions = new List<FunctionGenerationInfo>();
			ResolvedPersons = new DistinctCollection<NetworkNode>();
			ResolvedLocatives = new DistinctCollection<NetworkNode>();
			ResolvedActions = new DistinctCollection<NetworkNode>();
			CurrentContextNodes = new DistinctCollection<NetworkNode>();
			Tales = new GenerationCollection();
		}
		#endregion
	}
}
