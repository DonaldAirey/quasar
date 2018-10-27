namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class DataSetSchema : ObjectSchema
	{

		// Public Properties
		public readonly DataModelSchema Schema;

		// Private Members
		private XmlSchemaElement xmlSchemaElement;

		public DataSetSchema(DataModelSchema schema, XmlSchemaElement xmlSchemaElement) : base(xmlSchemaElement)
		{

			// Initialize the object.
			this.Schema = schema;
			this.xmlSchemaElement = xmlSchemaElement;

		}

		public override string ToString() {return this.Name;}
		
		public string Name { get { return this.xmlSchemaElement.Name; } }

		public TableSchemaCollection Tables
		{

			get
			{

				TableSchemaCollection tables = new TableSchemaCollection();

				if (this.xmlSchemaElement.SchemaTypeName == XmlQualifiedName.Empty)
				{
					if (this.xmlSchemaElement.SchemaType is XmlSchemaComplexType)
					{
						XmlSchemaComplexType xmlSchemaComplexType = this.xmlSchemaElement.SchemaType as XmlSchemaComplexType;
						if (xmlSchemaComplexType.Particle is XmlSchemaChoice)
						{
							XmlSchemaChoice xmlSchemaChoice = xmlSchemaComplexType.Particle as XmlSchemaChoice;
							foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaChoice.Items)
							{
								if (xmlSchemaObject is XmlSchemaElement)
								{
									XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
									tables.Add(new TableSchema(this.Schema, xmlSchemaElement));
								}
							}
						}
					}
				}
				
				return tables;

			}

		}

		public ConstraintSchemaCollection Constraints
		{

			get
			{

				ConstraintSchemaCollection constraints = new ConstraintSchemaCollection();

				foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in this.xmlSchemaElement.Constraints)
				{
					if (xmlSchemaIdentityConstraint is XmlSchemaKey)
					{
						XmlSchemaKey xmlSchemaKey = xmlSchemaIdentityConstraint as XmlSchemaKey;
						constraints.Add(new KeySchema(this.Schema, xmlSchemaKey));
					}

					if (xmlSchemaIdentityConstraint is XmlSchemaUnique)
					{
						XmlSchemaUnique xmlSchemaUnique = xmlSchemaIdentityConstraint as XmlSchemaUnique;
						constraints.Add(new UniqueSchema(this.Schema, xmlSchemaUnique));
					}

					if (xmlSchemaIdentityConstraint is XmlSchemaKeyref)
					{
						XmlSchemaKeyref xmlSchemaKeyref = xmlSchemaIdentityConstraint as XmlSchemaKeyref;
						constraints.Add(new KeyrefSchema(this.Schema, xmlSchemaKeyref));
					}

				}

				return constraints;

			}

		}

		public object GetUnhandledAttribute(string attributeName)
		{

			// Cycle through each of the unhandled attributes (if any exist) looking for the 'AutoIncrement' attribute.
			if (this.xmlSchemaElement.UnhandledAttributes != null)
				foreach (XmlAttribute xmlAttribute in xmlSchemaElement.UnhandledAttributes)
					if (xmlAttribute.LocalName == attributeName)
						return xmlAttribute.Value;

			// At this point, the column element doesn't contain an attribute with the given name.
			return null;

		}

	}

}
