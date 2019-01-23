namespace MarkThree.Guardian
{

	using MarkThree;
	using System;
	using System.ComponentModel;
	using System.Collections;
	using System.Data;
	using System.Threading;

	/// <summary>
	/// Used to access shared data that is refreshed automatically from the server.
	/// </summary>
	[System.ComponentModel.DesignerCategory("Component")]
	public class MarketData : DataSetMarket
	{

		// Private Constant Members
		private const System.Int32 tableTableNameIndex = 0;
		private const System.Int32 tableRowsIndex = 1;
		private const System.Int32 rowStateIndex = 0;
		private const System.Int32 rowDataIndex = 1;

		// Private Static Members
		private static System.Int32 instanceCount;

		/// <summary>
		/// Initializes the static members of the MarketData class.
		/// </summary>
		static MarketData()
		{

			// This is used to keep track of instances of the class.  When the instance count reaches zero, it's time to release
			// all the external resources associated with this component.
			MarketData.instanceCount = 0;

		}

		/// <summary>
		/// Initializer for instances of MarketData managed by the designer
		/// </summary>
		public MarketData()
		{

			// This is used to keep track of the number of references to this shared component.  When the count drops to zero, the
			// external resources (namely the thread) are destroyed.
			MarketData.instanceCount++;

		}

		/// <summary>
		/// Initializer for instances of MarketData managed by the designer
		/// </summary>
		public MarketData(System.ComponentModel.IContainer iContainer)
		{

			// Add this component to the list of components managed by it's client.
			iContainer.Add(this);

			// This is used to keep track of the number of references to this shared component.  When the count drops to zero, the
			// external resources (namely the thread) are destroyed.
			MarketData.instanceCount++;

		}

		/// <summary>
		/// Explicitly destroy the resources for this object.
		/// </summary>
		/// <param name="disposing">true indicates that the managed resources are to be destroyed.</param>
		protected override void Dispose(bool disposing)
		{

			// This provides a counter that inheriting classes can used to determine when static resources should be destroyed.  
			// Note that because of the way constructors are called, it will be preincremented when it is used by the inheriting
			// class, and it will be predecremented by the time the inheriting class gets the 'Dispose' invokation.
			if (disposing)
				--MarketData.instanceCount;

			// Allow the base class to complete the destruction of this object.
			base.Dispose(disposing);

		}

		/// <summary>
		/// The number of instances that have references the data model.
		/// </summary>
		protected static int InstanceCount { get { return MarketData.instanceCount; } }

		/// <summary>
		/// Creates a primary key from a data row suitable for searching a table.
		/// </summary>
		/// <param name="dataTable">The table containing the primary key</param>
		/// <param name="dataRow">A row of data containing key elements</param>
		/// <returns>An array of object suitable for searching for an element based on it's primary key.</returns>
		public static object[] CreateKey(DataRow dataRow)
		{

			// Run through the primary key of the given table and construct an array of object from the source row based on the
			// columns used in the primary key.
			int index = 0;
			object[] key = new object[dataRow.Table.PrimaryKey.Length];
			foreach (DataColumn dataColumn in dataRow.Table.PrimaryKey)
				key[index++] = dataRow[dataColumn.ColumnName, dataRow.RowState == DataRowState.Deleted ? DataRowVersion.Original :
					DataRowVersion.Current];

			// This object can be used to find an element in a data table.
			return key;

		}

		/// <summary>
		/// Checks to see if all the locks in the data model have been released.
		/// </summary>
		/// <returns>true if the current thread has no locks on any table in the MarketData.</returns>
		public static bool IsLocked
		{

			get
			{

				// If this variable is 'true' at the end of the loop below, then all the table locks have been released.
				bool isLocked = false;

				// The 'Debug.Assert' is somewhat lame because it only displays where the error occurred and allows you to stop the
				// execution or continue.  The 'Exception' processing is more powerful because we can break immediately and trace
				// the stack to see where the table failed to aquire a lock.  If we find that a table is accessed without a lock,
				// then throw the exception and give the developer detailed information about what table was accessed.
				foreach (TableLock tableLock in MarketData.TableLocks)
					if (tableLock.IsReaderLockHeld || tableLock.IsWriterLockHeld)
					{
						isLocked = true;
						throw new Exception(String.Format("Table '{0}' is still locked.", tableLock.Name));
					}

				// This will be true if all locks have been released.
				return isLocked;

			}

		}

		/// <summary>
		/// Combine data from an outside source into the data model.
		/// </summary>
		/// <param name="reconciledData">A free-form structure containing new data for this data model.</param>
		public static long Merge(ArrayList reconciledData)
		{

			// The 'RowVersion' is used to reconcile the client data model with the servers.  This value will keep track of the
			// largest 'RowVersion' returned in this set of data which will be used on the next call to the 'Reconcile' method on
			// the server.  This is how the client data model is kept in sync with the server data model.
			long maxRowVersion = long.MinValue;

			// The 'allowViolation' flag is used when the merge method determines that a record has violated the referential 
			// integrity constraints.  Normally, records are 'prequalified' before they are added to the data model.  If they can't
			// be pre-qualified, they are left in the incoming buffer until the parents have been added.  When it is determined
			// that the parent of a record doesn't exist in the batch or in the client data model, the record is added to the
			// database and allowed to fail the constraint rules as a way to exit the loop.
			bool allowViolation = false;

			// This loop will continue until all the records in all the tables returned from the server have been added to the data
			// model.
			while (reconciledData.Count != 0)
			{

				// This flag is used to test whether anything has been added during this iteration of the merge.  When all the 
				// results in the incoming buffer have been examined, and nothing could be added, then the buffer contains elements
				// that violate referential integrity.  That is, there are children in the incoming buffer that do not have parent
				// elements in the client's data model.  This is taken as a signal to allow the records into the data model without
				// the 'prequalification' process that normally orders parents ahead of the children.  When this happens, the
				// incoming records will fail, generate an exception and be removed from the incoming buffer.  This will empty out
				// the buffer and complete the processing of the incoming data.
				bool isAnythingMerged = false;

				// This loop will read each of the table records from the incoming batch.  The records in the batch are arranged 
				// as a generic structure of lists and arrays.  This makes the code more difficult to read, but measurably reduces
				// the amount of information that needs to be serialized between the client and the server because the type
				// information isn't transmitted (as it would be with a user defined structure).
				int tableIndex = 0;
				while (tableIndex < reconciledData.Count)
				{

					// Extract the table record from the reconciled data.  Note that the array indexing is used to access the 
					// records rather than an enumerator.  When all the data from a given table has been handled, the table record
					// is deleted from the reconciled data.  This is illegal with an enumerator, so the array indexing is used.
					object[] tableData = (object[])reconciledData[tableIndex];

					// Set up the variables needed to read the rows from the record.  The table name is used to find the equivalent
					// table in the client data model.  The 'sourceRows' is a list of all the row records from the server that are
					// in the server version of the table and must be merged with the client table.  The 'key' is a reusable 
					// buffer where the primary key elements are assembled when it is time to find the record in the client data
					// model.  And finally the 'rowVersionColumn' is where the incoming records rowVersion can be found.
					Table destinationTable = MarketData.Tables[(string)tableData[tableTableNameIndex]];
					ArrayList sourceRows = (ArrayList)tableData[tableRowsIndex];
					object[] key = new object[destinationTable.PrimaryKey.Length];
					int rowVersionColumn = destinationTable.RowVersionColumn.Ordinal;

					// This will iterate through all the records in the incoming table and add them to the client's table.  The 
					// records will be 'prequalified' by trying to find all the parent records before the row is added.  If a
					// parent record doesn't exist currently in the client data model, the record is deferred and the rest of the
					// batch is handled in the hopes that the parent record is part of the incoming reconciled data.  Several
					// passes of merging are made this way until all the records have been added, or it is determined that a
					// violation of referential integrity has occurred.
					int sourceRowIndex = 0;
					while (sourceRowIndex < sourceRows.Count)
					{

						// The generic data structure passed in from the server is difficult to read, but more efficiently 
						// transmitted that a user defined structure.  This means just a little more work on the client side to
						// follow how the data is extracted.  The rows are transmitted with some 'housekeeping' data.  The first
						// element of the 'sourceRow' is an instruction to add, update or remove the record.  The second element of
						// the 'sourceRow' is an array of the actual data that needs to be added to the able.  The 'sourceRow' is 
						// the collection of the housekeeping elements, while the 'sourceData' is the actual row that needs to be
						// added to the client data model.
						object[] sourceRow = (object[])sourceRows[sourceRowIndex];
						object[] sourceData = (object[])sourceRow[rowDataIndex];
						DataRowState dataRowState = (DataRowState)sourceRow[rowStateIndex];

						// This loop will determine if the record is consistent with the client data model.  That is, it will
						// prequalify the record to be added by checking that all relations.  When a record is added or modified,
						// all the parent relations need to exist.  When a record is deleted, all the child relations must have
						// been deleted first.
						bool isConsistent = true;
						if (dataRowState == DataRowState.Deleted)
						{

							// A record can be deleted only when all the child relations have been removed.
							foreach (DataRelation childRelation in destinationTable.ChildRelations)
							{

								// This will construct the key to the child table.  It is possible for a null reference to indicate 
								// that there is no child required for this record, so the key constructor will check for that
								// possiblity.
								bool isNullKey = true;
								object[] childKey = new object[childRelation.ParentColumns.Length];
								for (int index = 0; index < childKey.Length; index++)
								{
									childKey[index] = sourceData[childRelation.ParentColumns[index].Ordinal];
									if (childKey[index] != DBNull.Value)
										isNullKey = false;
								}

								// If a child record exists, then this record isn't ready to be removed from the data model.  It
								// will be deferred while the other records are processed.  If the child is subsequently deleted, 
								// this record will pass the above test on another iteration through the reconciled data.
								if (!isNullKey && childRelation.ChildTable.Rows.Find(childKey) != null)
								{
									isConsistent = false;
									break;
								}

							}

						}
						else
						{

							// A record can be added or modified only when all the parent relations have been added.
							foreach (DataRelation parentRelation in destinationTable.ParentRelations)
							{

								// This will construct the key to the parent table.  It is possible for a null reference to 
								// indicate that there is no parent required for this record, so the key constructor will check for
								// that possiblity.
								bool isNullKey = true;
								object[] parentKey = new object[parentRelation.ParentColumns.Length];
								for (int index = 0; index < parentKey.Length; index++)
								{
									parentKey[index] = sourceData[parentRelation.ChildColumns[index].Ordinal];
									if (parentKey[index] != DBNull.Value)
										isNullKey = false;
								}

								// If a key to the parent table is present, but the record doesn't exist in the parent table, then
								// this record isn't ready to be added to the data model.  It will be deferred while the other
								// records are read.  If the parent is subsequently read, this record will pass the above test on
								// another iteration through the reconciled data.
								if (!isNullKey && parentRelation.ParentTable.Rows.Find(parentKey) == null)
								{
									isConsistent = false;
									break;
								}

							}

						}

						// If this record would cause a constraint exception, it is skipped on this pass through the data.  If the
						// entire set of reconciled data has been read and this record still can't pass the constraint test, then
						// it is allowed into the data model and allowed to fail as a way to clear out the buffer and exit the 
						// loop.
						if (!isConsistent && !allowViolation)
						{
							sourceRowIndex++;
							continue;
						}

						// This will keep track of the most recent "RowVersion' from the server data model found in this batch of 
						// data.
						long rowVersion = (long)sourceData[rowVersionColumn];
						maxRowVersion = rowVersion > maxRowVersion ? rowVersion : maxRowVersion;

						// Construct a key and attempt to find the record in the current table.  This indicates whether the record 
						// should be added or updated.  In the case of a delete, it is the record that should be removed from the
						// table.
						for (int keyIndex = 0; keyIndex < destinationTable.PrimaryKey.Length; keyIndex++)
							key[keyIndex] = sourceData[destinationTable.PrimaryKey[keyIndex].Ordinal];
						DataRow destinationRow = destinationTable.Rows.Find(key);

						try
						{
				
							// In the "sourceRow" record is information about what action to take on this record.
							if ((DataRowState)sourceRow[rowStateIndex] == DataRowState.Deleted)
							{

								// Delete the record from the client's data model.
								if (destinationRow != null)
									destinationRow.Delete();

							}
							else
							{

								// Add the source record if it doesn't already exist in the data model, modify it if it does.
								if (destinationRow == null)
								{

									// This will add the values in the source row to the current table.
									destinationRow = destinationTable.NewRow();
									foreach (DataColumn destinationColumn in destinationTable.Columns)
										destinationRow[destinationColumn.Ordinal] = sourceData[destinationColumn.Ordinal];
									destinationTable.Rows.Add(destinationRow);

								}
								else
								{

									// This will modify the values in the destination record to have the same value as the source.
									// Note that the 'BeginEdit' and 'EndEdit' will consolidate all the 'Changed' events into a
									// single event when the 'EndEdit' is executed.
									destinationRow.BeginEdit();
									foreach (DataColumn destinationColumn in destinationTable.Columns)
										destinationRow[destinationColumn.Ordinal] = sourceData[destinationColumn.Ordinal];
									destinationRow.EndEdit();

								}

							}

							// At this point, the record can be accepted into the community of relational integrity abiding members
							// of our data model.
							if (destinationRow != null)
								destinationRow.AcceptChanges();

							// Remove each of the rows from the incoming list as they are added to the data model.  When the buffer
							// is empty, the 'merging' is done.
							sourceRows.Remove(sourceRow);
							isAnythingMerged = true;

						}
						catch (Exception exception)
						{

							// Record any errors trying to read the data.
							EventLog.Error("{0}: {1}", exception.Message, exception.StackTrace);

							// Remove the offensive record from the incoming buffer.
							sourceRows.Remove(sourceRow);

							// Roll back the change to this row.
							if (destinationRow.RowState != DataRowState.Detached)
								destinationRow.RejectChanges();

						}

					}

					// When the incoming table record has been emptied of all its records, it is removed from the list of tables.
					// When the list is empty, the merge is complete.
					if (sourceRows.Count == 0)
						reconciledData.Remove(tableData);
					else
						tableIndex++;

				}

				// This condition indicates that there are rows in the data set that can't be merged.  When this happens, the
				// prequalification of the rows will be skipped and a contraint violation will be allowed to end the merge.
				if (!isAnythingMerged && reconciledData.Count != 0)
					allowViolation = true;

			}

			// This value is used on the next reconcilliation to mark the most recent record on the client.
			return maxRowVersion;

		}

	}

}
