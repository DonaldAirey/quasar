namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;

	/// <summary>The column header is used to control the columns and the sorting that appears in a document.</summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public partial class ColumnCursor : Control
	{

		// Private Members
		private float scaleFactor;
		private Tile tile;
		private Style style;
		
		/// <summary>
		/// Create a cursor for moving columns around with drag-and-drop operations.
		/// </summary>
		/// <param name="bitmap">The bitmap to use for the cursor.</param>
		public ColumnCursor(Tile tile, Style style, float scaleFactor)
		{

			// IDE supported components
			InitializeComponent();

			// Initialize the object.  This is a top-level window, meaning it should sit on top of every other window.  The size of
			// the window is determined by size of the tile.
			this.tile = tile.Clone();
			this.style = style.Clone();
			this.scaleFactor = scaleFactor;
			this.SetTopLevel(true);
			this.Size = new Size(Convert.ToInt32(tile.RectangleF.Size.Width * this.scaleFactor),
				Convert.ToInt32(tile.RectangleF.Height * this.scaleFactor));
			this.Visible = true;

		}

		/// <summary>
		/// This is the offset to where the cursor 'Hot' point is.
		/// </summary>
		public Point Offset { get { return new Point(this.Width / 2, this.Height - 4); } }

		/// <summary>
		/// Sets the location of the column cursor with respect to the cursor's 'Hot' point.
		/// </summary>
		public new Point Location { set { value.X -= this.Width / 2; value.Y -= this.Height - 4; base.Location = value; } }

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
		protected override void OnPaint(PaintEventArgs e)
		{

			Bitmap paintBitmap = new Bitmap(e.ClipRectangle.Width, e.ClipRectangle.Height, e.Graphics);
			Graphics paintGraphic = Graphics.FromImage(paintBitmap);

			// The Graphics space is transformed to match the area where the invalid rectangle is located in document 
			// (world) coordinates. This allows the painting logic to simply write to the world coordinates.
			paintGraphic.Transform = new Matrix(this.scaleFactor, 0.0f, 0.0f, this.scaleFactor,
				Convert.ToSingle(-e.ClipRectangle.X - this.tile.RectangleF.X) * this.scaleFactor,
				Convert.ToSingle(-e.ClipRectangle.Y - this.tile.RectangleF.Y) * this.scaleFactor);

			// The rectangle and the style of the current tile are used several times below.
			RectangleF tileRectangle = this.tile.RectangleF;

			// Fill in the area to be redrawn with the background color specified by the style.
			paintGraphic.FillRectangle(style.InteriorBrush, tileRectangle);

			// Left Border: a polygon if joined with another border, a rectangle otherwise.
			if (style.LeftBorder != null)
				paintGraphic.FillPolygon(style.LeftBorder.Brush, new PointF[]
					{
						new PointF(tileRectangle.Left, tileRectangle.Top),
						new PointF(tileRectangle.Left + style.LeftBorder.Width,
							style.TopBorder == null ? tileRectangle.Top : tileRectangle.Top + style.LeftBorder.Width),
						new PointF(tileRectangle.Left + style.LeftBorder.Width,
							style.BottomBorder == null ? tile.RectangleF.Bottom : tile.RectangleF.Bottom - style.LeftBorder.Width),
						new PointF(tileRectangle.Left, tile.RectangleF.Bottom)
					});

			// Top Border: a polygon if joined with another border, a rectangle otherwise.
			if (style.TopBorder != null)
				paintGraphic.FillPolygon(style.TopBorder.Brush, new PointF[]
					{
						new PointF(tileRectangle.Left, tileRectangle.Top),
						new PointF(tileRectangle.Right, tileRectangle.Top),
						new PointF(style.RightBorder == null ? tileRectangle.Right :
							tileRectangle.Right - style.TopBorder.Width, tile.RectangleF.Top + style.TopBorder.Width),
						new PointF(style.LeftBorder == null ? tile.RectangleF.Left :
							tile.RectangleF.Left + style.TopBorder.Width, tile.RectangleF.Top + style.TopBorder.Width)
					});

			// Right Border: a polygon if joined with another border, a rectangle otherwise.
			if (style.RightBorder != null)
				paintGraphic.FillPolygon(style.RightBorder.Brush, new PointF[]
					{
						new PointF(tileRectangle.Right - style.RightBorder.Width,
							style.TopBorder == null ? tile.RectangleF.Top : tile.RectangleF.Top + style.RightBorder.Width),
						new PointF(tileRectangle.Right, tileRectangle.Top),
						new PointF(tileRectangle.Right, tile.RectangleF.Bottom),
						new PointF(tile.RectangleF.Right - style.RightBorder.Width,
							style.BottomBorder == null ? tile.RectangleF.Bottom : tile.RectangleF.Bottom - style.RightBorder.Width)
					});

			// Bottom Border: a polygon if joined with another border, a rectangle otherwise.
			if (style.BottomBorder != null)
				paintGraphic.FillPolygon(style.BottomBorder.Brush, new PointF[]
					{
						new PointF(style.LeftBorder == null ? tileRectangle.Left :
							tileRectangle.Left + style.BottomBorder.Width, tile.RectangleF.Bottom - style.BottomBorder.Width),
						new PointF(style.RightBorder == null ? tileRectangle.Right :
							tileRectangle.Right - style.BottomBorder.Width, tile.RectangleF.Bottom - style.BottomBorder.Width),
						new PointF(tileRectangle.Right, tile.RectangleF.Bottom),
						new PointF(tile.RectangleF.Left, tile.RectangleF.Bottom)
					});

			// The viewer displays both image and data.
			if (tile.Data is Bitmap || style.Image is Bitmap)
			{

				// This image will be displayed in the tile.
				Image image = style.Image == null ? tile.Data as Image : style.Image;

				// This is where the image will be placed within the tile unless an instruction below chooses a different
				// alignment.
				PointF location = new PointF(tileRectangle.X, tileRectangle.Y);

				// This will align the image horizontally.
				switch (style.StringFormat.Alignment)
				{
				case StringAlignment.Center:

					// This will center the image horizontally in the tile.
					location.X = tileRectangle.X + (tileRectangle.Width - image.Width) / 2.0f;
					break;

				case StringAlignment.Far:

					// This will align the image with the right side of the tile.
					location.X = tileRectangle.Right - image.Width;
					break;

				}

				// This will align the image vertically.
				switch (style.StringFormat.LineAlignment)
				{

				case StringAlignment.Center:

					// This will center the image vertically in the tile.
					location.Y = tileRectangle.Y + (tileRectangle.Height - image.Height) / 2.0f;
					break;

				case StringAlignment.Far:

					// This will align the image with the bottom of the tile.
					location.Y = tileRectangle.Bottom - image.Height;
					break;

				}

				// Draw the image in the tile's rectangle with the alignment specified.
				paintGraphic.DrawImage(image, new RectangleF(location, image.Size));

			}
			else
			{

				// Draw the formatted data in the given text box with the alignment specified in the stylesheet.
				paintGraphic.DrawString(string.Format(style.NumberFormat, tile.Data), style.Font, style.FontBrush,
					tileRectangle, style.StringFormat);

			}

			// Draw the final composit image to the device.  Doing this in one operation effectively eliminates the flicker
			// that multiple updates to the screen device would cause.
			e.Graphics.DrawImage(paintBitmap, e.ClipRectangle);

			// Return the resources used to paint the image back to the GDI.  Because of the potential frequency of the drawing
			// operations and the relatively large resources needed to paint the document, these are forced back into the free
			// pool immediately rather than waiting for garbage collection to come along and clean up.
			paintGraphic.Dispose();
			paintBitmap.Dispose();

		}

	}

}
