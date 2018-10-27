namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Threading;

	/// <copyright>Copyright © 2005 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	internal class CellOrderComparer : IComparer
	{

		#region IComparer Members

		public int Compare(object x, object y)
		{

			if (x is SpreadsheetCell && y is SpreadsheetCell)
			{

				SpreadsheetCell cell1 = (SpreadsheetCell)x;
				SpreadsheetCell cell2 = (SpreadsheetCell)y;

				// If the two rows are the same, compare the column.
				int compare = cell1.RowIndex.CompareTo(cell2.RowIndex);
				return compare == 0 ? cell1.ColumnIndex.CompareTo(cell2.ColumnIndex) : compare;

			}

			throw new Exception("The method or operation is not implemented for these types.");

		}

		#endregion
	}

}
