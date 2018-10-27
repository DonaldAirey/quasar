namespace MarkThree.Forms
{

	using System;
	using System.Windows.Forms;

	/// <summary>
	/// This event is called when the title of a viewer has changed.
	/// </summary>
	public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs titleChangedEventArgs);
	
	/// <summary>
	/// Used to pass argument through the 'TextChanged' event handler.
	/// </summary>
	public class TextChangedEventArgs : EventArgs
	{

		// Private Members
		private string title;

		/// <summary>
		/// The cell where the editing took place.
		/// </summary>
		public string Title {get {return this.title;} set {this.title = value;}}
        		
		/// <summary>
		/// Creates event argument for the 'TextChanged' event.
		/// </summary>
		/// <param name="title">The title of the viewer.</param>
		public TextChangedEventArgs(string title)
		{
			
			// Initialize the members.
			this.title = title;
		
		}

	}

}
