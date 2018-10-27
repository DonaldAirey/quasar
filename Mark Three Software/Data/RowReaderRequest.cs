namespace MarkThree
{

	using System;
	using System.Data;
	using System.Threading;

	/// <summary>
	/// A row lock for reading that is managed as part of a transaction.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class RowReaderRequest : RowRequest
	{

		/// <summary>
		/// Create a request to lock a row for reading.
		/// </summary>
		/// <param name="row">The row that is to be locked for reading.</param>
		public RowReaderRequest(Row row) : base(row) { }

		/// <summary>
		/// Aquire the managed lock to read a row.
		/// </summary>
		public override void AcquireLock()
		{

#if LOCKTRACE
			// This will emit debugging information that is useful to track the acquiring and releasing of the locks.
			if (this.row.RowState == DataRowState.Detached)
				Console.WriteLine("{0}: Acquiring Row Reader Lock on new row {1}:{2}", Thread.CurrentThread.Name == null ?
					Convert.ToString(Thread.CurrentThread.ManagedThreadId) : Thread.CurrentThread.Name, row.GetType(), row.RowId);
			else
				Console.WriteLine("{0}: Acquiring Row Reader Lock on {1}:{2}", Thread.CurrentThread.Name == null ?
					Convert.ToString(Thread.CurrentThread.ManagedThreadId) : Thread.CurrentThread.Name, row.Table.TableName,
					row.RowId);
#endif

			// Acquire the managed lock.
			this.Row.ReaderWriterLock.AcquireReaderLock(Timeout.Infinite);

		}

		/// <summary>
		/// Release a managed lock for reading.
		/// </summary>
		public override void ReleaseLock()
		{

#if LOCKTRACE
			// This will emit debugging information that is useful to track the acquiring and releasing of the locks.
			if (this.row.RowState == DataRowState.Detached || this.row.RowState == DataRowState.Deleted)
				Console.WriteLine("{0}: Releasing Row Reader Lock on {1}:{2}", Thread.CurrentThread.Name == null ?
					Convert.ToString(Thread.CurrentThread.ManagedThreadId) : Thread.CurrentThread.Name, row.Table.TableName,
					row.RowState);
			else
				Console.WriteLine("{0}: Releasing Row Reader Lock on {1}:{2}", Thread.CurrentThread.Name == null ?
					Convert.ToString(Thread.CurrentThread.ManagedThreadId) : Thread.CurrentThread.Name, row.Table.TableName,
					row.RowId);
#endif

			// Release the managed lock.
			this.Row.ReaderWriterLock.ReleaseReaderLock();

		}

	}

}
