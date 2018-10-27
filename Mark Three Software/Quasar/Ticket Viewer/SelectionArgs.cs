/*************************************************************************************************************************
*
*	File:			SelectionArgs.cs
*	Description:	This class is used to address a cell in a spreadsheet.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace Shadows.Quasar.Viewers.Ticket
{

	class SelectionArgs
	{

		private bool containsFocus;
		private int rowIndex;
		private int columnIndex;

		/// <summary>
		/// Indicates whether the window had the focus when the selection was made
		/// </summary>
		public bool ContainsFocus {get {return this.containsFocus;}}

		/// <summary>
		/// Index of the row
		/// </summary>
		public int RowIndex {get {return this.rowIndex;}}

		/// <summary>
		/// Index of the column
		/// </summary>
		public int ColumnIndex {get {return this.columnIndex;}}

		/// <summary>
		/// Creates a SelectionArg
		/// </summary>
		/// <param name="rowIndex">Index of the row</param>
		/// <param name="columnIndex">Index of the column</param>
		public SelectionArgs(int rowIndex, int columnIndex, bool containsFocus)
		{

			// Initialize the members
			this.containsFocus = containsFocus;
			this.rowIndex = rowIndex;
			this.columnIndex = columnIndex;

		}

	}
		
}
