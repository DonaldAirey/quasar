namespace MarkThree
{

	using System;
	using System.Data;
	using System.Threading;

	/// <summary>
	/// A row lock for writing that is managed as part of a transaction.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class RowWriterRequest : RowRequest
	{

		/// <summary>
		/// Create a request to lock a row for writing.
		/// </summary>
		/// <param name="row">The row that is to be locked for writing.</param>
		public RowWriterRequest(Row row) : base(row) { }

		/// <summary>
		/// Release a managed lock for writing.
		/// </summary>
		public override void AcquireLock()
		{

#if LOCKTRACE
			if (this.row.RowState == DataRowState.Detached || this.row.RowState == DataRowState.Deleted)
				Console.WriteLine("{0}: Acquiring Row Writer Lock on new row {1}:{2}", Thread.CurrentThread.Name == null ?
				Convert.ToString(Thread.CurrentThread.ManagedThreadId) : Thread.CurrentThread.Name, row.GetType(), row.RowId);
			else
				Console.WriteLine("{0}: Acquiring Row Writer Lock on {1}:{2}", Thread.CurrentThread.Name == null ?
				Convert.ToString(Thread.CurrentThread.ManagedThreadId) : Thread.CurrentThread.Name, row.Table.TableName, row.RowId);
#endif

			// Acquire the managed lock.
			this.Row.ReaderWriterLock.AcquireWriterLock(Timeout.Infinite);

		}

		/// <summary>
		/// Release a managed lock for writing.
		/// </summary>
		public override void ReleaseLock()
		{

#if LOCKTRACE
			if (this.row.RowState == DataRowState.Detached || this.row.RowState == DataRowState.Deleted)
				Console.WriteLine("{0}: Releasing Row Writer Lock on {1}:{2}", Thread.CurrentThread.Name == null ?
				Convert.ToString(Thread.CurrentThread.ManagedThreadId) : Thread.CurrentThread.Name, row.Table.TableName, row.RowState);
			else
				Console.WriteLine("{0}: Releasing Row Writer Lock on {1}:{2}", Thread.CurrentThread.Name == null ?
				Convert.ToString(Thread.CurrentThread.ManagedThreadId) : Thread.CurrentThread.Name, row.Table.TableName, row.RowId);
#endif

			// Release the managed lock.
			this.Row.ReaderWriterLock.ReleaseWriterLock();

		}

	}

}
