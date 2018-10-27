namespace MarkThree
{

	using System;
	using System.Data;
	using System.Collections;

	/// <summary>
	/// A collection of rows in a Table.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class RowCollection
	{
        
		// Private Members
		private MarkThree.Table table;
		private System.Data.DataRowCollection dataRowCollection;
        
		/// <summary>
		/// Creates a collection of rows.
		/// </summary>
		/// <param name="dataRowCollection"></param>
		public RowCollection(MarkThree.Table table, System.Data.DataRowCollection dataRowCollection)
		{

			// The purpose of this class is simply to provide a strongly typed list of the Row class from the original
			// DataRow version of the list.
			this.table = table;
			this.dataRowCollection = dataRowCollection;

		}

		/// <summary>
		/// Gets a row from the table.
		/// </summary>
		/// <param name="index">The index of the row in the table.</param>
		/// <returns>The row at the given index.</returns>
		public Row this[int index] { get { return ((Row)(this.dataRowCollection[index])); } }

		/// <summary>
		/// The number of rows in the table.
		/// </summary>
		public int Count { get { return this.dataRowCollection.Count; } }

		/// <summary>
		/// Gets a System.Collections.IEnumerator for the collection.
		/// </summary>
		/// <returns>An enumerator that allows iteration through the collection.</returns>
		public IEnumerator GetEnumerator() { return this.dataRowCollection.GetEnumerator(); }
        
		/// <summary>
		/// Adds a row to the collection.
		/// </summary>
		/// <param name="sharedRow">A row to be added to the table.</param>
		public void Add(Row row)
		{

			// Reject the operation if the proper locks aren't in place.
			if (!this.table.ReaderWriterLock.IsWriterLockHeld)
				throw new LockException("Attempt was made to access a row in {0} without a lock.", this.table);

			// Each of the indices needs to be updated with the key elements that point to this record before the record is added
			// to the table.
			this.dataRowCollection.Add(row);

		}

		/// <summary>
		/// Removes a row from the table.
		/// </summary>
		/// <param name="row">The row to be removed from the table.</param>
		public void Remove(Row row)
		{

			// Reject the operation if the proper locks aren't in place.
			if (!this.table.ReaderWriterLock.IsWriterLockHeld)
				throw new LockException("Attempt was made to access a row in {0} without a lock.", this.table);

			// This operation is meant to be done outside of a normal transaction as there's no chance to reject it later.  A row 
			// that is deleted using the 'Delete' method has its own logic for purging the indices within the scope of a
			// transaction, so deleted rows don't purge their indices here.
			this.dataRowCollection.Remove(row);

		}

		/// <summary>
		/// Gets the row that contains the primary key elements.
		/// </summary>
		/// <param name="key">The unique key used to find a row.</param>
		/// <returns>A row containing the primary key elements.</returns>
		public Row Find(object key) { return this.dataRowCollection.Find(key) as Row; }

		public Row Find(object[] keys) { return this.dataRowCollection.Find(keys) as Row; }

	}
    
}
