/*************************************************************************************************************************
*
*	File:			CommandArgs.cs
*	Description:	This class is used to pass a command to the command thread.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	public class CommandArgs
	{

		private int command;
		private object key;
		private object argument;

		/// <summary>
		/// The command.
		/// </summary>
		public int Command {get {return this.command;}}

		/// <summary>
		/// The key element of the command, usually a unique identifier of the object of the command.
		/// </summary>
		public object Key {get {return this.key;}}

		/// <summary>
		/// The parameters for the command.
		/// </summary>
		public object Argument {get {return this.argument;}}

		/// <summary>
		/// Creates a structure which can be used to process a generic command.
		/// </summary>
		/// <param name="key">Key value of the target of the command</param>
		/// <param name="argument">A value associated with this command</param>
		public CommandArgs(int command, object key, object argument)
		{

			// Initialize the members
			this.command = command;
			this.key = key;
			this.argument = argument;

		}

	}

}
