namespace MarkThree
{

	using System;
	using System.Data;

	/// <summary>
	/// Represents a row of data in a MarkThree.Table
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class IndexRow : TransactionRow
	{

		/// <summary>
		/// Creates a row.
		/// </summary>
		/// <param name="dataRowBuilder">A DataRowBuilder used to construct the row from the column information.</param>
		internal IndexRow(System.Data.DataRowBuilder dataRowBuilder) : 
			base(dataRowBuilder)
		{

			// This constructor is left intensionally blank.

		}

		/// <summary>
		/// Gets the MarkThree.Table for which this row has a schema.
		/// </summary>
		internal IndexTable IndexTable { get { return (this as DataRow).Table as IndexTable; } }

		/// <summary>
		/// The unique identifier of a row.
		/// </summary>
		public Row Row
		{
			get { return base[this.IndexTable.RowColumn] as Row; }
			set { base[this.IndexTable.RowColumn] = value; }
		}

	}
    
}
