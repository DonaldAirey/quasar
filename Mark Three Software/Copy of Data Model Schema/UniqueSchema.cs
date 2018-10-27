namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml.Schema;

	public class UniqueSchema : ConstraintSchema
	{

		// Private Members
		private XmlSchemaUnique xmlSchemaUnique;

		public UniqueSchema(DataModelSchema schema, XmlSchemaUnique xmlSchemaUnique)
			: base(schema, xmlSchemaUnique)
		{

			// Initialize the object
			this.xmlSchemaUnique = xmlSchemaUnique;

		}

	}

}
