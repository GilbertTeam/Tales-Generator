using System;

namespace TalesGenerator.Text
{
	public class FunctionConflictSetCollection : BaseCollection<FunctionConflictSet>
	{
		#region Methods

		public void Toggle()
		{
			foreach (FunctionConflictSet conflictSet in this)
			{
				if (!conflictSet.Closed)
				{
					conflictSet.Next();

					break;
				}

				conflictSet.Reset();
			}
		}
		#endregion
	}
}
