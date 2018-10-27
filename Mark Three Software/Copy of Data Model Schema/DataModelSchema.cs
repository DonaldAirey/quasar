namespace MarkThree.MiddleTier
{

	using System;
	using System.CodeDom;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;
	using System.Xml.Serialization;
	using System.Text.RegularExpressions;

	public class DataModelSchema
	{

		// Private Fields
		private System.Xml.Schema.XmlSchema xmlSchema;
		
		// Public Attributes
		public System.String TargetNamespace;
		public System.String TargetTableName;
		public System.String DurableStoreName;
		public System.String VolatileStoreName;
		public readonly XmlNamespaceManager XmlNamespaceManager;

		/// <summary>
		/// Constructs a schema from the contents of an XML specification.
		/// </summary>
		/// <param name="fileContents">The contents of a file that specifies the schema in XML.</param>
		public DataModelSchema(string fileContents)
		{

			// Initialize the object
			this.VolatileStoreName = "ADO Data Model";
			this.DurableStoreName = "SQL Data Model";

			// Create a string reader from the input string (which contains the source of the original file).  Read this data into
			// an XmlSchema object.
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(fileContents));
			this.xmlSchema = XmlSchema.Read(xmlTextReader, new ValidationEventHandler(ValidationCallback));

			// Compiling the Schema is critical for qualified name resolution.
			XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
			xmlSchemaSet.Add(this.xmlSchema);
			xmlSchemaSet.Compile();

			// The namespace manager is used to create qualified names from the XPath specifications.
			this.XmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			foreach (XmlQualifiedName xmlQualifiedName in this.xmlSchema.Namespaces.ToArray())
				this.XmlNamespaceManager.AddNamespace(xmlQualifiedName.Name, xmlQualifiedName.Namespace);

		}

		public ItemIterator Items
		{
			get
			{
				return new ItemIterator(this, this.xmlSchema.Items);
			}
		}
		
		public string Name { get { return this.xmlSchema.Id; } }

		public decimal Version { get { return Convert.ToDecimal(this.xmlSchema.Version); } }

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

		public TableSchemaCollection Tables
		{

			get
			{

				TableSchemaCollection tables = new TableSchemaCollection();
				
				foreach (ObjectSchema objectSchema in this.Items)
				{

					if (objectSchema is DataSetSchema)
					{
						DataSetSchema dataSetSchema = objectSchema as DataSetSchema;
						foreach (TableSchema tableSchema in dataSetSchema.Tables)
							tables.Add(tableSchema);
					}

					if (objectSchema is TableSchema)
						tables.Add(objectSchema as TableSchema);

				}

				return tables;

			}

		}

		public TypeSchemaCollection Classes
		{

			get
			{
				TypeSchemaCollection classList = new TypeSchemaCollection();
				foreach (XmlSchemaObject schemaItem in this.xmlSchema.Items)
					if (schemaItem is XmlSchemaComplexType)
						classList.Add(new TypeSchema(this, schemaItem as XmlSchemaComplexType));
				return classList;
			}

		}

		public TypeSchema GetTypeSchema(XmlQualifiedName typeName)
		{

			foreach (XmlSchemaObject schemaItem in this.xmlSchema.Items)
				if (schemaItem is XmlSchemaComplexType)
				{
					XmlSchemaComplexType xmlSchemaComplexType = schemaItem as XmlSchemaComplexType;
					if (xmlSchemaComplexType.QualifiedName == typeName)
						return new TypeSchema(this, xmlSchemaComplexType);
				}

			return null;

		}
		
		public XmlSchemaComplexType GetComplexType(string name)
		{

			foreach (XmlSchemaObject schemaItem in this.xmlSchema.Items)
				if (schemaItem is XmlSchemaComplexType)
				{
					XmlSchemaComplexType xmlSchemaComplexType = schemaItem as XmlSchemaComplexType;
					if (xmlSchemaComplexType.Name == name)
						return xmlSchemaComplexType;
				}

			return null;

		}

		/// <summary>
		/// Recursively creates the list of columns from the schema and the inherited classes.
		/// </summary>
		/// <param name="xmlSchemaComplexType">The node in the Xml Schema containing the class definition.</param>
		private void GetColumns(XmlSchemaComplexType xmlSchemaComplexType, ColumnSchemaCollection columnSchemaCollection)
		{

			// Columns can be specified as attributes.
			foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
				columnSchemaCollection.Add(new ColumnSchema(this, xmlSchemaAttribute));

			// The ComplexContent is mutually exclusive of the Particle.  That is, if there is no particle defined for this comlex
			// type then it must have a comlex content description.  Comlex content extends a base class.
			if (xmlSchemaComplexType.Particle == null)
			{

				// The Comlex Content describes an extension of a base class.
				if (xmlSchemaComplexType.ContentModel is XmlSchemaComplexContent)
				{

					// Strongly type the XmlSchemaContent.
					XmlSchemaComplexContent xmlSchemaComplexContent = xmlSchemaComplexType.ContentModel as XmlSchemaComplexContent;

					// A complex content can be derived by extension (adding columns) or restriction (removing columns).  This
					// section will look for the extensions to the base class.
					if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentExtension)
					{

						// The Complex Content Extension describes a base class and the additional columns that make up a derived
						// class.  This section will recursively collect the columns from the base class and then parse out the
						// extra columns in-line.
						XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension =
							xmlSchemaComplexContent.Content as XmlSchemaComplexContentExtension;

						// This will recursively read the columns from the base classes.
						GetColumns(GetComplexType(xmlSchemaComplexContentExtension.BaseTypeName.Name), columnSchemaCollection);

						// The additional columns for this inherited table are found on the in-line in the <sequence> node that follows
						// the <extension> node.
						if (xmlSchemaComplexContentExtension.Particle is XmlSchemaSequence)
						{

							// Strongly type the XmlSchemaSequence
							XmlSchemaSequence xmlSchemaSequence = xmlSchemaComplexContentExtension.Particle as XmlSchemaSequence;

							// Read through the sequence and replace any column from an inherited class with the column in the
							// derived class.  Also note that the columns are added in alphabetical order to give some amount of
							// predictability to the way the parameter lists are constructed when there are several layers of
							// inheritance.
							foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaSequence.Items)
							{
								ColumnSchema columnSchema = new ColumnSchema(this, xmlSchemaObject);
								if (columnSchemaCollection.ContainsKey(columnSchema.Name))
									columnSchemaCollection.Remove(columnSchema.Name);
								columnSchemaCollection.Add(columnSchema);
							}

						}

						// The Complex Content can also contain attributes that describe columns.
						foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexContentExtension.Attributes)
						{
							ColumnSchema columnSchema = new ColumnSchema(this, xmlSchemaAttribute);
							if (columnSchemaCollection.ContainsKey(columnSchema.Name))
								columnSchemaCollection.Remove(columnSchema.Name);
							columnSchemaCollection.Add(columnSchema);
						}

					}

				}

			}
			else
			{

				// This section will parse the simple particle.  The particle has no inheritiance to evaluate.
				if (xmlSchemaComplexType.Particle is XmlSchemaSequence)
				{

					// Strongly type the XmlSchemaSequence member.
					XmlSchemaSequence xmlSchemaSequence = xmlSchemaComplexType.Particle as XmlSchemaSequence;

					// Each XmlSchemaElement on the Particle describes a column.
					foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaSequence.Items)
					{
						ColumnSchema columnSchema = new ColumnSchema(this, xmlSchemaObject);
						if (columnSchemaCollection.ContainsKey(columnSchema.Name))
							columnSchemaCollection.Remove(columnSchema.Name);
						columnSchemaCollection.Add(columnSchema);
					}

				}

				// The ComplexType can also have attributes that describe table columns.
				foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
				{
					ColumnSchema columnSchema = new ColumnSchema(this, xmlSchemaAttribute);
					if (columnSchemaCollection.ContainsKey(columnSchema.Name))
						columnSchemaCollection.Remove(columnSchema.Name);
					columnSchemaCollection.Add(columnSchema);
				}

			}

		}

		public ColumnSchemaCollection GetColumns(XmlSchemaComplexType xmlSchemaComplexType)
		{

			ColumnSchemaCollection columnSchemaCollection = new ColumnSchemaCollection();
			GetColumns(xmlSchemaComplexType, columnSchemaCollection);
			return columnSchemaCollection;

		}

		public ColumnSchemaCollection GetColumns(XmlQualifiedName xmlQualifiedName)
		{

			ColumnSchemaCollection columnSchemaCollection = new ColumnSchemaCollection();
			GetColumns(GetComplexType(xmlQualifiedName.Name), columnSchemaCollection);
			return columnSchemaCollection;

		}

	}

	public class ItemIterator
	{

		private DataModelSchema schema;
		private XmlSchemaObjectCollection xmlSchemaObjectCollection;

		public ItemIterator(DataModelSchema schema, XmlSchemaObjectCollection xmlSchemaObjectCollection)
		{

			this.schema = schema;
			this.xmlSchemaObjectCollection = xmlSchemaObjectCollection;

		}

		public IEnumerator<ObjectSchema> GetEnumerator() { return new ItemEnumerator(schema, this.xmlSchemaObjectCollection.GetEnumerator()); }

	}

	public class ItemEnumerator : IEnumerator<ObjectSchema>
	{

		private DataModelSchema schema;
		private XmlSchemaObjectEnumerator xmlSchemaObjectEnumerator;

		public ItemEnumerator(DataModelSchema schema, XmlSchemaObjectEnumerator xmlSchemaObjectEnumerator)
		{

			// Initialize the object
			this.schema = schema;
			this.xmlSchemaObjectEnumerator = xmlSchemaObjectEnumerator;

		}

		#region IEnumerator<ObjectSchema> Members

		public ObjectSchema Current
		{

			get
			{

				if (this.xmlSchemaObjectEnumerator.Current is XmlSchemaElement)
				{

					XmlSchemaElement xmlSchemaElement = this.xmlSchemaObjectEnumerator.Current as XmlSchemaElement;

					bool isDataSet = false;
					if (xmlSchemaElement.UnhandledAttributes != null)
						foreach (XmlAttribute xmlAttribute in xmlSchemaElement.UnhandledAttributes)
							if (xmlAttribute.LocalName == "IsDataSet")
								isDataSet = Convert.ToBoolean(xmlAttribute.Value);
					return isDataSet ? new DataSetSchema(this.schema, xmlSchemaElement) as ObjectSchema :
						new TableSchema(this.schema, xmlSchemaElement) as ObjectSchema;

				}

				return new ObjectSchema(this.xmlSchemaObjectEnumerator.Current);

			}

		}

		#endregion

		#region IDisposable Members

		public void Dispose() { }

		#endregion

		#region IEnumerator Members

		object IEnumerator.Current
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public bool MoveNext()
		{
			return this.xmlSchemaObjectEnumerator.MoveNext();
		}

		public void Reset()
		{
			this.xmlSchemaObjectEnumerator.Reset();
		}

		#endregion
	}

}
