namespace MarkThree.Forms
{

	using System;
	using System.Drawing;
	using System.Windows.Forms;

	/// <summary>
	/// Used to direct the mouse events to a quadrant of the viewer.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class QuadrantMouseEventArgs
	{

		// Public Members
		public MouseButtons Button;
		public PointF Location;

		/// <summary>
		/// Used to direct mouse events to a quadrant of the viewer.
		/// </summary>
		/// <param name="button">The buttons pressed on the mouse.</param>
		/// <param name="location">The location of the mouse in document coordinates.</param>
		public QuadrantMouseEventArgs(MouseButtons button, PointF location)
		{

			// Initialize the object.
			this.Button = button;
			this.Location = location;

		}

	}

}
