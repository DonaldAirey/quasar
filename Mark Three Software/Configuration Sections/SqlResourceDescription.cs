namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public class SqlResourceDescription : ResourceDescription
	{

		public readonly string ConnectionString;

		public SqlResourceDescription(string name, string connectionString) : base(name)
		{

			// Initialize the object.
			this.ConnectionString = connectionString;

		}

	}

}
