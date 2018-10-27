namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public class ServerSchema : DataModelSchema
	{

		private System.Collections.Generic.List<string> references;
		private System.String name;

		public ServerSchema(string fileContent) : base(fileContent) { }

		public List<String> References { get { return this.references; } set { this.references = value; } }

		public new string Name { get { return this.name; } set { this.name = value; } }

	}

}
