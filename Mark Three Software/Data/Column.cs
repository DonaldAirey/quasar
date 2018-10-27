namespace MarkThree
{

	using System;
	using System.Data;

	/// <summary>
	/// A Column of data in a Table.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Column : DataColumn
	{

        // Public Members
		public bool IsPersistent;

		/// <summary>
		/// Creates a column.
		/// </summary>
		public Column()
		{

			// Initialize the object.
			this.IsPersistent = true;

		}

		/// <summary>
		/// Creates a column.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="dataType">The default data type of values found in this column.</param>
		public Column(string columnName, System.Type dataType)
			: base(columnName, dataType)
		{

			// Initialize the object.
			this.IsPersistent = true;

		}

		/// <summary>
		/// Creates a column.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <param name="dataType">The default data type of values found in this column.</param>
		/// <param name="expr">A formula for calculating the value of the column.</param>
		/// <param name="type">The mapping used when the table is converted to XML.</param>
		public Column(string columnName, System.Type dataType, string expr, MappingType type)
			: base(columnName, dataType, expr, type)
		{

			// Initialize the object.
			this.IsPersistent = true;

		}

		/// <summary>
		/// The MarkThree.Data.Table to which this column belongs.
		/// </summary>
		public new Table Table { get { return base.Table as Table; } }
        
	}
    
}
