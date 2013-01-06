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

		public ICollection<FunctionResolveContext> ResolvedFunctions { get; private set; }

		public ICollection<TaleGenerationInfo> GenerationList { get; private set; }

		public ICollection<NetworkNode> Persons { get; private set; }

		public ICollection<NetworkNode> Locatives { get; private set; }

		public ICollection<NetworkNode> Actions { get; private set; }
		#endregion

		#region Constructors

		public TextGenerationContext(TalesNetwork network, string text)
		{
			Contract.Requires<ArgumentNullException>(network != null);
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(text));

			Network = network;
			Text = text;
			ResolvedFunctions = new List<FunctionResolveContext>();
			GenerationList = new List<TaleGenerationInfo>();
			Persons = new List<NetworkNode>();
			Locatives = new List<NetworkNode>();
			Actions = new List<NetworkNode>();
		}
		#endregion
	}
}
