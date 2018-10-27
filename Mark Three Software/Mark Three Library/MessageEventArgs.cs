using System;

namespace MarkThree
{

	/// <summary>
	/// Delegate for handling message events.
	/// </summary>
	public delegate void MessageEventHandler(object sender, MessageEventArgs e);

	/// <summary>
	/// Message Event Arguments
	/// </summary>
	public class MessageEventArgs : EventArgs
	{

		private string message;

		/// <summary>
		/// The text of the message.
		/// </summary>
		public string Message {get {return this.message;}}

		/// <summary>
		/// Construct the argument for a message event.
		/// </summary>
		/// <param name="message">The text of the message.</param>
		public MessageEventArgs(string message)
		{

			// Initialize members.
			this.message = message;

		}

	}

}
