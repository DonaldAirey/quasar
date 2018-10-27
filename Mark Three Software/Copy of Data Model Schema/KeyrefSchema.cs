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

		public KeyrefSchema(DataModelSchema schema, XmlSchemaKeyref xmlSchemaKeyref)
			: base(schema, xmlSchemaKeyref)
		{

			// Initialize the object
			this.xmlSchemaKeyref = xmlSchemaKeyref;

		}

		public ConstraintSchema Refer
		{

			get
			{

				foreach (ObjectSchema objectSchema in this.DataModelSchema.Items)
				{

					if (objectSchema is DataSetSchema)
					{
						DataSetSchema dataSetSchema = objectSchema as DataSetSchema;
						foreach (ConstraintSchema constraintSchema in dataSetSchema.Constraints)
							if (constraintSchema is KeySchema || constraintSchema is UniqueSchema &&
								constraintSchema.QualifiedName == this.xmlSchemaKeyref.Refer)
								return constraintSchema;

					}

					if (objectSchema is TableSchema)
					{
						TableSchema tableSchema = objectSchema as TableSchema;
						foreach (ConstraintSchema constraintSchema in tableSchema.Constraints)
							if (constraintSchema is KeySchema)
							{
								KeySchema keySchema = constraintSchema as KeySchema;
								if (keySchema.QualifiedName == this.xmlSchemaKeyref.Refer)
									return keySchema;
							}
					}

				}

				throw new Exception(string.Format("Referential Integrity error in key {0}", this.Name));
		
			}

		}

		public override bool Equals(object obj)
		{
			if (obj is KeyrefSchema)
			{
				KeyrefSchema keyrefSchema = obj as KeyrefSchema;
				return this.QualifiedName.Equals(keyrefSchema.QualifiedName);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return this.QualifiedName.GetHashCode();
		}

		public static bool operator ==(KeyrefSchema keyrefOne, KeyrefSchema keyrefTwo)
		{
			return object.ReferenceEquals(keyrefOne, null) && object.ReferenceEquals(keyrefTwo, null) ? true :
				object.ReferenceEquals(keyrefOne, null) && !object.ReferenceEquals(keyrefTwo, null) ? false :
				!object.ReferenceEquals(keyrefTwo, null) && object.ReferenceEquals(keyrefTwo, null) ? false :
				keyrefOne.QualifiedName == keyrefTwo.QualifiedName;
		}

		public static bool operator !=(KeyrefSchema keyrefOne, KeyrefSchema keyrefTwo)
		{
			return object.ReferenceEquals(keyrefOne, null) && object.ReferenceEquals(keyrefTwo, null) ? false :
				object.ReferenceEquals(keyrefOne, null) && !object.ReferenceEquals(keyrefTwo, null) ? true :
				!object.ReferenceEquals(keyrefTwo, null) && object.ReferenceEquals(keyrefTwo, null) ? true :
				keyrefOne.QualifiedName != keyrefTwo.QualifiedName;
		}

		public bool IsNullable
		{

			get
			{

				foreach (ColumnSchema columnSchema in this.Fields)
					if (columnSchema.MinOccurs == 0)
						return true;

				return false;

			}

		}

	}

}
