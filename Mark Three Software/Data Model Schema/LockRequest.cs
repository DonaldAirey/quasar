namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public class LockRequest : IComparable<LockRequest>
	{

		private TableSchema tableSchema;

		public LockRequest(TableSchema tableSchema)
		{

			// Initialize the object.
			this.tableSchema = tableSchema;

		}

		public TableSchema TableSchema { get { return this.tableSchema; } }

		#region IComparable<LockRequest> Members

		public int CompareTo(LockRequest other)
		{
			return this.tableSchema.CompareTo(other.tableSchema);
		}

		#endregion

	}

}
