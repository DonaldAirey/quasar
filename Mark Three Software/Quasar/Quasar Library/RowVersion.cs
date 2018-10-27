/*************************************************************************************************************************
*
*	File:			Global.asmx
*	Description:	Global Data used by the Web Services.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;
using System.Threading;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Optimistic Concurrency check for tables and datasets.  Can also be used for a relative 'age' of a record.
	/// </summary>
	public class RowVersion
	{

		public static long NotUsed = -1L;
		private long rowVersion = 1;
		private static ReaderWriterLock lockRowVersion =  new ReaderWriterLock();

		/// <summary>
		/// Thread-safe Increment of the RowVersion.
		/// </summary>
		/// <returns>The incremented row version of the Data Model.</returns>
		public long Increment()
		{

			try
			{
				lockRowVersion.AcquireWriterLock(CommonTimeout.LockWait);
				long temp = ++this.rowVersion;
				return temp;
			}
			finally
			{
				lockRowVersion.ReleaseWriterLock();
			}

		}

		/// <summary>
		/// Thread-safe read of the current Data Model's RowVersion.
		/// </summary>
		/// <returns>The current row version of the Data Model.</returns>
		public long Get()
		{

			try
			{
				lockRowVersion.AcquireReaderLock(CommonTimeout.LockWait);
				long temp = this.rowVersion;
				return temp;
			}
			finally
			{
				lockRowVersion.ReleaseReaderLock();
			}

		}

		/// <summary>
		/// Thread-safe read of the current Data Model's RowVersion.
		/// </summary>
		/// <returns>The current row version of the Data Model.</returns>
		public void Set(long rowVersion)
		{

			try
			{
				lockRowVersion.AcquireWriterLock(CommonTimeout.LockWait);
				this.rowVersion = rowVersion;
			}
			finally
			{
				lockRowVersion.ReleaseWriterLock();
			}

		}

	}

}
