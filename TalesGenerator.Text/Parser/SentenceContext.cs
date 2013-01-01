using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TalesGenerator.Text.Parser
{
	public class SentenceContext : IEnumerable<SentenceToken>
	{
		#region IEnumerable<SentenceToken> Members

		public IEnumerator<SentenceToken> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
