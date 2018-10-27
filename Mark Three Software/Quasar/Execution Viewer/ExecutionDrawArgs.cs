/*************************************************************************************************************************
*
*	File:			ExecutionDrawArgs.cs
*	Description:	An execution record.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace Shadows.Quasar.Viewers.Execution
{

	/// <summary>
	/// Passes parameters to the ExecutionRefresh Thread.
	/// </summary>
	class ExecutionDrawArgs
	{

		private ExecutionSet executionSet;

		/// <summary>
		/// The local executions
		/// </summary>
		public ExecutionSet ExecutionSet {get {return this.executionSet;}}

		/// <summary>
		/// Constructor used to pass argument to the 'ExecutionRefresh' thread.
		/// </summary>
		public ExecutionDrawArgs(ExecutionSet executionSet)
		{

			// Initialize members.
			this.executionSet = executionSet;

		}

	}

}
