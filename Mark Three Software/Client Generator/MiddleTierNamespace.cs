namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.CodeDom;
	using System.Xml.Schema;

	/// <summary>
	/// The root namespace MarkThree.MiddleTier the DataSet output file.
	/// </summary>
	public class MiddleTierNamespace : CodeNamespace
	{

		// The schema used to generate this namespace.
		private DataModelSchema schema;

		/// <summary>
		/// Creates a CodeDOM namespace that contains the strongly typed DataSet.
		/// </summary>
		/// <param name="schema">The schema description of the strongly typed DataSet.</param>
		public MiddleTierNamespace(DataModelSchema schema)
		{

			// Initialize the object.
			this.schema = schema;

			//namespace MarkThree.UnitTest
			//{
			//	using MarkThree;
			//	using System;
			//	using System.ComponentModel;
			//	using System.Data;
			this.Name = this.Schema.TargetNamespace;
			this.Imports.Add(new CodeNamespaceImport("MarkThree"));
			this.Imports.Add(new CodeNamespaceImport("System"));
			this.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
			this.Imports.Add(new CodeNamespaceImport("System.Data"));

			// This is where all the hard work is done.
			this.Types.Add(new TypedDataSetClass(this));

		}

		/// <summary>
		/// Gets the schema that describes the data model.
		/// </summary>
		public DataModelSchema Schema { get { return this.schema; } }

	}

}
