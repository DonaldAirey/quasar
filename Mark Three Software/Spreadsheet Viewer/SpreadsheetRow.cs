namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;

	/// <summary>
	/// A horizontal organization of elements in a cartesian product of data and styles.
	/// </summary>
	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class SpreadsheetRow : DataRow
	{

		// Private Members
		private MarkThree.Forms.SpreadsheetCell[] spreadsheetCells;

		// Internal Members
		internal System.Boolean IsVisible;
		internal System.Int32 RowIndex;
		internal System.Int32 RowViewIndex;
		internal System.Drawing.Rectangle rectangle;

		/// <summary>
		/// Create an organization of spreadsheet cells.
		/// </summary>
		/// <param name="dataRowBuilder">An object used to build the row.</param>
		public SpreadsheetRow(DataRowBuilder dataRowBuilder) : base(dataRowBuilder)
		{

			// Initialize the object.
			this.RowIndex = 0;
			this.RowViewIndex = 0;
			this.IsVisible = true;
			this.rectangle = Rectangle.Empty;

			// The cells are a combination of data and a reference to an item in a DataRow of a DataTable.  The DataTable needs to 
			// remain relatively pure so the expression operator can work on the row filters and computed column.  So a
			// SpreadsheetCell is layered on top of the cells.  The SpreadsheetCell contains some of the housekeeping members
			// needed (styles, states, etc.) and refers to an underlying item in a DataTable.
			ArrayList arrayList = new ArrayList();
			foreach (SpreadsheetColumn spreadsheetColumn in this.Table.Columns)
				arrayList.Add(new SpreadsheetCell(this, spreadsheetColumn));
			this.spreadsheetCells = (SpreadsheetCell [])arrayList.ToArray(typeof(SpreadsheetCell));
		
		}

		public SpreadsheetCell[] Cells
		{
			get { return this.spreadsheetCells; }
		}

		/// <summary>
		/// Sets or gets the SpreadsheetCell at a given column index.
		/// </summary>
		/// <param name="columnIndex">The column index of the requested cell.</param>
		/// <returns>The cell at the given column index.</returns>
		public new SpreadsheetCell this[int columnIndex]
		{
			get { return this[this.Table.Columns[columnIndex]]; }
			set { this[this.Table.Columns[columnIndex]] = value; }
		}

		/// <summary>
		/// Sets or gets the SpreadsheetCell at a given column.
		/// </summary>
		/// <param name="dataColumn">A SpreadsheetColumn that gives the offset to the cell.</param>
		/// <returns>The SpreadsheetCell at the given column.</returns>
		public new SpreadsheetCell this[DataColumn dataColumn]
		{
			get { return this.spreadsheetCells[dataColumn.Ordinal]; }
			set { this.spreadsheetCells[dataColumn.Ordinal] = value; }
		}

	}

}
