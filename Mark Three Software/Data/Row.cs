namespace MarkThree
{

	using System;
	using System.Data;
	using System.Threading;

	/// <summary>
	/// Represents a row of data in a MarkThree.Table
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Row : DataRow
	{

		// Public Readonly Properties
		public readonly System.Threading.ReaderWriterLock ReaderWriterLock;

		// Private Members
		private System.Int64 rowVersion;

		/// <summary>
		/// Creates a row.
		/// </summary>
		/// <param name="dataRowBuilder">The System.Data.DataRowBuilder type supports the .NET Framework and is not intended to be
		/// used directly by your code.</param>
		public Row(System.Data.DataRowBuilder dataRowBuilder)
			: base(dataRowBuilder)
		{

			// Initialize the object.
			this.ReaderWriterLock = new ReaderWriterLock();

		}

		/// <summary>
		/// Gets or sets a column of data in the row.
		/// </summary>
		/// <param name="index">The index of the column in the row.</param>
		/// <returns>The object found at the given index in the row.</returns>
		public new object this[int index]
		{

			get
			{
				// Reject the operation if the proper locks aren't in place.
				if (!this.ReaderWriterLock.IsReaderLockHeld && !this.ReaderWriterLock.IsWriterLockHeld)
					throw new LockException("Attempt was made to access a row in {0} without a lock.", this.Table);
				// The base class is used to return the value in the column once the locks are in place.
				return base[index];
			}

			set
			{
				// Reject the operation if the proper locks aren't in place.
				if (!this.Table.ReaderWriterLock.IsWriterLockHeld || !this.ReaderWriterLock.IsWriterLockHeld)
					throw new LockException("Attempt was made to access a row in {0} without a lock.", this.Table);
				// The base class is used to set the value in the column once the locks are in place.
				base[index] = value;
			}

		}

		/// <summary>
		/// Gets or sets a column of data in the row.
		/// </summary>
		/// <param name="columnName">The name of the column.</param>
		/// <returns>The object found at the given index in the row.</returns>
		public new object this[string columnName]
		{

			get
			{
				// Reject the operation if the proper locks aren't in place.
				if ((!this.Table.ReaderWriterLock.IsReaderLockHeld && !this.Table.ReaderWriterLock.IsWriterLockHeld) ||
					(!this.ReaderWriterLock.IsReaderLockHeld && !this.ReaderWriterLock.IsWriterLockHeld))
					throw new LockException("Attempt was made to access a row in {0} without a lock.", this.Table);
				// The base class is used to return the value in the column once the locks are in place.
				return base[columnName];
			}

			set
			{
				// Reject the operation if the proper locks aren't in place.
				if (!this.Table.ReaderWriterLock.IsWriterLockHeld || !this.ReaderWriterLock.IsWriterLockHeld)
					throw new LockException("Attempt was made to access a row in {0} without a lock.", this.Table);
				// The base class is used to set the value in the column once the locks are in place.
				base[columnName] = value;
			}

		}

		/// <summary>
		/// Gets or sets a column of data in the row.
		/// </summary>
		/// <param name="column">The column that is to be accessed.</param>
		/// <returns>The object found at the given index in the row.</returns>
		public object this[Column column]
		{

			get
			{
				// Reject the operation if the proper locks aren't in place.
				if (!this.ReaderWriterLock.IsReaderLockHeld && !this.ReaderWriterLock.IsWriterLockHeld)
					throw new LockException("Attempt was made to access a row in {0} without a lock.", this.Table);
				// The base class is used to return the value in the column once the locks are in place.
				return base[column];
			}

			set
			{
				// Reject the operation if the proper locks aren't in place.
				if (this.RowState != DataRowState.Detached && (!this.Table.ReaderWriterLock.IsWriterLockHeld || !this.ReaderWriterLock.IsWriterLockHeld))
					throw new Exception("This row isn't locked");
				// The base class is used to set the value in the column once the locks are in place.
				base[column] = value;
			}

		}

		/// <summary>
		/// Deletes a record from the table.
		/// </summary>
		public new void Delete()
		{

			// Reject the operation if the proper locks aren't in place.
			if (!this.ReaderWriterLock.IsWriterLockHeld)
				throw new Exception("This row isn't locked");

			// The row version is inaccessible after the row has been deleted, so it is moved to a safe place.  When this row is 
			// deleted, the server logic will attempt to make a copy of the primary key fields and move it to a deleted data
			// model.  The row version of the record before it was deleted needs to be available for this record when it is moved.
			this.rowVersion = (long)this[this.Table.RowVersionColumn];
			base.Delete();

		}

		/// <summary>
		/// Commits almost all the changes that were made to this row since the last time MarkThree.Row.AcceptChanges was called.
		/// </summary>
		public new void AcceptChanges()
		{

			// Reject the operation if the proper locks aren't in place for this operation.
			if (!this.Table.ReaderWriterLock.IsWriterLockHeld || !this.ReaderWriterLock.IsWriterLockHeld)
				throw new LockException("Attempt was made to access a row in {0} without a lock.", this.Table);

			// IMPORTANT CONCEPT: Deleted rows are not purged from the table at the end of the transaction.  They are kept around
			// long enough to be transmitted back to the client workstations where the client uses the deleted information to
			// remove that record from the client data model.  The 'deletedTime' is used as a time stamp that indicates when the
			// deleted record has become stale and can be purged from the tables.
			base.AcceptChanges();

		}

		/// <summary>
		/// Rejects all the changes that were made to this row since the last time MarkThree.Row.RejectChanges() was called.
		/// </summary>
		public new void RejectChanges()
		{

			// Reject the operation if the proper locks aren't in place for this operation.
			if (!this.Table.ReaderWriterLock.IsWriterLockHeld || !this.ReaderWriterLock.IsWriterLockHeld)
				throw new LockException("Attempt was made to access a row in {0} without a lock.", this.Table);

			// Restoring the default deleted time indicates that this record is no longer marked for deletion.  The rest of the
			// changes are rejected by calling the base class to finish restoring the row.
			base.RejectChanges();

		}

		/// <summary>
		/// Gets the MarkThree.Table for which this row has a schema.
		/// </summary>
		public new Table Table { get { return (this as DataRow).Table as Table; } }

		/// <summary>
		/// An indication of the relative age of a record.
		/// </summary>
		public long RowVersion { get { return this.RowState == DataRowState.Deleted ? this.rowVersion : (long)this[this.Table.RowVersionColumn]; } }

	}

}
