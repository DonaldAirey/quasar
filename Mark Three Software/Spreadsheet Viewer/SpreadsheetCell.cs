namespace MarkThree.Forms
{

	using System;
	using System.Data;
	using System.Drawing;

	/// <summary>
	/// A unit of data in a spreadsheet.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class SpreadsheetCell
	{

		// Private Members
		private MarkThree.Forms.SpreadsheetRow spreadsheetRow;
		private MarkThree.Forms.SpreadsheetColumn spreadsheetColumn;
		private MarkThree.Forms.Style style;
		internal MarkThree.Forms.Style[] AnimationArray = null;
		private System.Boolean isModified;
		private System.Boolean isSelected;
		private System.Boolean isActiveCell;
		private System.Drawing.Rectangle displayRectangle;
		private System.Drawing.Rectangle printerRectangle;
		internal System.Int32 AnimationIndex;

		/// <summary>
		/// Create a Spreadsheet Cell.
		/// </summary>
		/// <param name="spreadsheetRow">The spreadsheet row associated with this cell.</param>
		/// <param name="spreadsheetColumn">The spreadsheet column associated with this cell.</param>
		public SpreadsheetCell(SpreadsheetRow spreadsheetRow, SpreadsheetColumn spreadsheetColumn)
		{

			// Initialize the Object.
			this.spreadsheetRow = spreadsheetRow;
			this.spreadsheetColumn = spreadsheetColumn;
			this.style = spreadsheetColumn.Style;
			this.IsModified = true;

		}

		public int RowIndex { get { return this.spreadsheetRow.RowIndex; } }

		public int ColumnIndex { get { return this.spreadsheetColumn.ColumnIndex; } }

		public int RowViewIndex { get { return this.spreadsheetRow.RowViewIndex; } }

		public int ColumnViewIndex { get { return this.spreadsheetColumn.ColumnViewIndex; } }

		/// <summary>
		/// Indicates whether the cell is part of a selection of cells.
		/// </summary>
		public bool IsSelected { get { return this.isSelected; } set { this.isSelected = value; } }

		/// <summary>
		/// Indicates whether the cell is the active cell.
		/// </summary>
		public bool IsActiveCell { get { return this.isActiveCell; } set { this.isActiveCell = value; } }

		/// <summary>
		/// Indicates whether the cell has been modified.
		/// </summary>
		public bool IsModified { get { return this.isModified; } set { this.isModified = value; } }

		/// <summary>
		/// Gets or sets the data in the cell.
		/// </summary>
		public object Value
		{

			get
			{

				// Get the data from the underlying data table.
				return ((DataRow)this.spreadsheetRow)[this.spreadsheetColumn.Ordinal];

			}

			set
			{

				// Set the data in the underlying data table.
				((DataRow)this.spreadsheetRow)[this.spreadsheetColumn.Ordinal] = value;

			}

		}

		/// <summary>
		/// Gets or sets the style used to display the data in the cell.
		/// </summary>
		public Style Style {get {return this.style;} set {this.style = value;}}

		/// <summary>
		/// Gets or sets the rectangle occupied by this cell on the display surface.
		/// </summary>
		public Rectangle DisplayRectangle {get {return this.displayRectangle;} set {this.displayRectangle = value;}}

		/// <summary>
		/// The rectangle occupied by this cell on the printed surface.
		/// </summary>
		public Rectangle PrinterRectangle {get {return this.printerRectangle;} set {this.printerRectangle = value;}}

	}

}
