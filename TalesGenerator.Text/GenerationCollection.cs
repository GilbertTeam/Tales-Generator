using System.Linq;

namespace TalesGenerator.Text
{
	public class GenerationCollection : BaseCollection<TaleGenerationInfo>
	{
		#region Fields

		private int _currentIndex = -1;

		private TaleGenerationInfo _predTaleInfo;
		#endregion

		#region Methods

		public TaleGenerationInfo GetCurrentTale()
		{
			if (_currentIndex >= Count)
			{
				return null;
			}

			if (_currentIndex == -1)
			{
				_predTaleInfo = List[++_currentIndex];

				return _predTaleInfo;
			}

			TaleGenerationInfo currentTaleInfo = List[_currentIndex];

			if (currentTaleInfo.ConflictSets.Count == 0)
			{
				_currentIndex++;
			}
			else if (currentTaleInfo.ConflictSets.Any(conflictSet => !conflictSet.Closed))
			{
				if (currentTaleInfo == _predTaleInfo)
				{
					currentTaleInfo.ConflictSets.Toggle();
				}
			}
			else
			{
				_currentIndex++;
			}

			if (_currentIndex < Count)
			{
				_predTaleInfo = List[_currentIndex];

				return _predTaleInfo;
			}
			else
			{
				return null;
			}
		}
		#endregion
	}
}
