namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Threading;

	/// <summary>
	/// Used to find records in a table using one or more values as a key.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Index
	{

		// Private Members
		private Column[] columns;
		private DataView dataView;

		/// <summary>
		/// Creates an index on a table.
		/// </summary>
		/// <param name="name">The name of the index.</param>
		/// <param name="columns">The set of columns used in the key for this index.</param>
		public Index(string name, Column[] columns)
		{

			this.columns = columns;

			// Make sure that columns were specified in the constructor.
			if (columns.Length == 0)
				throw new Exception("An index must reference columns in a table.");

			// Initialize the object.
			Table table = columns[0].Table;
			this.dataView = new DataView(table);

			// Create the sort order used for finding rows.
			string sort = string.Empty;
			for (int columnIndex = 0; columnIndex < columns.Length; columnIndex++)
			{
				Column column = columns[columnIndex];

				sort += string.Format("{0}", column.ColumnName);
				if (columnIndex < columns.Length - 1)
					sort += ",";
			}
			this.dataView.Sort = sort;

		}

		public Column[] Columns { get { return this.columns; } }

		/// <summary>
		/// Indicates whether this index uses the given columns as a key.
		/// </summary>
		/// <param name="columns">The set of columns composing a key.</param>
		/// <returns>true if this index uses the given columns to find rows in a table, otherwise false.</returns>
		public bool Contains(Column[] columns)
		{

			// There's no chance that this is the right index if the number of columns doesn't match.
			if (this.columns.Length != columns.Length)
				return false;

			// This will reject any combination of columns that don't exactly matchin the columns used to create this index.
			for (int index = 0; index < columns.Length; index++)
				if (this.columns[index] != columns[index])
					return false;

			// At this point, the set of given columns exactly matches the ones used to create this index.  That is, this index can
			// be used to look up rows in a table given data in the set of columns passed to this method.
			return true;

		}

		/// <summary>
		/// Gets the row that contains the specified key values.
		/// </summary>
		/// <param name="key">A single key value.</param>
		/// <returns>The row containing the key value or null if there are no rows that have the key value.</returns>
		public Row Find(object key)
		{

			// This will find the row in the data table that contains the key value or return a null to indicate the key isn't
			// present in the index.  The index contains a mapping between the key elements and a row in the data table.
			int index = this.dataView.Find(key);
			return index == -1 ? null : (this.dataView[index].Row as Row);

		}

		/// <summary>
		/// Gets the row that contains the specified key values.
		/// </summary>
		/// <param name="key">A array of key values.</param>
		/// <returns>The row containing the key values or null if there are no rows that have the key values.</returns>
		public Row Find(object[] keys)
		{

			// This will find the row in the data table that contains the key value or return a null to indicate the key isn't
			// present in the index.  The index contains a mapping between the key elements and a row in the data table.
			int index = this.dataView.Find(keys);
			return index == -1 ? null : (this.dataView[index].Row as Row);

		}

	}

}

