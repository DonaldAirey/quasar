namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Data;
	using System.Windows.Forms;

	/// <summary>The column header is used to control the columns and the sorting that appears in a document.</summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ColumnCursor : System.Windows.Forms.Control
	{

		[Browsable(false)]
		public System.Drawing.Bitmap Bitmap;

		/// <summary>
		/// Create a cursor for moving columns around with drag-and-drop operations.
		/// </summary>
		/// <param name="bitmap">The bitmap to use for the cursor.</param>
		public ColumnCursor(Bitmap bitmap)
		{

			// Initialize the object.  This is a top-level window, meaning it should sit on top of every other window.
			this.Bitmap = bitmap;
			this.SetTopLevel(true);
			this.Size = bitmap.Size;
			this.Visible = true;

		}

		#region Dispose Method
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{

			// Destroy the resources
			if (disposing)
				this.Bitmap.Dispose();

			// Make sure the base class can release resources.
			base.Dispose(disposing);

		}
		#endregion

		/// <summary>
		/// This is the offset to where the cursor 'Hot' point is.
		/// </summary>
		public Point Offset {get {return new Point(this.Width / 2, this.Height - 4);}}

		/// <summary>
		/// Sets the location of the column cursor with respect to the cursor's 'Hot' point.
		/// </summary>
		public new Point Location
		{

			set
			{
				value.X -= this.Width / 2;
				value.Y -= this.Height - 4;
				base.Location = value;
			}

		}

		/// <summary>
		/// The creation parameters for the column cursor.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{

				// Thiw will create a window that floats above all other windows, has no task list, edges or other trappings of windows.
				CreateParams createParams = new CreateParams();
				createParams.ClassName = base.CreateParams.ClassName;
				createParams.Style = unchecked((int)(WindowStyle.Popup | WindowStyle.Clipsiblings | WindowStyle.Clipchildren | WindowStyle.Disabled));
				createParams.ExStyle = unchecked((int)(ExtendedWindowStyle.Windowedge | ExtendedWindowStyle.Toolwindow | ExtendedWindowStyle.Topmost));
				return createParams;

			}

		}

		/// <summary>
		/// Paints the column cursor.
		/// </summary>
		/// <param name="paintEventArgs">Arguments for painting in the graphics context.</param>
		protected override void OnPaint(PaintEventArgs paintEventArgs)
		{

			// Paint the bitmap in the window.
			paintEventArgs.Graphics.DrawImage(this.Bitmap, this.ClientRectangle);

		}

	}

}
