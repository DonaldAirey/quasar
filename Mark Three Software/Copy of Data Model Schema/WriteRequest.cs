namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public class WriteRequest : LockRequest
	{

		public WriteRequest(TableSchema tableSchema) : base(tableSchema) { }

	}

}
