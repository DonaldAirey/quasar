namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml.Schema;

	public class KeyrefSchema : ConstraintSchema
	{

		// Private Members
		private XmlSchemaKeyref xmlSchemaKeyref;

		// Public Readonly Fields
		public readonly ConstraintSchema Refer;

		public KeyrefSchema(DataModelSchema dataModelSchema, XmlSchemaKeyref xmlSchemaKeyref)
			: base(dataModelSchema, xmlSchemaKeyref)
		{

			// Initialize the object
			this.xmlSchemaKeyref = xmlSchemaKeyref;
			this.Refer = GetRefer(xmlSchemaKeyref);

		}

		/// <summary>
		/// Returns the hash code for the ColumnSchema.
		/// </summary>
		/// <returns>A code that can be use for hashing algorithms.</returns>
		public override int GetHashCode() { return this.QualifiedName.GetHashCode(); }

		/// <summary>
		/// Determines whether the specified object KeyrefSchema object is equivalent to the current KeyrefSchema object.
		/// </summary>
		/// <param name="obj">The KeyrefSchema object to be compared.</param>
		/// <returns>true of the two objects are the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// The objects are equivalent if the other object is a KeyrefSchema and their qualified names are the same.
			return obj is KeyrefSchema ? this.QualifiedName.Equals((obj as KeyrefSchema).QualifiedName) : false;

		}

		/// <summary>
		/// Determines if two KeyrefSchemas are equal.
		/// </summary>
		/// <param name="firstKeyref">The first KeyrefSchema to be compared.</param>
		/// <param name="secondKeyref">The second KeyrefSchema to be compared.</param>
		/// <returns>true if the two KeyrefSchemas are equal.</returns>
		public static bool operator ==(KeyrefSchema firstKeyref, KeyrefSchema secondKeyref)
		{

			// Since the KeyrefSchema is a reference type, they will be equal if both are null.  If only one is null, then can't be
			// equal.  If neither of the fields is null, then the qualified names determine if they're equal.
			return Object.ReferenceEquals(firstKeyref, null) && Object.ReferenceEquals(secondKeyref, null) ? true :
				Object.ReferenceEquals(firstKeyref, null) || Object.ReferenceEquals(secondKeyref, null) ? false :
				firstKeyref.QualifiedName == secondKeyref.QualifiedName;

		}

		/// <summary>
		/// Determines if two KeyrefSchemas are not equal.
		/// </summary>
		/// <param name="firstKeyref">The first KeyrefSchema to be compared.</param>
		/// <param name="secondKeyref">The second KeyrefSchema to be compared.</param>
		/// <returns>true if the two KeyrefSchemas are not equal.</returns>
		public static bool operator !=(KeyrefSchema firstKeyref, KeyrefSchema secondKeyref)
		{

			// Since the KeyrefSchema is a reference type, they will be equal if both are null.  If only one is null, then they 
			// can't be equal.  If neither of the fields is null, then the qualified names determine if they're equal or not.
			return Object.ReferenceEquals(firstKeyref, null) && Object.ReferenceEquals(secondKeyref, null) ? false :
				Object.ReferenceEquals(firstKeyref, null) || Object.ReferenceEquals(secondKeyref, null) ? true :
				firstKeyref.QualifiedName != secondKeyref.QualifiedName;

		}

		private ConstraintSchema GetRefer(XmlSchemaKeyref xmlSchemaKeyref)
		{

			foreach (TableSchema tableSchema in this.DataModelSchema.Tables)
				foreach (ConstraintSchema constraintSchema in tableSchema.Constraints)
					if (constraintSchema is KeySchema || constraintSchema is UniqueSchema &&
						constraintSchema.QualifiedName == xmlSchemaKeyref.Refer)
						return constraintSchema;

			return null;

		}

	}

}
