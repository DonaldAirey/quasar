namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public class ReadRequest : LockRequest
	{

		public ReadRequest(TableSchema tableSchema) : base(tableSchema) { }

	}

}
