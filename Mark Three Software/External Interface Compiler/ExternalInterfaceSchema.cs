namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;

	public class ExternalInterfaceSchema : MiddleTierSchema
	{

		private string internalNamespace;
		private ArrayList references;

		public ExternalInterfaceSchema(string fileContent) : base(fileContent) { }

		public ArrayList References { get { return this.references; } set { this.references = value; } }

		public string InternalNamespace { get { return this.internalNamespace; } set { this.internalNamespace = value; } }

	}

}
