namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Describes a column in a data model.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ColumnSchema : ObjectSchema, IComparable
	{

		// Private Fields
		private MarkThree.MiddleTier.DataModelSchema dataModelSchema;

		// Public Readonly Fields
		public readonly MarkThree.MiddleTier.ComplexTypeSchema DeclaringType;
		public readonly System.Boolean IsAutoIncrement;
		public readonly System.Boolean IsPersistent;
		public readonly System.Int32 AutoIncrementSeed;
		public readonly System.Int32 AutoIncrementStep;
		public readonly System.Object DefaultValue;
		public readonly System.Object FixedValue;
		public readonly System.Decimal MinOccurs;
		public readonly System.String Name;
		public readonly System.Type DataType;
		public readonly System.Xml.XmlQualifiedName QualifiedName;

		/// <summary>
		/// Create a description of a column in a data model.
		/// </summary>
		/// <param name="dataModelSchema">The Schema of the entire data model.</param>
		/// <param name="xmlSchemaObject">The schema of the column.</param>
		public ColumnSchema(DataModelSchema dataModelSchema, XmlSchemaObject xmlSchemaObject) :
			base(dataModelSchema, xmlSchemaObject)
		{

			// Initialize the object
			this.dataModelSchema = dataModelSchema;
			this.Name = string.Empty;
			this.QualifiedName = XmlQualifiedName.Empty;
			this.DataType = typeof(System.Object);
			this.MinOccurs = 0.0M;
			this.DefaultValue = null;
			this.FixedValue = null;

			// Extract the column properties from an Element.
			if (xmlSchemaObject is XmlSchemaElement)
			{
				XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
				this.Name = xmlSchemaElement.Name;
				this.QualifiedName = xmlSchemaElement.QualifiedName;
				this.DataType = xmlSchemaElement.ElementSchemaType.Datatype.ValueType;
				this.MinOccurs = xmlSchemaElement.MinOccurs;
				this.DefaultValue = ConvertValue(xmlSchemaElement.DefaultValue);
				this.FixedValue = ConvertValue(xmlSchemaElement.FixedValue);
			}

			// Extract the column properties from an Attribute.			
			if (xmlSchemaObject is XmlSchemaAttribute)
			{
				XmlSchemaAttribute xmlSchemaAttribute = xmlSchemaObject as XmlSchemaAttribute;
				this.Name = xmlSchemaAttribute.Name;
				this.QualifiedName = xmlSchemaAttribute.QualifiedName;
				this.DataType = xmlSchemaAttribute.AttributeSchemaType.Datatype.ValueType;
				this.DefaultValue = ConvertValue(xmlSchemaAttribute.DefaultValue);
				this.FixedValue = ConvertValue(xmlSchemaAttribute.FixedValue);
			}

			// Determine the IsIdentityColumn property.
			object autoIncrementAttribute = GetUnhandledAttribute(xmlSchemaObject, "AutoIncrement");
			this.IsAutoIncrement = autoIncrementAttribute == null ? false : Convert.ToBoolean(autoIncrementAttribute);

			// Determine the IsPersistent property.
			object isColumnPersistentAttribute = GetUnhandledAttribute(xmlSchemaObject, "IsPersistent");
			this.IsPersistent = isColumnPersistentAttribute == null ? true : Convert.ToBoolean(isColumnPersistentAttribute);

			// Determine the AutoIncrementSeed property.
			object autoIncrementSeedAttribute = GetUnhandledAttribute(xmlSchemaObject, "AutoIncrementSeed");
			this.AutoIncrementSeed = autoIncrementSeedAttribute == null ? 0 : Convert.ToInt32(autoIncrementSeedAttribute);

			// Determine the AutoIncrementStop property
			object autoIncrementStepAttribute = GetUnhandledAttribute(xmlSchemaObject, "AutoIncrementStep");
			this.AutoIncrementStep = autoIncrementStepAttribute == null ? 0 : Convert.ToInt32(autoIncrementStepAttribute);

			// In an object oriented architecture it is important to know at which level of the hierarchy a column is initially
			// declared.  Just like the equivalent in the object oriented languages, this field indicates which table actually owns
			// the data of a given column.
			XmlSchemaObject parentObject = xmlSchemaObject.Parent;
			while (!(parentObject is XmlSchemaComplexContentExtension) && !(parentObject is XmlSchemaComplexType))
				parentObject = parentObject.Parent;
			this.DeclaringType = FindDeclaringType(parentObject, xmlSchemaObject);

		}

		/// <summary>
		/// The display text of the object.
		/// </summary>
		/// <returns></returns>
		public override string ToString() { return this.Name; }

		/// <summary>
		/// Returns the hash code for the ColumnSchema.
		/// </summary>
		/// <returns>A code that can be use for hashing algorithms.</returns>
		public override int GetHashCode() { return this.QualifiedName.GetHashCode(); }

		/// <summary>
		/// Determines whether the specified object ColumnSchema object is equivalent to the current ColumnSchema object.
		/// </summary>
		/// <param name="obj">The ColumnSchema object to be compared.</param>
		/// <returns>true of the two objects are the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// The objects are equivalent if the other object is a ColumnSchema and their qualified names are the same.
			return obj is ColumnSchema ? this.QualifiedName.Equals((obj as ColumnSchema).QualifiedName) : false;

		}

		/// <summary>
		/// Determines if two ColumnSchemas are equal.
		/// </summary>
		/// <param name="firstColumn">The first ColumnSchema to be compared.</param>
		/// <param name="secondColumn">The second ColumnSchema to be compared.</param>
		/// <returns>true if the two ColumnSchemas are equal.</returns>
		public static bool operator==(ColumnSchema firstColumn, ColumnSchema secondColumn)
		{

			// Since the ColumnSchema is a reference type, they will be equal if both are null.  If only one is null, then can't be
			// equal.  If neither of the fields is null, then the qualified names determine if they're equal.
			return Object.ReferenceEquals(firstColumn, null) && Object.ReferenceEquals(secondColumn, null) ? true :
				Object.ReferenceEquals(firstColumn, null) || Object.ReferenceEquals(secondColumn, null) ? false :
				firstColumn.QualifiedName == secondColumn.QualifiedName;
			
		}

		/// <summary>
		/// Determines if two ColumnSchemas are not equal.
		/// </summary>
		/// <param name="firstColumn">The first ColumnSchema to be compared.</param>
		/// <param name="secondColumn">The second ColumnSchema to be compared.</param>
		/// <returns>true if the two ColumnSchemas are not equal.</returns>
		public static bool operator !=(ColumnSchema firstColumn, ColumnSchema secondColumn)
		{

			// Since the ColumnSchema is a reference type, they will be equal if both are null.  If only one is null, then they 
			// can't be equal.  If neither of the fields is null, then the qualified names determine if they're equal or not.
			return Object.ReferenceEquals(firstColumn, null) && Object.ReferenceEquals(secondColumn, null) ? false :
				Object.ReferenceEquals(firstColumn, null) || Object.ReferenceEquals(secondColumn, null) ? true :
				firstColumn.QualifiedName != secondColumn.QualifiedName;

		}

		/// <summary>
		/// Finds additional attributes not parsed by the XmlSchema reader.
		/// </summary>
		/// <param name="attributeName">The name of the unhandled attribute.</param>
		/// <returns>The text of the attribute or 'null' if the attribute isn't part of the element.</returns>
		public object GetUnhandledAttribute(XmlSchemaObject xmlSchemaObject, string attributeName)
		{

			// Cycle through each of the unhandled attributes (if any exist) looking for the 'AutoIncrement' attribute.
			if (xmlSchemaObject is XmlSchemaElement)
			{
				XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
				if (xmlSchemaElement.UnhandledAttributes != null)
					foreach (XmlAttribute xmlAttribute in xmlSchemaElement.UnhandledAttributes)
						if (xmlAttribute.LocalName == attributeName)
							return xmlAttribute.Value;
			}

			// At this point, the column element doesn't contain an attribute with the given name.
			return null;

		}

		/// <summary>
		/// Converts the text of a value into a value.
		/// </summary>
		/// <param name="value">The text of a value.</param>
		/// <returns>The text converted into a base type object.</returns>
		private object ConvertValue(object value)
		{

			// Convert the text based on the datatype of the column.
			if (value != null)
				switch (this.DataType.ToString())
				{
				case "System.Boolean":
					return Convert.ToBoolean(value);
				case "System.Int16":
					return Convert.ToInt16(value);
				case "System.Int32":
					return Convert.ToInt32(value);
				case "System.Int64":
					return Convert.ToInt64(value);
				case "System.Decimal":
					return Convert.ToDecimal(value);
				case "System.Double":
					return Convert.ToDouble(value);
				case "System.String":
					return value;
				}

			return null;

		}

		/// <summary>
		/// Finds the ComplexTypeSchema where the column was initially declared.
		/// </summary>
		/// <returns>The ComplexTypeSchema where the column was defined.</returns>
		private ComplexTypeSchema FindDeclaringType(XmlSchemaObject typeObject, XmlSchemaObject columnObject)
		{

			// If the column was declared in the base class, then it owns it.  Note that the class is recursive and will continue
			// to search the hiararchy until the column is found.  If it wasn't found anywhere in the base classes, then the
			// current level of the hierarchy will be searched to see if it owns the column.
			XmlSchemaComplexType baseTypeObject = FindBaseType(typeObject);
			if (baseTypeObject != null)
			{
				ComplexTypeSchema declaringType = FindDeclaringType(baseTypeObject, columnObject);
				if (declaringType != null)
					return declaringType;
			}

			// If the column doesn't belong to one of the base classes, then see if it was declared at the current level of the 
			// hierarchy.
			if (typeObject is XmlSchemaComplexType)
			{
				XmlSchemaComplexType xmlSchemaComplexType = typeObject as XmlSchemaComplexType;
				if (ContainsColumn(xmlSchemaComplexType, columnObject))
					return new ComplexTypeSchema(this.dataModelSchema, xmlSchemaComplexType);
			}

			// At this point, the specified column hasn't been found at this level of the hiearchy.  This will allow the recursion
			// to unwind and the next level up, if it exists, will be tested.
			return null;

		}

		#region IComparable Members

		/// <summary>
		/// Compares a ColumnSchema to another object.
		/// </summary>
		/// <param name="obj">The object to be compared.</param>
		/// <returns>-1 if this object is less than the specified object, -1 if it is greater and 0 if they are equal.</returns>
		public int CompareTo(object obj)
		{

			// Compare the object to an XmlQualifiedName.
			if (obj is XmlQualifiedName)
			{
				XmlQualifiedName xmlQualifiedName = obj as XmlQualifiedName;
				int compare = this.QualifiedName.Namespace.CompareTo(xmlQualifiedName.Namespace);
				return compare == 0 ? this.QualifiedName.Name.CompareTo(xmlQualifiedName.Name) : compare;
			}

			// Compare the object to another ColumnSchema.
			if (obj is ColumnSchema)
			{
				ColumnSchema columnSchema = obj as ColumnSchema;
				int compare = this.QualifiedName.Namespace.CompareTo(columnSchema.QualifiedName.Namespace);
				return compare == 0 ? this.QualifiedName.Name.CompareTo(columnSchema.Name) : compare;

			}

			// No other comparisons are recognized.
			throw new Exception(string.Format("The method or operation is not implemented for a {0} type.", obj.GetType()));

		}

		#endregion

	}

}
