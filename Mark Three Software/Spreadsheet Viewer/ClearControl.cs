namespace MarkThree.Forms
{

	using System;
	using System.Drawing;
	using System.Windows.Forms;

	/// <summary>A panel that doesn't attempt to clear the background.</summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ClearControl : System.Windows.Forms.Control
	{

		/// <summary>
		/// Paints the background of the client area.
		/// </summary>
		/// <param name="pevent">Paint event arguments.</param>
		protected override void OnPaintBackground(PaintEventArgs paintEventArgs)
		{

			// This is intentionally left blank to prevent the base class from painting the window with the background color.
			// While this is handy for simple controls, it produces an unnecessary flicker with high performance controls.

		}

	}

}

