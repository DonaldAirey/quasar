namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Text;
	using System.Transactions;
	using System.Threading;

	/// <summary>
	/// Manages the resources needed to read and modify an SQL data model.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class SqlResourceManager : DurableResourceManager
	{

		// Public Readonly Properties.
		public readonly SqlConnection SqlConnection;

		// Private Static Members
		private static Dictionary<string, string> connectionTable;

		/// <summary>
		/// Initialize the static resources of an SqlResourceManager.
		/// </summary>
		static SqlResourceManager()
		{

			// This table is used to map an SQL data store name to the connection string used to establish a connection to that
			// store.
			SqlResourceManager.connectionTable = new Dictionary<string, string>();

		}

		/// <summary>
		/// Create a resource manager for an SQL data store.
		/// </summary>
		/// <param name="name">The name of the resource manager.</param>
		public SqlResourceManager(string name)
			: base(name)
		{

			// The name of the resource manager is mapped to a connection string using the static table.  That allows the name to
			// be used as an index to find the connection string, which effectively defines the properties of the SQL data store.
			string connectionString;
			if (!SqlResourceManager.connectionTable.TryGetValue(name, out connectionString))
				throw new Exception(string.Format("There is no connection string defined for {0}", name));

			// Use the connection string to establish a connection to the SQL data store.
			this.SqlConnection = new SqlConnection(connectionString);
			this.SqlConnection.Open();

		}

		/// <summary>
		/// Destroy the resources used by this resource manager.
		/// </summary>
		public override void Dispose()
		{

			// After the base class destroys its resources, close out the connection to the SQL data store.
			base.Dispose();
			this.SqlConnection.Close();

		}

		/// <summary>
		/// Adds a mapping between a resource name and a connection string.
		/// </summary>
		/// <param name="name">The friendly name of the SQL resource.</param>
		/// <param name="connectionString">A string used to establish a connection to the SQL resource.</param>
		public static void AddConnection(string name, string connectionString)
		{

			// The friendly name is used to map the resource manager to an SQL database when an SqlResourceManager is created.
			SqlResourceManager.connectionTable.Add(name, connectionString);

		}

	}

}
