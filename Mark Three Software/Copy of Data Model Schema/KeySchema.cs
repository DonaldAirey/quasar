namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class KeySchema : ConstraintSchema
	{

		// Static Fields
		public static XmlQualifiedName primaryKeyAttributeName;

		// Private Members
		private XmlSchemaKey xmlSchemaKey;

		static KeySchema()
		{

			// Initialize the static fields.
			KeySchema.primaryKeyAttributeName = new XmlQualifiedName("PrimaryKey", "urn:schemas-microsoft-com:xml-msdata");

		}

		public KeySchema(DataModelSchema schema, XmlSchemaKey xmlSchemaKey)
			: base(schema, xmlSchemaKey)
		{

			// Initialize the object
			this.xmlSchemaKey = xmlSchemaKey;

		}

	}

}
