namespace MarkThree.MiddleTier
{

	using System;
	using System.IO;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Describes a data model.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class DataModelSchema : ObjectSchema
	{

		// Public Fields
		public System.String TargetNamespace;

		// Public Attributes
		public readonly MarkThree.MiddleTier.TableSchemaCollection Tables;
		public readonly MarkThree.MiddleTier.ComplexTypeSchemaCollection ComplexTypes;
		public readonly System.Decimal Version;
		public readonly System.String DurableStoreName;
		public readonly System.String Name;
		public readonly System.String TargetTableName;
		public readonly System.String VolatileStoreName;
		public readonly System.Xml.XmlNamespaceManager XmlNamespaceManager;

		/// <summary>
		/// Constructs a schema from the contents of an XML specification.
		/// </summary>
		/// <param name="fileContents">The contents of a file that specifies the schema in XML.</param>
		public DataModelSchema(string fileContents)
			: base(null, null)
		{

			// Initialize the object
			this.VolatileStoreName = "ADO Data Model";
			this.DurableStoreName = "SQL Data Model";

			// Create a string reader from the input string (which contains the source of the original file).  Read this data into
			// an XmlSchema object.
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(fileContents));
			XmlSchema xmlSchema = XmlSchema.Read(xmlTextReader, new ValidationEventHandler(ValidationCallback));

			// Compiling the Schema is critical for qualified name resolution.
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
			xmlSchemaSet.Add(xmlSchema);
			xmlSchemaSet.Compile();

			// The namespace manager is used to create qualified names from the XPath specifications.
			this.XmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			foreach (XmlQualifiedName xmlQualifiedName in xmlSchema.Namespaces.ToArray())
				this.XmlNamespaceManager.AddNamespace(xmlQualifiedName.Name, xmlQualifiedName.Namespace);

			// Initialize the data structures from the schema read in from the file.
			this.Name = xmlSchema.Id;
			this.Version = Convert.ToDecimal(xmlSchema.Version);
			this.Tables = GetTables(xmlSchema);
			this.ComplexTypes = GetComplexTypes(xmlSchema);

			// Initialize all the constraints.
			InitializeConstraints(xmlSchema);

		}

		/// <summary>
		/// Callback for parsing errors on the Xml Schema.
		/// </summary>
		/// <param name="sender">The object that originated the message.</param>
		/// <param name="args">The event arguments.</param>
		public void ValidationCallback(object sender, ValidationEventArgs args)
		{

			// Catch all parsing exceptions here.
			throw new Exception(args.Message);

		}

		/// <summary>
		/// Creates a collection of tables found in the schema.
		/// </summary>
		/// <param name="xmlSchema">The schema that describes the data model.</param>
		/// <returns>A list of TableSchema objects that describe the tables found in the data model schema.</returns>
		private TableSchemaCollection GetTables(XmlSchema xmlSchema)
		{

			// This listof TableSchemas is ordered by the qualified name of the tables.
			TableSchemaCollection tables = new TableSchemaCollection();

			// Scan through the schema looking for table elements.  These can either be defined at the root element of the schema
			// description or they can be found as choices of a special element describing a 'DataSet'.
			foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
				if (xmlSchemaObject is XmlSchemaElement)
				{

					// Extract the element from the list of items
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;

					// If the element read from the schema is a data set element, then the tables are described as choices
					// associated with an implicit complex type on that element.
					if (IsDataSetElement(xmlSchemaElement))
					{

						// The tables are described as an choices of an implicit complex type.
						if (xmlSchemaElement.SchemaType is XmlSchemaComplexType)
						{

							// This is the implicit complex type associated with the DataSet.  Here be tables.
							XmlSchemaComplexType xmlSchemaComplexType = xmlSchemaElement.SchemaType as XmlSchemaComplexType;
							if (xmlSchemaComplexType.Particle is XmlSchemaChoice)
							{

								// This is the choices that contain the tables in the DataSet element.
								XmlSchemaChoice xmlSchemaChoice = xmlSchemaComplexType.Particle as XmlSchemaChoice;
								foreach (XmlSchemaObject choiceObject in xmlSchemaChoice.Items)
									if (choiceObject is XmlSchemaElement)
										tables.Add(new TableSchema(this, choiceObject as XmlSchemaElement));
							}
						}

					}
					else

						// Traditionally the tables are described as elements of the schema.
						tables.Add(new TableSchema(this, xmlSchemaElement));

				}

			// This is the list of tables, whether found as members of a DataSet, or described as members of the schema.
			return tables;

		}

		/// <summary>
		/// Gets a list of complex types in a data model.
		/// </summary>
		/// <param name="xmlSchema">The schema that describes the data model.</param>
		/// <returns>A collection of complex types.</returns>
		private ComplexTypeSchemaCollection GetComplexTypes(XmlSchema xmlSchema)
		{

			// This will navigate through the highest level of the schema looking for explicit descriptions of types.
			ComplexTypeSchemaCollection typeSchemaCollection = new ComplexTypeSchemaCollection();
			foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
				if (xmlSchemaObject is XmlSchemaComplexType)
					typeSchemaCollection.Add(new ComplexTypeSchema(this, xmlSchemaObject as XmlSchemaComplexType));
			return typeSchemaCollection;

		}

		/// <summary>
		/// The constraints must be initialized after the tables are constructed.
		/// </summary>
		/// <param name="xmlSchema"></param>
		private void InitializeConstraints(XmlSchema xmlSchema)
		{

			// This will scan each of the top-level element of the schema looking for Unique and Key constraints.  These 
			// constraints must be defined before the KeyRef constraints are created because the KeyRef constraints reference the
			// Unique and Key constraints.
			foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
					foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in xmlSchemaElement.Constraints)
					{
						if (xmlSchemaIdentityConstraint is XmlSchemaUnique)
						{
							UniqueSchema uniqueSchema = new UniqueSchema(this, xmlSchemaIdentityConstraint as XmlSchemaUnique);
							uniqueSchema.Selector.Constraints.Add(uniqueSchema);
						}
						if (xmlSchemaIdentityConstraint is XmlSchemaKey)
						{
							KeySchema keySchema = new KeySchema(this, xmlSchemaIdentityConstraint as XmlSchemaKey);
							keySchema.Selector.Constraints.Add(keySchema);
						}
					}
				}

			// Once the Key and Unique constraints are described, the schema can be scanned for Keyref schemas.  The 'Refer'
			// property of the KeyrefSchema objects points to the KeySchema or UniqueSchema.
			foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
					foreach (XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint in xmlSchemaElement.Constraints)
						if (xmlSchemaIdentityConstraint is XmlSchemaKeyref)
						{
							KeyrefSchema keyrefSchema = new KeyrefSchema(this, xmlSchemaIdentityConstraint as XmlSchemaKeyref);
							keyrefSchema.Selector.Constraints.Add(keyrefSchema);
						}
				}


		}

	}

}
