namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class ColumnSchema
	{

		private DataModelSchema schema;
		private XmlSchemaObject xmlSchemaObject;

		public ColumnSchema(DataModelSchema schema, XmlSchemaObject xmlSchemaObject)
		{

			// Initialize the object
			this.schema = schema;
			this.xmlSchemaObject = xmlSchemaObject;

		}

		public override string ToString() { return this.Name; }

		public string Name
		{
			
			get
			{

				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
					return xmlSchemaElement.Name;
				}

				if (xmlSchemaObject is XmlSchemaAttribute)
				{
					XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaObject as XmlSchemaAttribute;
					return xmlSchemaAttribute.Name;
				}

				return string.Empty;

			}

		}

		public XmlQualifiedName QualifiedName
		{

			get
			{

				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
					return xmlSchemaElement.QualifiedName;
				}

				if (xmlSchemaObject is XmlSchemaAttribute)
				{
					XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaObject as XmlSchemaAttribute;
					return xmlSchemaAttribute.QualifiedName;
				}

				return XmlQualifiedName.Empty;

			}

		}

		public override bool Equals(object obj)
		{
			if (obj is ColumnSchema)
				return this.QualifiedName.Equals((obj as ColumnSchema).QualifiedName);

			return false;
		}

		public override int GetHashCode()
		{
			return this.QualifiedName.GetHashCode();
		}

		public static bool operator==(ColumnSchema firstColumn, ColumnSchema secondColumn)
		{

			return Object.ReferenceEquals(firstColumn, null) && Object.ReferenceEquals(secondColumn, null) ? true :
				Object.ReferenceEquals(firstColumn, null) || Object.ReferenceEquals(secondColumn, null) ? false :
				firstColumn.QualifiedName == secondColumn.QualifiedName;
			
		}

		public static bool operator !=(ColumnSchema firstColumn, ColumnSchema secondColumn)
		{

			return Object.ReferenceEquals(firstColumn, null) && Object.ReferenceEquals(secondColumn, null) ? false :
				Object.ReferenceEquals(firstColumn, null) || Object.ReferenceEquals(secondColumn, null) ? true :
				firstColumn.QualifiedName != secondColumn.QualifiedName;

		}

		/// <summary>
		/// Indicates that a column is an identity column.
		/// </summary>
		/// <param name="columnSchema">The description of a column.</param>
		/// <returns>true if the column has an internally generated identifier.</returns>
		public bool IsIdentityColumn
		{

			get
			{
				// If an "AutoIncrement" attribute exists and evaluates to 'true', then the column generates it's own identifiers.
				object autoIncrementAttribute = GetUnhandledAttribute("AutoIncrement");
				bool autoIncrement = autoIncrementAttribute == null ? false : Convert.ToBoolean(autoIncrementAttribute);
				return autoIncrement;
			}

		}

		public object GetUnhandledAttribute(string attributeName)
		{

			// Cycle through each of the unhandled attributes (if any exist) looking for the 'AutoIncrement' attribute.
			if (this.xmlSchemaObject is XmlSchemaElement)
			{
				XmlSchemaElement xmlSchemaElement = this.xmlSchemaObject as XmlSchemaElement;
				if (xmlSchemaElement.UnhandledAttributes != null)
					foreach (XmlAttribute xmlAttribute in xmlSchemaElement.UnhandledAttributes)
						if (xmlAttribute.LocalName == attributeName)
							return xmlAttribute.Value;
			}

			// At this point, the column element doesn't contain an attribute with the given name.
			return null;

		}

		public Type DataType
		{

			get
			{

				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
					return xmlSchemaElement.ElementSchemaType.Datatype.ValueType;
				}

				if (xmlSchemaObject is XmlSchemaAttribute)
				{
					XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaObject as XmlSchemaAttribute;
					return xmlSchemaAttribute.AttributeSchemaType.Datatype.ValueType;
				}

				throw new Exception("Unrecognized Column Syntax in XML Specification");

			}

		}

		public bool IsAutoIncrement
		{

			get
			{

				// Autoincrement columns can't be initialized.
				object autoIncrement = GetUnhandledAttribute("AutoIncrement");
				return autoIncrement == null ? false : Convert.ToBoolean(autoIncrement);

			}

		}


		public bool IsPersistent
		{

			get
			{
				// Autoincrement columns can't be initialized.
				object isColumnPersistentFlag = GetUnhandledAttribute("IsPersistent");
				return isColumnPersistentFlag == null ? true : Convert.ToBoolean(isColumnPersistentFlag);
			}

		}

		public int AutoIncrementSeed
		{

			get
			{

				// Autoincrement columns can't be initialized.
				object autoIncrementSeed = GetUnhandledAttribute("AutoIncrementSeed");
				return autoIncrementSeed == null ? 0 : Convert.ToInt32(autoIncrementSeed);

			}

		}

		public int AutoIncrementStep
		{

			get
			{

				// Autoincrement columns can't be initialized.
				object autoIncrementStep = GetUnhandledAttribute("AutoIncrementStep");
				return autoIncrementStep == null ? 0 : Convert.ToInt32(autoIncrementStep);

			}

		}

		public decimal MinOccurs
		{

			get
			{

				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
					return xmlSchemaElement.MinOccurs;
				}

				if (xmlSchemaObject is XmlSchemaAttribute)
				{
					return 0;
				}

				throw new Exception("Unrecognized Column Syntax in XML Specification");

			}

		}

		public object FixedValue
		{

			get
			{

				string fixedValue = null;
				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
					fixedValue = xmlSchemaElement.FixedValue;
				}

				if (xmlSchemaObject is XmlSchemaAttribute)
				{
					XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaObject as XmlSchemaAttribute;
					fixedValue = xmlSchemaAttribute.FixedValue;
				}

				if (fixedValue == null)
					return null;

				// If the column provides a fixed value, then translate the fixed text into the proper datatype for the column.
				switch (this.DataType.ToString())
				{
				case "System.Boolean":
					return Convert.ToBoolean(fixedValue);
				case "System.Int16":
					return Convert.ToInt16(fixedValue);
				case "System.Int32":
					return Convert.ToInt32(fixedValue);
				case "System.Int64":
					return Convert.ToInt64(fixedValue);
				case "System.Decimal":
					return Convert.ToDecimal(fixedValue);
				case "System.Double":
					return Convert.ToDouble(fixedValue);
				case "System.String":
					return fixedValue;
				}

				throw new Exception("There is no legal fixed value for this column.");

			}

		}

		public object DefaultValue
		{

			get
			{

				string defaultValue = null;
				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
					defaultValue = xmlSchemaElement.DefaultValue;
				}

				if (xmlSchemaObject is XmlSchemaAttribute)
				{
					XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaObject as XmlSchemaAttribute;
					defaultValue = xmlSchemaAttribute.DefaultValue;
				}

				if (defaultValue == null)
					return this.MinOccurs == 0 ? DBNull.Value : null;

				// If the column provides a fixed value, then translate the default text into the proper datatype for the column.
				switch (this.DataType.ToString())
				{
				case "System.Boolean":
					return Convert.ToBoolean(defaultValue);
				case "System.Int16":
					return Convert.ToInt16(defaultValue);
				case "System.Int32":
					return Convert.ToInt32(defaultValue);
				case "System.Int64":
					return Convert.ToInt64(defaultValue);
				case "System.Decimal":
					return Convert.ToDecimal(defaultValue);
				case "System.Double":
					return Convert.ToDouble(defaultValue);
				case "System.String":
					return defaultValue;
				}

				// If the column allows for nulls, but doesn't use one of the known datatyptes, then an exception is thrown.
				if (this.MinOccurs == 0)
					return DBNull.Value;

				throw new Exception("There is no legal default value for this column.");

			}

		}

		public TypeSchema DeclaringType
		{

			get
			{

				XmlSchemaObject xmlSchemaObject = this.xmlSchemaObject.Parent;
				while (true)
				{

					if (xmlSchemaObject is XmlSchemaComplexContentExtension)
					{
						XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = xmlSchemaObject as XmlSchemaComplexContentExtension;
						TypeSchema typeSchema = this.schema.Classes.Find(xmlSchemaComplexContentExtension.BaseTypeName.Name);
						foreach (ColumnSchema columnSchema in typeSchema.Columns)
							if (columnSchema == this)
								return typeSchema;
					}

					if (xmlSchemaObject is XmlSchemaComplexType)
					{

						XmlSchemaComplexType xmlSchemaComplexType = xmlSchemaObject as XmlSchemaComplexType;

						if (xmlSchemaComplexType.Parent is XmlSchemaElement)
						{

							TableSchema tableSchema = new TableSchema(this.schema, xmlSchemaComplexType.Parent as XmlSchemaElement);

							TableSchema baseTable = tableSchema.BaseTable;
							while (baseTable != null)
							{

								ColumnSchema baseColumn;
								if (baseTable.MemberColumns.TryGetValue(this.Name, out baseColumn))
									if (baseColumn.DeclaringType == baseTable.GetTypeSchema())
										return baseTable.GetTypeSchema();

								baseTable = baseTable.BaseTable;

							}

						}


						return new TypeSchema(this.schema, xmlSchemaObject as XmlSchemaComplexType);

					}

					xmlSchemaObject = xmlSchemaObject.Parent;

				}

			}

		}

	}

}
