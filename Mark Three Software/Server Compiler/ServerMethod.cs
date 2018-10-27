namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.CodeDom;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Common class for creating a CodeDOM method that provides a transacted interface to a data model table.
	/// </summary>
	public class ServerMethod : CodeMemberMethod
	{

		// Private Fields
		private ServerSchema serverSchema;
		private TableSchema tableSchema;

		/// <summary>
		/// Create a CodeDOM description of a method used to provide a transacted interface to a data model table.
		/// </summary>
		/// <param name="tableSchema"></param>
		public ServerMethod(TableSchema tableSchema)
		{

			// Initialize the object.
			this.tableSchema = tableSchema;
			this.serverSchema = tableSchema.DataModelSchema as ServerSchema;
		
		}

		/// <summary>
		/// Gets the schema that describes the entire data model.
		/// </summary>
		public ServerSchema ServerSchema { get { return this.serverSchema; } }

		/// <summary>
		/// Gets the schema that describes the data model table.
		/// </summary>
		public TableSchema TableSchema { get { return this.tableSchema; } }

	}

}
