namespace MarkThree.Forms
{

	using MarkThree.Forms;
	using System;
	using System.Windows.Forms;

	/// <summary>
	/// This event is called when the user has accepted input from the spreadsheet edit control.
	/// </summary>
	public delegate void NavigationEventHandler(object sender, NavigationEventArgs e);
	
	/// <summary>
	/// Used to pass argument through the 'NavigateKey' event handler.
	/// </summary>
	public class NavigationEventArgs : EventArgs
	{

		private NavigationCommand navigationCommand;

		/// <summary>
		/// The direction to move the spreadsheet control.
		/// </summary>
		public NavigationCommand NavigationCommand {get {return this.navigationCommand;}}

		/// <summary>
		/// Creates event argument for the 'NavigateKey' event when the user has completed the input action.
		/// </summary>
		/// <param name="key">The key that terminated the edit mode.</param>
		/// <param name="text">The text of the cell.</param>
		public NavigationEventArgs(NavigationCommand navigationCommand)
		{
			
			// Initialize the members.
			this.navigationCommand = navigationCommand;
		
		}

	}

}
