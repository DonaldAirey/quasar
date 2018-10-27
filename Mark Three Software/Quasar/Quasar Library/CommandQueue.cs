/*************************************************************************************************************************
*
*	File:			CommandQueue.cs
*	Description:	Passes commands to a thread queue which executes them sequentially in the background.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace MarkThree.Quasar
{

	using System;
	using System.Diagnostics;
	using System.Threading;

	/// <summary>
	/// Summary description for CommandQueue.
	/// </summary>
	public class CommandQueue : ThreadQueue
	{

		/// <summary>
		/// Executes a command on the command thread.
		/// </summary>
		/// <param name="command">The command to be executed.</param>
		public void Execute(ThreadHandler commandHandler, params object[] argument)
		{

			// Execute the command sequentially on the command thread.  Remember that 'ThreadQueues' execute the commands
			// in the order they were placed on the queue.  The descriptive name of the thread is useful for debugging,
			// especially when several command threads are queued.
			this.Enqueue(commandHandler, argument);

		}

	}

}
