namespace MarkThree.Forms
{

	using System;

	/// <summary>
	/// Arguments to begin the edit mode for cells in a spreadsheet.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class StartEditArgs
	{

		private bool isNavigationAllowed;
		private bool isTextSelected;
		private string initialText;

		/// <summary>
		/// Gets an indicator of whether navigation is allowed inside the edit control.
		/// </summary>
		public bool IsNavigationAllowed {get {return this.isNavigationAllowed;}}

		/// <summary>
		/// Gets an indicator of whether the text is selected when the control is activated.
		/// </summary>
		public bool IsTextSelected {get {return this.isTextSelected;}}

		/// <summary>
		/// Gets the initial text for the control when it is activated.
		/// </summary>
		public string InitialText {get {return this.initialText;}}

		/// <summary>
		/// Creates an object used to initiate the edit mode for cells.
		/// </summary>
		/// <param name="isNavigationAllowed">True to allow in-cell editing.</param>
		/// <param name="isTextSelected">True to select the initialization text.</param>
		/// <param name="initialText">The initial text that appears in the edit control.</param>
		public StartEditArgs(bool isNavigationAllowed, bool isTextSelected, string initialText)
		{

			// Initialize the members
			this.isNavigationAllowed = isNavigationAllowed;
			this.isTextSelected = isTextSelected;
			this.initialText = initialText;

		}

	}

}
