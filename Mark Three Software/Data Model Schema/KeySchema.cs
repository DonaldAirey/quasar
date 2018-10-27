namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class KeySchema : ConstraintSchema
	{

		// Private Members
		private XmlSchemaKey xmlSchemaKey;

		public KeySchema(DataModelSchema schema, XmlSchemaKey xmlSchemaKey)
			: base(schema, xmlSchemaKey)
		{

			// Initialize the object
			this.xmlSchemaKey = xmlSchemaKey;

		}

	}

}
