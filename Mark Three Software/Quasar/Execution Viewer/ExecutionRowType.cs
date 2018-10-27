using System;

namespace Shadows.Quasar.Viewers.Execution
{

	/// <summary>
	/// Enumeration for the row types in the placement screen.
	/// </summary>
	class ExecutionRowType
	{

		/// <summary>Unused Row</summary>
		public const int Unused = 0;
		/// <summary>An Execution Detail Line.</summary>
		public const int GlobalExecution = 1;
		/// <summary>An Execution Detail Line.</summary>
		public const int LocalExecution = 2;
		/// <summary>A placeholder for the prompting text.</summary>
		public const int Placeholder = 3;

	};

}
