namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Describes a class (Complex Type) in a data model.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ComplexTypeSchema : ObjectSchema
	{

		// Public Readonly Fields
		public readonly System.String Name;
		public readonly System.Xml.XmlQualifiedName QualifiedName;

		/// <summary>
		/// Creates a description of a class (Complex Type) in a data model.
		/// </summary>
		/// <param name="dataModelSchema">The Schema of the entire data model.</param>
		/// <param name="xmlSchemaComplexType">The description of the complex type.</param>
		public ComplexTypeSchema(DataModelSchema dataModelSchema, XmlSchemaComplexType xmlSchemaComplexType) :
			base(dataModelSchema, xmlSchemaComplexType)
		{

			// Initialize the object.
			this.Name = GetName(xmlSchemaComplexType);
			this.QualifiedName = GetQualifiedName(xmlSchemaComplexType);

		}

		/// <summary>
		/// Gets the name of the complex type.
		/// </summary>
		/// <param name="xmlSchemaComplexType">The description of the complex type.</param>
		/// <returns>The name of the comlex type.</returns>
		public string GetName(XmlSchemaComplexType xmlSchemaComplexType)
		{

			// A comlex type takes its name from the owning element (table) if the type is implicit.
			if (xmlSchemaComplexType.Parent is XmlSchemaElement)
			{
				XmlSchemaElement parentElement = xmlSchemaComplexType.Parent as XmlSchemaElement;
				return parentElement.Name;
			}

			// When a comlex type is defined at the root level of the schema, it has its own name.
			return xmlSchemaComplexType.Name;

		}

		/// <summary>
		/// Gets the qualified name of the complex type.
		/// </summary>
		/// <param name="xmlSchemaComplexType">The description of the complex type.</param>
		/// <returns>The qualified name of the comlex type.</returns>
		public XmlQualifiedName GetQualifiedName(XmlSchemaComplexType xmlSchemaComplexType)
		{

			// A comlex type takes its name from the owning element (table) if the type is implicit.
			if (xmlSchemaComplexType.Parent is XmlSchemaElement)
			{
				XmlSchemaElement parentElement = xmlSchemaComplexType.Parent as XmlSchemaElement;
				return parentElement.QualifiedName;
			}

			// When a comlex type is defined at the root level of the schema, it has its own name.
			return xmlSchemaComplexType.QualifiedName;

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
		/// Determines whether the specified object TypeSchema object is equivalent to the current TypeSchema object.
		/// </summary>
		/// <param name="obj">The TypeSchema object to be compared.</param>
		/// <returns>true of the two objects are the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// The objects are equivalent if the other object is a TypeSchema and their qualified names are the same.
			return obj is ComplexTypeSchema ? this.QualifiedName.Equals((obj as ComplexTypeSchema).QualifiedName) : false;

		}

		/// <summary>
		/// Determines if to TypeSchemas are equal.
		/// </summary>
		/// <param name="firstType">The first TypeSchema to be compared.</param>
		/// <param name="secondType">The second TypeSchema to be compared.</param>
		/// <returns>true if the two TypeSchemas are equal.</returns>
		public static bool operator ==(ComplexTypeSchema firstType, ComplexTypeSchema secondType)
		{

			// Since the TypeSchema is a reference type, they will be equal if both are null.  If only one is null, then can't be
			// equal.  If neither of the fields is null, then the qualified names determine if they're equal.
			return Object.ReferenceEquals(firstType, null) && Object.ReferenceEquals(secondType, null) ? true :
				Object.ReferenceEquals(firstType, null) || Object.ReferenceEquals(secondType, null) ? false :
				firstType.QualifiedName == secondType.QualifiedName;

		}

		/// <summary>
		/// Determines if to TypeSchemas are not equal.
		/// </summary>
		/// <param name="firstType">The first TypeSchema to be compared.</param>
		/// <param name="secondType">The second TypeSchema to be compared.</param>
		/// <returns>true if the two TypeSchemas are not equal.</returns>
		public static bool operator !=(ComplexTypeSchema firstType, ComplexTypeSchema secondType)
		{

			// Since the TypeSchema is a reference type, they will be equal if both are null.  If only one is null, then they 
			// can't be equal.  If neither of the fields is null, then the qualified names determine if they're equal or not.
			return Object.ReferenceEquals(firstType, null) && Object.ReferenceEquals(secondType, null) ? false :
				Object.ReferenceEquals(firstType, null) || Object.ReferenceEquals(secondType, null) ? true :
				firstType.QualifiedName != secondType.QualifiedName;

		}

	}

}
