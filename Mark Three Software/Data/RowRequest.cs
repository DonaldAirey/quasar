namespace MarkThree
{

	using MarkThree;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Threading;

	/// <summary>
	/// A request for a lock on a row.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public abstract class RowRequest : LockRequest
	{

		// Public Readonly Properties
		public readonly Row Row;

		/// <summary>
		/// Create a request for a lock on a row.
		/// </summary>
		/// <param name="row">A row that is to be locked.</param>
		public RowRequest(Row row)
		{

			// Initialize the object.
			this.Row = row;

		}

	}

}
