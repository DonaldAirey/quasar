namespace MarkThree.Forms
{

	using System;
	using System.Data;
	using System.Drawing;

	/// <summary>
	/// A column of data and formatting in the spreadsheet.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class SpreadsheetColumn : DataColumn
	{

		// Private Members
		private MarkThree.Forms.Style style;
		private System.Drawing.Image image;
		private System.Drawing.Rectangle rectangle;
		private System.String description;

		// Internal Members
		internal System.Int32 ColumnIndex;
		internal System.Int32 ColumnViewIndex;

		/// <summary>
		/// Create a column.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		public SpreadsheetColumn()
		{

			// Initialize the object
			base.DataType = typeof(System.Object);
			this.ColumnIndex = 0;
			this.ColumnViewIndex = 0;
			this.style = null;
			this.image = null;
			this.rectangle = Rectangle.Empty;
			this.description = string.Empty;

		}


		/// <summary>
		/// Create a column.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		public SpreadsheetColumn(string columnName) : base(columnName, typeof(System.Object))
		{

			// Initialize the object
			this.style = null;
			this.image = null;
			this.rectangle = Rectangle.Empty;
			this.description = string.Empty;
		
		}

		/// <summary>
		/// Gets or sets a description for the column.
		/// </summary>
		public string Description { get { return this.description; } set { this.description = value; } }

		/// <summary>
		/// Gets or sets the rectangle occupied by this column on the display.
		/// </summary>
		public Rectangle Rectangle { get { return this.rectangle; } set { this.rectangle = value; } }

		/// <summary>
		/// Gets or sets the style used to display the heading of this column.
		/// </summary>
		public Style Style { get { return this.style; } set { this.style = value; } }

		/// <summary>
		/// Indicates whether the column contains a point.
		/// </summary>
		/// <param name="point">A location on the display surface.</param>
		/// <returns>true if the column header contains the point.</returns>
		public bool Contains(Point point) { return this.rectangle.Contains(point); }

		/// <summary>
		/// Indicates whether the column contains a rectangle.
		/// </summary>
		/// <param name="rectangle">A rectangle on the display surface.</param>
		/// <returns>true if column header contains the rectangle.</returns>
		public bool Contains(Rectangle rectangle) { return this.rectangle.Contains(rectangle); }

		/// <summary>
		/// Gets or sets the image used in the column header.
		/// </summary>
		public Image Image { get { return this.image; } set { this.image = value; } }
	
	}

}
