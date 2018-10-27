namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// A collection of requests to lock a data store resource.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class LockRequestCollection
	{

		// Private Members
		private List<LockRequest> lockRequests;

		/// <summary>
		/// Creates a collection of requests to lock a data store object.
		/// </summary>
		public LockRequestCollection()
		{

			// Initialize the object.
			this.lockRequests = new List<LockRequest>();

		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection of MarkThree.LockRequests.
		/// </summary>
		/// <returns>An enumerator used to iterate through a collection of LockRequests.</returns>
		public IEnumerator<LockRequest> GetEnumerator() { return this.lockRequests.GetEnumerator(); }

		public int Count { get { return this.lockRequests.Count; } }

		/// <summary>
		/// Adds a request to lock a database resource to the ordered collection.
		/// </summary>
		/// <param name="lockRequest">A request to lock a database resource.</param>
		public void Add(LockRequest lockRequest)
		{

			// Insert the item in the list if it doesn't exist already.  Otherwise, there is a precedence to how
			// locks are allocated: a request for a write operation always replaces a request to read.
			int index = this.lockRequests.BinarySearch(lockRequest);
			if (index < 0)
			{
				this.lockRequests.Insert(~index, lockRequest);
			}
			else
			{

				// This will resolve the situation where the same resource has been requested more than once.  A request to write
				// an object will always be honered instead of a request to read.
				LockRequest other = this.lockRequests[index];
				if (lockRequest is RowReaderRequest && other is RowWriterRequest)
					return;

				//  If a request to read is already in he ordered list, it will be replaced with the request to write.
				if (lockRequest is RowWriterRequest && other is RowReaderRequest)
					this.lockRequests[index] = lockRequest;

			}

		}

		/// <summary>
		/// Adds a request to lock a database resource to the ordered collection.
		/// </summary>
		/// <param name="lockRequest">A request to lock a database resource.</param>
		public void AcquireLock(LockRequest lockRequest)
		{

			// Insert the item in the list if it doesn't exist already.  Otherwise, there is a precedence to how
			// locks are allocated: a request for a write operation always replaces a request to read.
			int index = this.lockRequests.BinarySearch(lockRequest);
			if (index < 0)
			{
				this.lockRequests.Insert(~index, lockRequest);
				lockRequest.AcquireLock();
			}
			else
			{

				// This will resolve the situation where the same resource has been requested more than once.  A request to write
				// an object will always be honered instead of a request to read.
				LockRequest other = this.lockRequests[index];
				if (lockRequest is RowReaderRequest && other is RowWriterRequest)
					return;

				//  If a request to read is already in he ordered list, it will be replaced with the request to write.
				if (lockRequest is RowWriterRequest && other is RowReaderRequest)
					throw new Exception("We need a method to promote a lock here.");

			}

		}

	}

}
