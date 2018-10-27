namespace MarkThree
{

	using System;
	using System.Collections;
	using System.Data;

	/// <summary>
	/// A collection of columns in a Table.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ColumnCollection
	{
        
		// Private Members
		private DataColumnCollection dataColumnCollection;
        
		/// <summary>
		/// Creates a collection of columns.
		/// </summary>
		/// <param name="dataColumnCollection"></param>
		public ColumnCollection(DataColumnCollection dataColumnCollection)
		{

			// The purpose of this class is simply to provide a strongly typed list of the Column class from the original
			// DataColumn version of the list.
			this.dataColumnCollection = dataColumnCollection;

		}

		/// <summary>
		/// Gets a column from the list using the name of the column.
		/// </summary>
		/// <param name="index">The name of the column.</param>
		/// <returns>The column matching the name or null if that column doesn't exist.</returns>
		public Column this[int index] { get { return ((Column)(this.dataColumnCollection[index])); } }

		/// <summary>
		/// Gets a column from the list using the name of the column.
		/// </summary>
		/// <param name="index">The name of the column.</param>
		/// <returns>The column matching the name or null if that column doesn't exist.</returns>
		public Column this[string index] { get { return ((Column)(this.dataColumnCollection[index])); } }

		/// <summary>
		/// The number of columns in the list.
		/// </summary>
		public int Count { get { return this.dataColumnCollection.Count; } }

		public bool Contains(Column column) { return this.dataColumnCollection.Contains(column.ColumnName); }
        
		/// <summary>
		/// Gets a System.Collections.IEnumerator for the collection.
		/// </summary>
		/// <returns>An enumerator that allows iteration through the collection.</returns>
		public IEnumerator GetEnumerator() { return this.dataColumnCollection.GetEnumerator(); }
        
		/// <summary>
		/// Adds a column to the collection.
		/// </summary>
		/// <param name="sharedColumn">A column to be added to the collection.</param>
		public void Add(Column column) { this.dataColumnCollection.Add(column); }

		/// <summary>
		/// Adds a column to the collection.
		/// </summary>
		/// <param name="sharedColumn">A column to be added to the collection.</param>
		public void Add(DataColumn dataColumn) { this.dataColumnCollection.Add(dataColumn); }

	}
    
}
