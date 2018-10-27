namespace MarkThree.Forms
{

	using System;
	using System.Drawing;

	/// <summary>Arguments for when a region of the spreadsheet has changed.</summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class InvalidRegionEventArgs : EventArgs
	{

		private System.Drawing.Region region;

		/// <summary>The region of the spreadsheet that need to be redrawn.</summary>
		public Region Region {get {return this.region;} set {this.region = value;}}

		/// <summary>
		/// Creates an argument to indicate that part of the spreadsheet needs to be redrawn.
		/// </summary>
		/// <param name="region">The invalid part of the spreadsheet.</param>
		public InvalidRegionEventArgs(Region region)
		{

			// Initialize the object
			this.region = region;

		}

	}
	
}
