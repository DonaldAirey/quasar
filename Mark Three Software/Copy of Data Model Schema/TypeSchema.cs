namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class TypeSchema
	{

		public readonly DataModelSchema Schema;
		private XmlSchemaComplexType xmlSchemaComplexType;

		public TypeSchema(DataModelSchema schema, XmlSchemaComplexType xmlSchemaComplexType)
		{

			// Initialize the object.
			this.Schema = schema;
			this.xmlSchemaComplexType = xmlSchemaComplexType;

		}

		public XmlQualifiedName QualifiedName
		{

			get
			{

				if (this.xmlSchemaComplexType.Parent is XmlSchemaElement)
				{
					XmlSchemaElement parentElement = this.xmlSchemaComplexType.Parent as XmlSchemaElement;
					return parentElement.QualifiedName;
				}

				return this.xmlSchemaComplexType.QualifiedName;

			}
		}
		
		public override bool Equals(object obj)
		{
			if (obj is TypeSchema)
				return this.QualifiedName.Equals((obj as TypeSchema).QualifiedName);

			return false;
		}

		public override int GetHashCode()
		{
			return this.QualifiedName.GetHashCode();
		}

		public static bool operator ==(TypeSchema firstType, TypeSchema secondType)
		{

			return Object.ReferenceEquals(firstType, null) && Object.ReferenceEquals(secondType, null) ? true :
				Object.ReferenceEquals(firstType, null) || Object.ReferenceEquals(secondType, null) ? false :
				firstType.QualifiedName == secondType.QualifiedName;

		}

		public static bool operator !=(TypeSchema firstType, TypeSchema secondType)
		{

			return Object.ReferenceEquals(firstType, null) && Object.ReferenceEquals(secondType, null) ? false :
				Object.ReferenceEquals(firstType, null) || Object.ReferenceEquals(secondType, null) ? true :
				firstType.QualifiedName != secondType.QualifiedName;

		}

		public ColumnSchemaCollection Columns { get { return this.Schema.GetColumns(this.xmlSchemaComplexType); } }

		public string Name
		{
			
			get
			{

				if (this.xmlSchemaComplexType.Parent is XmlSchemaElement)
				{
					XmlSchemaElement parentElement = this.xmlSchemaComplexType.Parent as XmlSchemaElement;
					return parentElement.Name;
				}
			
				return this.xmlSchemaComplexType.Name;

			}

		}

		public TableSchema BaseTable { get { return this.Schema.Tables.Find(this.Name); } }

		internal void GetColumns(ColumnSchemaCollection columnSchemaCollection)
		{

			// The ComplexContent is mutually exclusive of the Particle.  That is, if there is no particle defined for this comlex
			// type then it must have a comlex content description.  Comlex content extends a base class.
			if (this.xmlSchemaComplexType == null)
			{

				// The Comlex Content describes an extension of a base class.
				if (this.xmlSchemaComplexType.ContentModel is XmlSchemaComplexContent)
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
						TypeSchema baseType = this.Schema.GetTypeSchema(xmlSchemaComplexContentExtension.BaseTypeName);
						baseType.GetColumns(columnSchemaCollection);

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
								ColumnSchema columnSchema = new ColumnSchema(this.Schema, xmlSchemaObject);
								if (columnSchemaCollection.ContainsKey(columnSchema.Name))
									columnSchemaCollection.Remove(columnSchema.Name);
								columnSchemaCollection.Add(columnSchema);
							}

						}

						// The Complex Content can also contain attributes that describe columns.
						foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexContentExtension.Attributes)
						{
							ColumnSchema columnSchema = new ColumnSchema(this.Schema, xmlSchemaAttribute);
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
						ColumnSchema columnSchema = new ColumnSchema(this.Schema, xmlSchemaObject);
						if (columnSchemaCollection.ContainsKey(columnSchema.Name))
							columnSchemaCollection.Remove(columnSchema.Name);
						columnSchemaCollection.Add(columnSchema);
					}

				}

			}

		}

	}

}
