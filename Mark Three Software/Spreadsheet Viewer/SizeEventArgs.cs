namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>Arguments for when the size of the spreadsheet has changed.</summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class SizeEventArgs : EventArgs
	{

		private System.Drawing.Size size;

		/// <summary>The size of the spreadsheet.</summary>
		public Size Size {get {return this.size;} set {this.size = value;}}

		/// <summary>
		/// Creates an argument that gives the new size of the spreadsheet.
		/// </summary>
		/// <param name="size">The size of the spreadsheet.</param>
		public SizeEventArgs(Size size)
		{

			// Initialize the object
			this.size = size;

		}

	}

}
