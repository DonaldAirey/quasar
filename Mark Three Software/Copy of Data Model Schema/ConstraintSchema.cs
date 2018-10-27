namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;
	using System.Xml.XPath;

	public class ConstraintSchema : ObjectSchema
	{

		public readonly DataModelSchema DataModelSchema;
		private XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint;

		public ConstraintSchema(DataModelSchema schema, XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint)
			: base(xmlSchemaIdentityConstraint)
		{

			// Initialize the object.
			this.DataModelSchema = schema;
			this.xmlSchemaIdentityConstraint = xmlSchemaIdentityConstraint;

		}

		public override string ToString() { { return this.Name; } }

		public string Name { get { return this.xmlSchemaIdentityConstraint.Name; } }

		public XmlQualifiedName QualifiedName { get { return this.xmlSchemaIdentityConstraint.QualifiedName; } }


		public override bool Equals(object obj)
		{
			if (obj is KeySchema)
			{
				KeySchema keySchema = obj as KeySchema;
				return this.QualifiedName.Equals(keySchema.QualifiedName);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return this.QualifiedName.GetHashCode();
		}

		public static bool operator ==(ConstraintSchema constraintOne, ConstraintSchema constraintTwo)
		{
			return object.ReferenceEquals(constraintOne, null) && object.ReferenceEquals(constraintTwo, null) ? true :
				object.ReferenceEquals(constraintOne, null) && !object.ReferenceEquals(constraintTwo, null) ? false :
				!object.ReferenceEquals(constraintOne, null) && object.ReferenceEquals(constraintTwo, null) ? false :
				constraintOne.QualifiedName == constraintTwo.QualifiedName;
		}

		public static bool operator !=(ConstraintSchema constraintOne, ConstraintSchema constraintTwo)
		{
			return object.ReferenceEquals(constraintOne, null) && object.ReferenceEquals(constraintTwo, null) ? false :
				object.ReferenceEquals(constraintOne, null) && !object.ReferenceEquals(constraintTwo, null) ? true :
				!object.ReferenceEquals(constraintOne, null) && object.ReferenceEquals(constraintTwo, null) ? true :
				constraintOne.QualifiedName != constraintTwo.QualifiedName;
		}

		public ColumnSchema[] Fields
		{

			get
			{

				List<ColumnSchema> columnList = new List<ColumnSchema>();

				TableSchema tableSchema = this.Selector;

				foreach (XmlSchemaXPath xmlSchemaXPath in this.xmlSchemaIdentityConstraint.Fields)
				{
					string[] qNameParts = xmlSchemaXPath.XPath.Split(':');
					XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(qNameParts[1],
						this.DataModelSchema.XmlNamespaceManager.LookupNamespace(qNameParts[0]));

					foreach (ColumnSchema columnSchema in tableSchema.MemberColumns)
						if (columnSchema.QualifiedName == xmlQualifiedName)
							columnList.Add(columnSchema);
				}

				return columnList.ToArray();

			}

		}

		public TableSchema Selector
		{

			get
			{

				string[] tokens = this.xmlSchemaIdentityConstraint.Selector.XPath.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
				XmlSchemaObject navigationObject = this.xmlSchemaIdentityConstraint.Parent;
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

						string[] qNameParts = token.Split(':');
						XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(qNameParts[1],
							this.DataModelSchema.XmlNamespaceManager.LookupNamespace(qNameParts[0]));
						navigationObject = NavigateSchema(navigationObject, xmlQualifiedName);
						break;

					}

				}

				if (navigationObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = navigationObject as XmlSchemaElement;
					return new TableSchema(this.DataModelSchema, xmlSchemaElement);
				}

				throw new Exception(string.Format("Referential Integrity error in key {0}", this.Name));

			}

		}

		private XmlSchemaObject NavigateSchema(XmlSchemaObject navigationObject, XmlQualifiedName xmlQualifiedName)
		{

			XmlSchemaElement parentElement = navigationObject as XmlSchemaElement;
			if (parentElement.SchemaTypeName == XmlQualifiedName.Empty)
			{
				if (parentElement.SchemaType is XmlSchemaComplexType)
				{
					XmlSchemaComplexType xmlSchemaComplexType = parentElement.SchemaType as XmlSchemaComplexType;
					if (xmlSchemaComplexType.Particle is XmlSchemaChoice)
					{
						XmlSchemaChoice xmlSchemaChoice = xmlSchemaComplexType.Particle as XmlSchemaChoice;
						foreach (XmlSchemaObject xmlSchemaObject in xmlSchemaChoice.Items)
						{
							if (xmlSchemaObject is XmlSchemaElement)
							{
								XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
								if (xmlSchemaElement.QualifiedName == xmlQualifiedName)
									return xmlSchemaElement;
							}
						}
					}
				}
			}

			return null;

		}

		public bool IsPrimaryKey
		{

			get
			{

				object primaryKey = GetUnhandledAttribute(KeySchema.primaryKeyAttributeName);
				return primaryKey == null ? false : Convert.ToBoolean(primaryKey);

			}

		}

		public bool IsUnique
		{

			get
			{

				foreach (ColumnSchema columnSchema in this.Fields)
					if (columnSchema.MinOccurs != 1)
						return false;

				return true;

			}

		}

		public object GetUnhandledAttribute(XmlQualifiedName attributeName)
		{

			// Cycle through each of the unhandled attributes (if any exist) looking for the 'AutoIncrement' attribute.
			if (this.xmlSchemaIdentityConstraint.UnhandledAttributes != null)
				foreach (XmlAttribute xmlAttribute in this.xmlSchemaIdentityConstraint.UnhandledAttributes)
					if (xmlAttribute.NamespaceURI == attributeName.Namespace && xmlAttribute.LocalName == attributeName.Name)
						return xmlAttribute.Value;

			// At this point, the column element doesn't contain an attribute with the given name.
			return null;

		}

	}

}
