namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.Schema;

	/// <summary>
	/// Describes a constraint on a table in a data model.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class ConstraintSchema : ObjectSchema, IComparable
	{

		// Public Readonly Fields
		public readonly MarkThree.MiddleTier.ColumnSchema[] Fields;
		public readonly MarkThree.MiddleTier.TableSchema Selector;
		public readonly System.String Name;
		public readonly System.Boolean IsPrimaryKey;
		public readonly System.Boolean IsNullable;
		public readonly System.Xml.XmlQualifiedName QualifiedName;

		/// <summary>
		/// Create a description of a constraint on a table.
		/// </summary>
		/// <param name="schema">The Schema of the entire data model.</param>
		/// <param name="xmlSchemaIdentityConstraint">The schema of a constraint.</param>
		public ConstraintSchema(DataModelSchema dataModelSchema, XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint)
			: base(dataModelSchema, xmlSchemaIdentityConstraint)
		{

			// Initialize the object.
			this.Name = xmlSchemaIdentityConstraint.Name;
			this.QualifiedName = xmlSchemaIdentityConstraint.QualifiedName;
			this.Selector = GetSelector(xmlSchemaIdentityConstraint);
			this.Fields = GetFields(xmlSchemaIdentityConstraint);
			this.IsPrimaryKey = GetPrimaryKeyStatus(xmlSchemaIdentityConstraint);
			this.IsNullable = GetNullableStatus();

		}

		/// <summary>
		/// The display text of the object.
		/// </summary>
		/// <returns></returns>
		public override string ToString() { return this.Name; }

		/// <summary>
		/// Returns the hash code for the ConstraintSchema.
		/// </summary>
		/// <returns>A code that can be use for hashing algorithms.</returns>
		public override int GetHashCode() { return this.QualifiedName.GetHashCode(); }

		/// <summary>
		/// Determines whether the specified object ConstraintSchema object is equivalent to the current ConstraintSchema object.
		/// </summary>
		/// <param name="obj">The ConstraintSchema object to be compared.</param>
		/// <returns>true of the two objects are the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// The objects are equivalent if the other object is a ConstraintSchema and their qualified names are the same.
			return obj is ConstraintSchema ? this.QualifiedName.Equals((obj as ConstraintSchema).QualifiedName) : false;

		}

		/// <summary>
		/// Determines if two ConstraintSchemas are equal.
		/// </summary>
		/// <param name="firstConstraint">The first ConstraintSchema to be compared.</param>
		/// <param name="secondConstraint">The second ConstraintSchema to be compared.</param>
		/// <returns>true if the two ConstraintSchemas are equal.</returns>
		public static bool operator ==(ConstraintSchema firstConstraint, ConstraintSchema secondConstraint)
		{

			// Since the ConstraintSchema is a reference type, they will be equal if both are null.  If only one is null, then can't be
			// equal.  If neither of the fields is null, then the qualified names determine if they're equal.
			return Object.ReferenceEquals(firstConstraint, null) && Object.ReferenceEquals(secondConstraint, null) ? true :
				Object.ReferenceEquals(firstConstraint, null) || Object.ReferenceEquals(secondConstraint, null) ? false :
				firstConstraint.QualifiedName == secondConstraint.QualifiedName;

		}

		/// <summary>
		/// Determines if to ConstraintSchemas are not equal.
		/// </summary>
		/// <param name="firstConstraint">The first ConstraintSchema to be compared.</param>
		/// <param name="secondConstraint">The second ConstraintSchema to be compared.</param>
		/// <returns>true if the two ConstraintSchemas are not equal.</returns>
		public static bool operator !=(ConstraintSchema firstConstraint, ConstraintSchema secondConstraint)
		{

			// Since the ConstraintSchema is a reference type, they will be equal if both are null.  If only one is null, then they 
			// can't be equal.  If neither of the fields is null, then the qualified names determine if they're equal or not.
			return Object.ReferenceEquals(firstConstraint, null) && Object.ReferenceEquals(secondConstraint, null) ? false :
				Object.ReferenceEquals(firstConstraint, null) || Object.ReferenceEquals(secondConstraint, null) ? true :
				firstConstraint.QualifiedName != secondConstraint.QualifiedName;

		}

		/// <summary>
		/// Gets the table referenced in the selector.
		/// </summary>
		/// <param name="xmlSchemaIdentityConstraint">The schema description of the constraint.</param>
		/// <returns>The table to which this constraint is bound.</returns>
		private TableSchema GetSelector(XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint)
		{

			// Pull apart the XPath string to get the instructions for navigating the document to the selector table.
			string[] tokens = xmlSchemaIdentityConstraint.Selector.XPath.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
			XmlSchemaObject navigationObject = xmlSchemaIdentityConstraint.Parent;
			foreach (string token in tokens)
			{
				switch (token)
				{
				case ".":

					break;

				case "..":

					navigationObject = navigationObject.Parent;
					break;

				default:

					// If the token isn't an instruction to move up or down in the hierarchy, then it specifies the qualified name 
					// of a table.  This will pull apart the token and construct a qualified name from the parts.  It is a simple
					// way to do an 'XPath' navigation, but it works reasonably well.
					string[] qNameParts = token.Split(':');
					XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(qNameParts[1],
						this.DataModelSchema.XmlNamespaceManager.LookupNamespace(qNameParts[0]));

					XmlSchemaElement parentElement = navigationObject as XmlSchemaElement;
					if (parentElement.SchemaType is XmlSchemaComplexType)
					{
						XmlSchemaComplexType xmlSchemaComplexType = parentElement.SchemaType as XmlSchemaComplexType;
						if (xmlSchemaComplexType.Particle is XmlSchemaChoice)
						{
							XmlSchemaChoice xmlSchemaChoice = xmlSchemaComplexType.Particle as XmlSchemaChoice;
							foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaChoice.Items)
								if (xmlSchemaObject is XmlSchemaElement)
								{
									XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
									if (xmlSchemaElement.QualifiedName == xmlQualifiedName)
									{
										navigationObject = xmlSchemaElement;
										break;
									}
								}
						}
					}
					break;

				}

			}

			// An element found after processing the instructions in the XPath is interpreted to be the selector table of the
			// constraint.
			if (navigationObject is XmlSchemaElement)
			{
				XmlSchemaElement xmlSchemaElement = navigationObject as XmlSchemaElement;
				return this.DataModelSchema.Tables.Find(xmlSchemaElement.QualifiedName);
			}

			// This indicates that the XPath specification couldn't be navigated.
			return null;

		}

		/// <summary>
		/// Gets the columns that make up the fields of a constraint.
		/// </summary>
		/// <param name="xmlSchemaIdentityConstraint">The constraint schema.</param>
		/// <returns>A set of columns that describe the fields in the constraint.</returns>
		private ColumnSchema[] GetFields(XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint)
		{

			// The fields of the constraint are collected in this object.
			List<ColumnSchema> columnList = new List<ColumnSchema>();

			// The raw schema contains the qualified names of all the columns in this constraint.  The tables are defined before 
			// the constraints are evaluated, so it's safe to reference the table and its columns while collecting the fields in
			// the constraint.
			foreach (XmlSchemaXPath xmlSchemaXPath in xmlSchemaIdentityConstraint.Fields)
			{

				// Pull apart the field and construct a qualified name from the XPath specification.  The qualified name can be used
				// to find the equivalent column in the selector's table.
				string[] qNameParts = xmlSchemaXPath.XPath.Split(':');
				XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(qNameParts[1],
					this.DataModelSchema.XmlNamespaceManager.LookupNamespace(qNameParts[0]));
				columnList.Add(this.Selector.Columns[xmlQualifiedName]);

			}

			// The list of fields is converted to an array of columns for the lifetime of this object.
			return columnList.ToArray();

		}

		/// <summary>
		/// Indicates whether any of the elements that make up the constraint can be a null value.
		/// </summary>
		/// <returns>true if any of the elements of the constraint can be nulled.</returns>
		private bool GetNullableStatus()
		{

			// This indicates that any of the elements of the constraint can be nulled.
			foreach (ColumnSchema columnSchema in this.Fields)
				if (columnSchema.MinOccurs == 0)
					return true;

			// This indicates that the columns making up a unique constraint can't hold a null value.
			return false;

		}

		#region IComparable Members

		/// <summary>
		/// Compares a ConstraintSchema to another object.
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

			// Compare the object to another ConstraintSchema.
			if (obj is ConstraintSchema)
			{
				ConstraintSchema constraintSchema = obj as ConstraintSchema;
				int compare = this.QualifiedName.Namespace.CompareTo(constraintSchema.QualifiedName.Namespace);
				return compare == 0 ? this.QualifiedName.Name.CompareTo(constraintSchema.Name) : compare;

			}

			// No other comparisons are recognized.
			throw new Exception(string.Format("The method or operation is not implemented for a {0} type.", obj.GetType()));

		}

		#endregion

	}

}
