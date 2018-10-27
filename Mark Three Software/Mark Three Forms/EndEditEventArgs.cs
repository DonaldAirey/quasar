namespace MarkThree.Forms
{

	using System;
	using System.Windows.Forms;

	/// <summary>
	/// This event is called when the user has accepted input from the spreadsheet edit control.
	/// </summary>
	public delegate void EndEditEventHandler(object sender, EndEditEventArgs e);
	
	/// <summary>
	/// Used to pass argument through the 'EndEdit' event handler.
	/// </summary>
	public class EndEditEventArgs : EventArgs
	{

		private object result;
		private bool handled;

		/// <summary>
		/// Indicates whether the navigation keys were handled.
		/// </summary>
		public bool Handled {get {return this.handled;} set {this.handled = value;}}

		/// <summary>
		/// The text entered by the user.
		/// </summary>
		public object Result {get {return this.result;}}

		/// <summary>
		/// Creates event argument for the 'EndEdit' event when the user has completed the input action.
		/// </summary>
		/// <param name="key">The key that terminated the edit mode.</param>
		/// <param name="text">The text of the cell.</param>
		public EndEditEventArgs(object result)
		{
			
			// Initialize the members.
			this.result = result;
			this.handled = false;
		
		}

	}

}
