namespace MarkThree
{

	using System;
	using System.Data;
	using System.Collections.Generic;
	using System.Threading;

	/// <summary>
	/// Maps key elements to a row in a data table.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	internal class IndexTable : DataTable
	{

		// Public Readonly Properties
		internal readonly DataColumn RowColumn;

		/// <summary>
		/// Create a table that is used to map the elements of a key to a row in a data table.
		/// </summary>
		/// <param name="index">The index to which this mapping table belongs.</param>
		internal IndexTable(Column[] columns)
		{

			// The index table is used to map a key (an array of objects) to a row in the data table.  The primary key of this
			// table is a copy of the columns that make up the key in the data table.
			DataColumn[] keyColumns = new DataColumn[columns.Length];
			for (int index = 0; index < columns.Length; index++)
				keyColumns[index] = new DataColumn(columns[index].ColumnName, columns[index].DataType);
			this.Columns.AddRange(keyColumns);

			// The primary key of the index table is the copy of the key columns from the original table.  This mimics the way a
			// relational database is able to use indices to find the actual chunks of data using the keys.
			this.PrimaryKey = keyColumns;

			// Setting the primary key will override the 'AllowDBNull' setting in a column.  This will set it back to the attribute
			// given to the column by the schema.
			for (int index = 0; index < columns.Length; index++)
				keyColumns[index].AllowDBNull = columns[index].AllowDBNull;

			// This is the other part of the mapping: the actual row where the data can be found.  When a fast search is required
			// to find a row in a table when only the key is known, this index table is searched using the primary key.  If a row 
			// is found, the 'Row' column contains a reference to the data table where the row can be found.
			this.RowColumn = new DataColumn("Row", typeof(Row));
			this.Columns.Add(this.RowColumn);

		}

		/// <summary>
		/// Creates a new row for the index table.
		/// </summary>
		/// <returns>A new IndexRow with the same schema as the table.</returns>
		internal new IndexRow NewRow() { return base.NewRow() as IndexRow; }

		/// <summary>
		/// Builds a new IndexRow.
		/// </summary>
		/// <param name="dataRowBuilder">The System.Data.DataRowBuilder supports the .NET Framework and is not to be used directly
		/// from your code.</param>
		/// <returns>A new IndexRow with the same schema as the table.</returns>
		protected override System.Data.DataRow NewRowFromBuilder(System.Data.DataRowBuilder dataRowBuilder)
		{
			return new IndexRow(dataRowBuilder);
		}

	}
    
}
