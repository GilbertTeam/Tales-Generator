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

		private readonly List<FunctionNode> _functions;

		private int _currentIndex;
		#endregion

		#region Properties

		public FunctionType FunctionType { get; private set; }

		public bool Closed
		{
			get { return _currentIndex == _functions.Count - 1; }
		}

		public FunctionNode CurrentFunction
		{
			get
			{
				Contract.Assume(_currentIndex < _functions.Count);

				return _functions[_currentIndex]; }
		}
		#endregion

		#region Constructors

		public FunctionConflictSet(IEnumerable<FunctionNode> functions)
		{
			Contract.Requires<ArgumentNullException>(functions != null);
			Contract.Requires<ArgumentException>(functions.Any());

			_functions = new List<FunctionNode>(functions);
			_currentIndex = 0;
			FunctionType = _functions.First().FunctionType;
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
		#endregion
	}
}
