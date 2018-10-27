namespace MarkThree.MiddleTier
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using System.Xml;
	using System.Xml.Schema;

	public class TableSchema : ObjectSchema, IComparable
	{

		// Private Members
		private MarkThree.MiddleTier.ConstraintSchema primaryKey;
		private MarkThree.MiddleTier.TableSchema baseTable;

		// Public Readonly Fields
		public readonly System.Boolean IsPersistent;
		public readonly System.String Name;
		public readonly System.Xml.XmlQualifiedName QualifiedName;
		public readonly MarkThree.MiddleTier.ColumnSchemaCollection Columns;
		public readonly MarkThree.MiddleTier.ConstraintSchemaCollection Constraints;
		public readonly MarkThree.MiddleTier.ComplexTypeSchema TypeSchema;
		public readonly System.Collections.Generic.List<ConstraintSchema> Keys;
		public readonly System.Collections.Generic.List<KeyrefSchema> ParentKeyrefs;

		public TableSchema(DataModelSchema dataModelSchema, XmlSchemaElement xmlSchemaElement)
			: base(dataModelSchema, xmlSchemaElement)
		{

			// Initialize the object.
			this.DataModelSchema = dataModelSchema;
			this.Name = xmlSchemaElement.Name;
			this.QualifiedName = xmlSchemaElement.QualifiedName;
			this.IsPersistent = GetIsPersistentAttribute(xmlSchemaElement);
			this.Columns = new ColumnSchemaCollection();
			this.Constraints = new ConstraintSchemaCollection();
			this.Keys = new List<ConstraintSchema>();
			this.ParentKeyrefs = new List<KeyrefSchema>();
			this.TypeSchema = GetTypeSchema(xmlSchemaElement);

			GetColumns(xmlSchemaElement.SchemaType, this.Columns);

			Constraints.ItemAdded += new ConstraintEvent(ConstraintAddedHandler);

		}

		public ConstraintSchema PrimaryKey { get { return this.primaryKey; } }

		public TableSchema BaseTable { get { return this.baseTable; } }

		private void ConstraintAddedHandler(object sender, ConstraintEventArgs constraintEventArgs)
		{

			if (constraintEventArgs.ConstraintSchema.IsPrimaryKey)
				this.primaryKey = constraintEventArgs.ConstraintSchema;

			if (constraintEventArgs.ConstraintSchema is KeySchema)
				this.Keys.Add(constraintEventArgs.ConstraintSchema as KeySchema);

			if (constraintEventArgs.ConstraintSchema is UniqueSchema)
				this.Keys.Add(constraintEventArgs.ConstraintSchema as UniqueSchema);

			if (constraintEventArgs.ConstraintSchema is KeyrefSchema)
			{

				KeyrefSchema keyrefSchema = constraintEventArgs.ConstraintSchema as KeyrefSchema;

				bool isMatch = keyrefSchema.Fields.Length == primaryKey.Fields.Length;
				for (int index = 0; index < keyrefSchema.Fields.Length; index++)
					if (keyrefSchema.Fields[index] != primaryKey.Fields[index])
					{
						isMatch = false;
						break;
					}

				if (isMatch)
					this.baseTable = keyrefSchema.Refer.Selector;

				this.ParentKeyrefs.Add(keyrefSchema);

			}

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
		/// Determines whether the specified object TableSchema object is equivalent to the current TableSchema object.
		/// </summary>
		/// <param name="obj">The TableSchema object to be compared.</param>
		/// <returns>true of the two objects are the same, false otherwise.</returns>
		public override bool Equals(object obj)
		{

			// The objects are equivalent if the other object is a TableSchema and their qualified names are the same.
			return obj is TableSchema ? this.QualifiedName.Equals((obj as TableSchema).QualifiedName) : false;

		}

		/// <summary>
		/// Determines if to TableSchemas are equal.
		/// </summary>
		/// <param name="firstTable">The first TableSchema to be compared.</param>
		/// <param name="secondTable">The second TableSchema to be compared.</param>
		/// <returns>true if the two TableSchemas are equal.</returns>
		public static bool operator ==(TableSchema firstTable, TableSchema secondTable)
		{

			// Since the TableSchema is a reference type, they will be equal if both are null.  If only one is null, then can't be
			// equal.  If neither of the fields is null, then the qualified names determine if they're equal.
			return Object.ReferenceEquals(firstTable, null) && Object.ReferenceEquals(secondTable, null) ? true :
				Object.ReferenceEquals(firstTable, null) || Object.ReferenceEquals(secondTable, null) ? false :
				firstTable.QualifiedName == secondTable.QualifiedName;

		}

		/// <summary>
		/// Determines if to TableSchemas are not equal.
		/// </summary>
		/// <param name="firstTable">The first TableSchema to be compared.</param>
		/// <param name="secondTable">The second TableSchema to be compared.</param>
		/// <returns>true if the two TableSchemas are not equal.</returns>
		public static bool operator !=(TableSchema firstTable, TableSchema secondTable)
		{

			// Since the TableSchema is a reference type, they will be equal if both are null.  If only one is null, then they 
			// can't be equal.  If neither of the fields is null, then the qualified names determine if they're equal or not.
			return Object.ReferenceEquals(firstTable, null) && Object.ReferenceEquals(secondTable, null) ? false :
				Object.ReferenceEquals(firstTable, null) || Object.ReferenceEquals(secondTable, null) ? true :
				firstTable.QualifiedName != secondTable.QualifiedName;

		}

		/// <summary>
		/// Gets the complex type of the table.
		/// </summary>
		/// <param name="xmlSchemaElement">The description of a table in the schema.</param>
		/// <returns>A ComplexTypeSchema representing the table schema.</returns>
		private ComplexTypeSchema GetTypeSchema(XmlSchemaElement xmlSchemaElement)
		{

			// The type can either be implicitly declared in-line with the table element or declared explicitly as a complex type.
			// The in-line types are extracted from the 'SchemaType' property.  The explicit types are found when the 
			// 'SchemaTypeName' is something other than the emtpy name.
			return xmlSchemaElement.SchemaTypeName == XmlQualifiedName.Empty ?
				new ComplexTypeSchema(this.DataModelSchema, xmlSchemaElement.SchemaType as XmlSchemaComplexType) :
				this.DataModelSchema.ComplexTypes.Find(xmlSchemaElement.SchemaTypeName);

		}

		private void GetColumns(XmlSchemaObject xmlSchemaObject, ColumnSchemaCollection columnSchemaCollection)
		{

			XmlSchemaComplexType baseType = FindBaseType(xmlSchemaObject);
			if (baseType != null)
				GetColumns(baseType, columnSchemaCollection);
			
			if (xmlSchemaObject is XmlSchemaComplexType)
			{

				XmlSchemaComplexType xmlSchemaComplexType = xmlSchemaObject as XmlSchemaComplexType;

				// Columns can be specified as attributes.
				foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
					columnSchemaCollection.Add(new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute));

				// The ComplexContent is mutually exclusive of the Particle.  That is, if there is no particle defined for this comlex
				// type then it must have a comlex content description.  Comlex content extends a base class.
				if (xmlSchemaComplexType.Particle == null)
				{

					// The Comlex Content describes an extension of a base class.  It is mutually exclusive of the Particle which 
					// describes a simple sequence of columns.
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
								foreach (XmlSchemaObject item in xmlSchemaSequence.Items)
								{
									ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, item);
									if (columnSchemaCollection.ContainsKey(columnSchema.QualifiedName))
										columnSchemaCollection.Remove(columnSchema.QualifiedName);
									columnSchemaCollection.Add(columnSchema);
								}

							}

							// The Complex Content can also contain attributes that describe columns.
							foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexContentExtension.Attributes)
							{
								ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute);
								if (columnSchemaCollection.ContainsKey(columnSchema.QualifiedName))
									columnSchemaCollection.Remove(columnSchema.QualifiedName);
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
						foreach (XmlSchemaObject item in xmlSchemaSequence.Items)
						{
							ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, item);
							if (columnSchemaCollection.ContainsKey(columnSchema.QualifiedName))
								columnSchemaCollection.Remove(columnSchema.QualifiedName);
							columnSchemaCollection.Add(columnSchema);
						}

					}

					// The ComplexType can also have attributes that describe table columns.
					foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaComplexType.Attributes)
					{
						ColumnSchema columnSchema = new ColumnSchema(this.DataModelSchema, xmlSchemaAttribute);
						if (columnSchemaCollection.ContainsKey(columnSchema.QualifiedName))
							columnSchemaCollection.Remove(columnSchema.QualifiedName);
						columnSchemaCollection.Add(columnSchema);
					}

				}

			}

		}

		/// <summary>
		/// Tests to see if a given column is part of the primary key.
		/// </summary>
		/// <param name="elementTable">The table schema to test.</param>
		/// <param name="columnSchema">The column schema to test.</param>
		/// <returns>True if the column belongs to the primary key of the table.</returns>
		public bool IsPrimaryKeyColumn(ColumnSchema columnSchema)
		{

			// Find the primary key of the requested table.
			ConstraintSchema keySchema = PrimaryKey;

			// Test each field in the key to see if the selector matches the given column name.
			if (keySchema != null)
				foreach (ColumnSchema fieldSchema in keySchema.Fields)
					if (fieldSchema == columnSchema)
						return true;

			// If there is a base class, then see if this column is part of the base class primary key.
			if (this.BaseTable != null)
				return this.BaseTable.IsPrimaryKeyColumn(columnSchema);

			// At this point, the column doesn not belong to any of the primary keys in the hiearchy.
			return false;

		}

		public bool IsAutogeneratedColumn(ColumnSchema columnSchema)
		{

			return columnSchema.IsAutoIncrement || (this.IsPrimaryKeyColumn(columnSchema) && this.BaseTable != null && this.BaseTable.HasIdentityColumn);

		}

		/// <summary>
		/// Indicates whether a column is or is descended from an identity column.
		/// </summary>
		/// <param name="elementTable">The table description.</param>
		/// <param name="columnSchema">The column description.</param>
		/// <returns>true of the column is or is descended from an identity column.</returns>
		public bool IsExternalIdColumn(ColumnSchema columnSchema)
		{

			if (IsPrimaryKeyColumn(columnSchema) && this.HasExternalIdColumn)
				return true;

			// At this point, the object hierarchy has been searched and no identity column was found.
			return false;

		}

		/// <summary>
		/// Tests to see if a column is an identity column.
		/// </summary>
		/// <param name="elementTable">The table to be tested for an identity column.</param>
		/// <returns>True if the table contains an identity column.</returns>
		public bool HasExternalIdColumn
		{

			get
			{

				// Check each column in the table to see if it has an 'Identity' attribute.
				foreach (ColumnSchema columnSchema in this.Columns)
					if (columnSchema.Name.IndexOf("ExternalId") != -1)
						return true;

				// If a base table exists, search it also for 'ExternalId' columns.
				TableSchema baseTable = this.BaseTable;
				return baseTable == null ? false : baseTable.HasExternalIdColumn;

			}

		}

		/// <summary>
		/// Tests to see if a column is an identity column.
		/// </summary>
		/// <param name="elementTable">The table to be tested for an identity column.</param>
		/// <returns>True if the table contains an identity column.</returns>
		public bool HasIdentityColumn
		{

			get
			{

				// Check each column in the table to see if it has an 'Identity' attribute.
				foreach (ColumnSchema columnSchema in this.Columns)
					if (columnSchema.IsAutoIncrement)
						return true;

				// Test to see if any of the base tables contains an identity column.
				TableSchema baseTable = this.BaseTable;
				return baseTable == null ? false : baseTable.HasIdentityColumn;

			}

		}

		/// <summary>
		/// Gets the parent keys of just the table requested.
		/// </summary>
		/// <param name="foreignKeyList"></param>
		/// <param name="elementTable"></param>
		private void GetMemberParentKeys(List<KeyrefSchema> foreignKeyList)
		{

			foreach (ConstraintSchema constraintSchema in this.Constraints)
				if (constraintSchema is KeyrefSchema)
				{
					KeyrefSchema keyrefSchema = constraintSchema as KeyrefSchema;
					foreignKeyList.Add(keyrefSchema);
				}

		}

		/// <summary>
		/// Finds foreign keys for a given table.
		/// </summary>
		/// <param name="elementTable">An element describing the table to be searched.</param>
		/// <param name="columnSchema">A collection of elements describing the columns in the foreign key.</param>
		/// <returns>A foreign key, or null if there is no match to the table/column combination.</returns>
		public KeyrefSchema[] MemberParentKeys
		{

			get
			{

				// This will hold the Keyref objects temporarily while they're collected.
				List<KeyrefSchema> foreignKeyList = new List<KeyrefSchema>();

				// Call the recursive procedure to fill in the array.
				GetMemberParentKeys(foreignKeyList);

				// At this point, no matching keys were found.
				return foreignKeyList.ToArray();

			}

		}

		/// <summary>
		/// Finds all child foreign keys for a given table.
		/// </summary>
		/// <param name="elementTable">An element describing the table to be searched.</param>
		/// <returns>An array of foreign key that are children of the given table.</returns>
		public KeyrefSchema[] ChildKeyrefs
		{

			get
			{

				// This will hold the Keyref objects temporarily while they're collected.
				List<KeyrefSchema> childKeyList = new List<KeyrefSchema>();

				foreach (TableSchema tableSchema in this.DataModelSchema.Tables)
					foreach (KeyrefSchema keyrefSchema in tableSchema.ParentKeyrefs)
						if (keyrefSchema.Refer.Selector == this)
							childKeyList.Add(keyrefSchema);

				return childKeyList.ToArray();

			}


		}

		/// <summary>
		/// Finds a foreign key that matches the table and collection of columns.
		/// </summary>
		/// <param name="elementTable">An element describing the table to be searched.</param>
		/// <param name="columnSchema">A collection of elements describing the columns in the foreign key.</param>
		/// <returns>A foreign key, or null if there is no match to the table/column combination.</returns>
		public KeyrefSchema FindForeignKey(ColumnSchema[] columns)
		{

			foreach (ConstraintSchema constraintSchema in this.Constraints)
				if (constraintSchema is KeyrefSchema)
				{

					// Create a Strongly Typed Keyref record.
					KeyrefSchema keyrefSchema = constraintSchema as KeyrefSchema;

					// A match must include all of the columns in the foreign key.  Try every permuatation of field against
					// field looking for a match.
					if (keyrefSchema.Selector == this)
					{

						int matches = 0;
						foreach (ColumnSchema fieldSchema in keyrefSchema.Fields)
							foreach (ColumnSchema columnSchema in columns)
								if (fieldSchema == columnSchema)
									matches++;

						// If the number of matches found is the same as the number of columns in the key, then this record is a match.
						if (matches == columns.Length)
							return keyrefSchema;

					}

				}

			// At this point, no matching keys were found.
			TableSchema baseTable = this.BaseTable;
			return baseTable == null ? null : baseTable.FindForeignKey(columns);

		}

		/// <summary>
		/// Indicates that a column is an identity column.
		/// </summary>
		/// <param name="columnSchema">The description of a column.</param>
		/// <returns>true if the column has an internally generated identifier.</returns>
		public bool IsIdentityColumn(ColumnSchema columnSchema)
		{

			if (columnSchema.IsAutoIncrement)
				return true;

			// See if the column may is part of an identity column in the base table.
			TableSchema baseTable = this.BaseTable;
			if (baseTable != null)
			{

				// See if the column is the identity column in the base table.
				if (baseTable.IsIdentityColumn(columnSchema))
					return true;

				// If the column is part of the primary key, then it's possible that the column is an identity column through a 
				// primary key relation.
				ConstraintSchema keySchema = this.PrimaryKey;
				if (keySchema != null && keySchema.Fields.Length == 1 && keySchema.Fields[0] == columnSchema)
				{

					// See if the column is related to an identity column through the primary index.
					ConstraintSchema baseKeySchema = baseTable.PrimaryKey;
					KeyrefSchema baseKeyRefSchema = this.FindForeignKey(new ColumnSchema[] { columnSchema });
					if (baseKeyRefSchema != null && baseKeyRefSchema.Refer == baseKeySchema)
						if (baseKeySchema.Fields.Length == 1)
						{
							ColumnSchema baseColumnSchema = baseTable.Columns[baseKeySchema.Fields[0].QualifiedName];
							return baseTable.IsExternalIdColumn(baseColumnSchema);
						}

				}

			}

			return false;

		}

		public bool GetIsPersistentAttribute(XmlSchemaElement xmlSchemaElement)
		{

			object isTablePersistentFlag = GetUnhandledAttribute(xmlSchemaElement, "persistent");
			return isTablePersistentFlag == null ? true : Convert.ToBoolean(isTablePersistentFlag);

		}

		public object GetUnhandledAttribute(XmlSchemaElement xmlSchemaElement, string attributeName)
		{

			// Cycle through each of the unhandled attributes (if any exist) looking for the 'AutoIncrement' attribute.
			if (xmlSchemaElement.UnhandledAttributes != null)
				foreach (XmlAttribute xmlAttribute in xmlSchemaElement.UnhandledAttributes)
					if (xmlAttribute.LocalName == attributeName)
						return xmlAttribute.Value;

			// At this point, the column element doesn't contain an attribute with the given name.
			return null;

		}

		public TableSchema[] TableHierarchy
		{

			get
			{

				List<TableSchema> tableHierarchy = new List<TableSchema>();
				TableSchema baseTable = this;
				while (baseTable != null)
				{
					tableHierarchy.Add(baseTable);
					baseTable = baseTable.BaseTable;
				}
				tableHierarchy.Reverse();
				return tableHierarchy.ToArray();

			}

		}

		public TableSchema[] Descendants
		{

			get
			{

				List<TableSchema> tableHierarchy = new List<TableSchema>();
				GetDescendants(tableHierarchy);
				return tableHierarchy.ToArray();

			}

		}

		private void GetDescendants(List<TableSchema> tableHierarchy)
		{

			foreach (KeyrefSchema childKeyref in this.ChildKeyrefs)
			{
				childKeyref.Selector.GetDescendants(tableHierarchy);
				if (!tableHierarchy.Contains(childKeyref.Selector))
					tableHierarchy.Add(childKeyref.Selector);
			}

		}

		public bool IsInheritedKey(ColumnSchema columnSchema)
		{

			return this.IsPrimaryKeyColumn(columnSchema) && this.BaseTable != null;

		}

		/// <summary>
		/// Indicates that the given foreign key is a reference to the base table.
		/// </summary>
		/// <param name="keyrefSchema">A foreign key description.</param>
		/// <returns>true if the given foreign key refers to the base table.</returns>
		public bool IsBaseKeyref(KeyrefSchema keyrefSchema)
		{

			// A true indicates that the given foreign key references the immediate base class of this table.
			return this.BaseTable != null && this.BaseTable == keyrefSchema.Refer.Selector;

		}

		public TableSchema RootTable
		{

			get
			{
				TableSchema rootTable = this;
				while (rootTable.BaseTable != null)
					rootTable = rootTable.BaseTable;
				return rootTable;
			}

		}

		/// <summary>
		/// Gets the number of times the specified table is referenced by parent keys.
		/// </summary>
		/// <param name="tableSchema"></param>
		/// <returns></returns>
		public int ParentKeyrefCount(TableSchema tableSchema)
		{

			int count = 0;
			foreach (KeyrefSchema keyrefSchema in this.ParentKeyrefs)
				if (keyrefSchema.Refer.Selector == tableSchema)
					count++;

			return count;

		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj is XmlQualifiedName)
			{
				XmlQualifiedName xmlQualifiedName = obj as XmlQualifiedName;
				int compare = this.QualifiedName.Namespace.CompareTo(xmlQualifiedName.Namespace);
				return compare == 0 ? this.QualifiedName.Name.CompareTo(xmlQualifiedName.Name) : compare;
			}

			if (obj is TableSchema)
			{
				TableSchema tableSchema = obj as TableSchema;
				int compare = this.QualifiedName.Namespace.CompareTo(tableSchema.QualifiedName.Namespace);
				return compare == 0 ? this.QualifiedName.Name.CompareTo(tableSchema.QualifiedName.Name) : compare;
			}

			throw new Exception("The method or operation is not implemented.");

		}

		#endregion
	}

}
