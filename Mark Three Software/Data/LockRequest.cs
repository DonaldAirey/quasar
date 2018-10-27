namespace MarkThree
{

	using MarkThree;
	using System;
	using System.Threading;

	/// <summary>
	/// A request for a lock on an object in a data store.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public abstract class LockRequest : IDisposable, IComparable<LockRequest>
	{

		/// <summary>
		/// Aquires a requested lock on a resource.
		/// </summary>
		public abstract void AcquireLock();

		/// <summary>
		/// Releases a requested lock on a resource.
		/// </summary>
		public abstract void ReleaseLock();

		#region IComparable<LockRequest> Members

		/// <summary>
		/// Compares one lock request to another.
		/// </summary>
		/// <param name="other">The other lock requested.</param>
		/// <returns>-1 if the this lock should be aquired before the 'other', -1 if not, 0 if both requests are for the same
		/// resource.</returns>
		public int CompareTo(LockRequest other)
		{

			// This will organize the disparate types of requests for locks so that they are always acquired and released in the
			// same order.
			if (this is RowRequest)
			{

				RowRequest rowRequest = this as RowRequest;

				// This will compare two row requests.
				if (other is RowRequest)
				{
					RowRequest otherRequest = other as RowRequest;
					int compare = rowRequest.Row.Table.TableName.CompareTo(otherRequest.Row.Table.TableName);
					return compare == 0 ? rowRequest.Row.CompareTo(otherRequest.Row) : compare;
				}

			}

			// All the valid combination of lock requests have been compared by this point.
			throw new Exception(string.Format("There is no comparison on a {0} and a {1}", this.GetType(), other.GetType()));

		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{

			// Release the locks.
			ReleaseLock();

		}

		#endregion

	}

}
