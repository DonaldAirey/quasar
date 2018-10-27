namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Transactions;
	using System.Threading;

	/// <summary>
	/// Manages the resources needed to read and modify an ADO data model.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class AdoResourceManager : VolatileResourceManager
	{

		// Internal Static Members
		internal static Dictionary<string, DataSet> dataSetTable;
		internal static Dictionary<DataSet, string> nameTable;

		// Private Members
		private System.Collections.Generic.List<ReaderWriterLock> locks;
		private System.Collections.Generic.List<Row> rows;

		/// <summary>
		/// Initializes the static properties of the resource manager for ADO data sets.
		/// </summary>
		static AdoResourceManager()
		{

			// These hash tables are used to map a resource name to a data set and a data set to a resource manager name.  For the 
			// most part, the resource managers are referenced by their names, but sometimes the name isn't available.  In those
			// situations, the dataset can be used to find the resource manager that handles that data set's actions.
			AdoResourceManager.dataSetTable = new Dictionary<string, DataSet>();
			AdoResourceManager.nameTable = new Dictionary<DataSet, string>();

		}

		/// <summary>
		/// Creates a manager of an ADO DataSet.
		/// </summary>
		/// <param name="typeName">The friendly name of an ADO dataset.</param>
		public AdoResourceManager(string typeName) : base(typeName)
		{

			// Initialize the object.
			this.rows = new List<Row>();
			this.locks = new List<ReaderWriterLock>();

		}

		/// <summary>
		/// Adds a mapping between a data set and a resource name.
		/// </summary>
		/// <param name="name">The name of the resource manager.</param>
		/// <param name="dataSet">The DataSet managed by the resource manager with the given name.</param>
		public static void AddDataSet(string name, DataSet dataSet)
		{

			// This will create a mapping between the DataSet and the name of its resource manager.
			AdoResourceManager.dataSetTable.Add(name, dataSet);
			AdoResourceManager.nameTable.Add(dataSet, name);

		}

		/// <summary>
		/// Adds a row to the transaction.
		/// </summary>
		/// <param name="row">This row will be accepted or rolled back with the other rows in the transaction.</param>
		public void Add(Row row)
		{

			// This is a list of rows that will be committed or rejected as a unit when the transaction completes.
			this.rows.Add(row);

		}

		public void Add(ReaderWriterLock readerWriterLock)
		{

			// This is a list of reader/writer locks that are managed by this resource manager.
			this.locks.Add(readerWriterLock);

		}

		/// <summary>
		/// Commit all the rows in this transaction.
		/// </summary>
		/// <param name="enlistment">Facilitates communication between an enlisted transaction and the transaction manager during the
		/// final phase of the transaction.</param>
		public override void Commit(Enlistment enlistment)
		{

			// The tables that own each of the rows in the transaction need to be locked before the record can be accepted or
			// rejected.  There are apparently some housekeeping functions that are not thread-safe.  This will create a distinct
			// list of tables that need to be locked in order to accept all the rows in this transaction.  Note that the indices
			// are not included when searching for resources to be locked.  This is because the indices are assumed to be locked if
			// the data table associated with that index is locked.
			SortedList<string, Table> lockList = new SortedList<string, Table>();
			foreach (Row row in this.rows)
				if (!lockList.ContainsKey(row.Table.TableName))
					lockList.Add(row.Table.TableName, row.Table);

			try
			{

				// Lock each of the distinct tables that have been modified by this transaction.
				foreach (KeyValuePair<string, Table> keyValuePair in lockList)
					keyValuePair.Value.ReaderWriterLock.AcquireWriterLock(Timeout.Infinite);

				// This will commit every one of the rows in the transaction that hasn't already been committed.
				foreach (Row row in this.rows)
					if (row.RowState != DataRowState.Unchanged)
						row.AcceptChanges();

			}
			finally
			{

				// The tables involved in this transaction are now available for other threads.
				foreach (KeyValuePair<string, Table> keyValuePair in lockList)
					keyValuePair.Value.ReaderWriterLock.ReleaseWriterLock();

			}

			// Indicates that the transaction participant has committed the data to its store.
			enlistment.Done();

		}

		/// <summary>
		/// Rollback (undo) any of the changes made during the transaction.
		/// </summary>
		/// <param name="enlistment">Facilitates communication between an enlisted transaction and the transaction manager during the
		/// final phase of the transaction.</param>
		public override void Rollback(Enlistment enlistment)
		{

			// In order to preserve relational integrity, the rows must be rejected in the opposite order they were modified.  For
			// example, if a child is deleted before the parent, the parent must be restored before the child.
			this.rows.Reverse();

			// The tables that own each of the rows in the transaction need to be locked before the record can be accepted or
			// rejected.  There are apparently some housekeeping functions that are not thread-safe.  This will create a distinct
			// list of tables that need to be locked in order to accept all the rows in this transaction.  Note that the indices
			// are not included when searching for resources to be locked.  This is because the indices are assumed to be locked if
			// the data table associated with that index is locked.
			SortedList<string, Table> lockList = new SortedList<string, Table>();
			foreach (Row row in this.rows)
				if (!lockList.ContainsKey(row.Table.TableName))
					lockList.Add(row.Table.TableName, row.Table);

			try
			{

				// Lock each of the distinct tables that have been modified by this transaction.
				foreach (KeyValuePair<string, Table> keyValuePair in lockList)
					keyValuePair.Value.ReaderWriterLock.AcquireWriterLock(Timeout.Infinite);

				// This will commit every one of the rows in the transaction that hasn't already been committed.
				foreach (Row row in this.rows)
					if (row.RowState != DataRowState.Unchanged)
						row.RejectChanges();

			}
			finally
			{

				// The tables involved in this transaction are now available for other threads.
				foreach (KeyValuePair<string, Table> keyValuePair in lockList)
					keyValuePair.Value.ReaderWriterLock.ReleaseWriterLock();

			}

			// Indicates that the transaction participant has committed the data to its store.
			enlistment.Done();

		}

		/// <summary>
		/// Disposes of the resources allocated by this resource manager.
		/// </summary>
		public override void Dispose()
		{

			// Release all the locks owned when the resource manager is destroyed.
			foreach (ReaderWriterLock readerWriterLock in this.locks)
			{
				if (readerWriterLock.IsReaderLockHeld)
					readerWriterLock.ReleaseReaderLock();
				if (readerWriterLock.IsWriterLockHeld)
					readerWriterLock.ReleaseWriterLock();
			}

		}

	}

}
