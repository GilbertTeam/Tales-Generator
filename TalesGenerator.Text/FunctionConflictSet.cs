using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using TalesGenerator.TaleNet;

namespace TalesGenerator.Text
{
	internal class FunctionConflictSet
	{
		#region Fields

		private readonly List<FunctionGenerationInfo> _functions;

		private int _currentIndex;
		#endregion

		#region Properties

		public FunctionType FunctionType { get; private set; }

		public bool Closed
		{
			get { return _currentIndex == _functions.Count - 1; }
		}

		public FunctionGenerationInfo CurrentFunction
		{
			get
			{
				Contract.Assume(_currentIndex < _functions.Count);

				return _functions[_currentIndex]; }
		}
		#endregion

		#region Constructors

		public FunctionConflictSet(IEnumerable<FunctionGenerationInfo> functions)
		{
			Contract.Requires<ArgumentNullException>(functions != null);
			Contract.Requires<ArgumentException>(functions.Any());

			_functions = new List<FunctionGenerationInfo>(functions);
			_currentIndex = 0;
			FunctionType = _functions.First().Function.FunctionType;
		}
		#endregion

		#region Methods

		public void Reset()
		{
			_currentIndex = 0;
		}

		public void Next()
		{
			if (!Closed)
			{
				_currentIndex++;
			}
		}

		public override string ToString()
		{
			return string.Format(
				"Function type = {0}. Current index = {1}. Count = {2}.",
				FunctionType,
				_currentIndex,
				_functions.Count);
		}
		#endregion
	}
}
