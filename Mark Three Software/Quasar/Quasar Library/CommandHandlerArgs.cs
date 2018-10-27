/*************************************************************************************************************************
*
*	File:			CommandHandlerArgs.cs
*	Description:	This class is used to pass a command handler and it's argument to the command thread.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	internal class CommandHandlerArgs
	{

		private ThreadHandler threadHandler;
		private object[] argument;

		/// <summary>
		/// The command handler
		/// </summary>
		public ThreadHandler ThreadHandler {get {return this.threadHandler;}}

		/// <summary>
		/// The argument element of the command, usually a unique identifier of the object of the command.
		/// </summary>
		public object[] Argument {get {return this.argument;}}

		/// <summary>
		/// Constructs argument to be passed to the thread that handles argument.
		/// </summary>
		/// <param name="ThreadHandler">The method that handles the command.</param>
		/// <param name="argument">Variable list of argument to be passed to the command handler.</param>
		public CommandHandlerArgs(ThreadHandler ThreadHandler, object[] argument)
		{

			// Initialize the members
			this.threadHandler = ThreadHandler;
			this.argument = argument;

		}

	}

}
